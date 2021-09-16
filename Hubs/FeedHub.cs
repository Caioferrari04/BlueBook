using BlueBook.Controllers;
using BlueBook.Data;
using BlueBook.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlueBook.Hubs
{
    public class FeedHub : Hub
    {
        readonly UserManager<Usuario> userManager;
        BookContext context;
        public FeedHub(BookContext context, UserManager<Usuario> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public async Task EnviarMensagem(Mensagem mensagem)
        {
            mensagem.DataEnvio = DateTime.Now;

            /*Alterado de Context.User.Identity.Name para Context.UserIdentifier por conta da possibilidade de alteração de nome,
             permitindo que a mensagem seja enviada, pois Context.User.Identity.Name comparava o nome do usuario atual com o
             seu email.*/
            if (mensagem.UsuarioID != Context.UserIdentifier) return;

            Usuario usuarioAtual = await userManager.FindByIdAsync(mensagem.UsuarioID);

            if (usuarioAtual == null) return;

            if (mensagem.Texto.Trim() == "") return;

            context.Mensagem.Add(mensagem);
            context.SaveChanges();

            Mensagem retorno = new Mensagem
            {
                NomeUsuario = mensagem.NomeUsuario,
                Texto = mensagem.Texto,
                DataEnvio = mensagem.DataEnvio,
                UsuarioID = usuarioAtual.Id,
                usuario = new Usuario { LinkImagem = usuarioAtual.LinkImagem }
            };

            await Clients.All.SendAsync("ReceberMensagem", retorno);
        }

        public async Task EnviarPedidoAmizade(string origem, string destino)
        {
            if (origem != Context.UserIdentifier) return;

            Usuario usuarioOrigem = await userManager.FindByIdAsync(origem);
            Usuario usuarioDestino = await userManager.FindByIdAsync(destino);

            if (usuarioDestino == null) return;

            /*Como o usuário que envia o pedido não pode ver o pedido aparecendo na tela, apenas o destinatário deve receber*/
            await Clients.User(usuarioDestino.Id).SendAsync("PedidoAmizade", usuarioOrigem.Id, usuarioDestino.Id, usuarioOrigem.UserName);
        }
        public string GetConnectionId() => Context.ConnectionId;

        public async Task AceitarPedidoAmizade(string origem, string destino)
        {

            if (destino != Context.UserIdentifier) return;

            Usuario usuarioOrigem = await userManager.FindByIdAsync(origem);
            Usuario usuarioDestino = await userManager.FindByIdAsync(destino);

            if (usuarioOrigem == null) return;

            List<string> usuarios = new List<string>();
            usuarios.Add(usuarioOrigem.Id);
            usuarios.Add(usuarioDestino.Id);

            Amizade novaAmizadeOrigem = new Amizade()
            {
                Origem = usuarioOrigem,
                Alvo = usuarioDestino
            };

            Amizade novaAmizadeAlvo = new Amizade()
            {
                OrigemId = usuarioDestino.Id,
                AlvoId = usuarioOrigem.Id
            };

            List<Amizade> amizades = new List<Amizade>() { novaAmizadeOrigem };
            usuarioOrigem.Origem = amizades;

            amizades[0] = novaAmizadeAlvo;
            usuarioDestino.Origem = amizades;
            context.Update(usuarioOrigem);
            context.Update(usuarioDestino);
            context.SaveChanges();

            await Clients.Users(usuarios).SendAsync("AceitarPedido", usuarioDestino.UserName);
        }

        public async Task RecusarPedidoAmizade(string origem, string destino)
        {
            var usuarioDestino = await userManager.FindByIdAsync(destino);

            if (usuarioDestino == null) return;

            List<string> usuarios = new List<string>();
            usuarios.Add(origem);
            usuarios.Add(usuarioDestino.Id);

            await Clients.Users(usuarios).SendAsync("RecusarPedido", usuarioDestino.UserName);
        }

        /*Feito para carregar mensagens em caso do usuário receber uma mensagem enquanto está com o chat privado fechado*/
        public async Task CarregarMensagensEnviadas(string origem, Mensagem MensagemRecente, string destino)
        {
            if (origem != Context.UserIdentifier) return;

            var mensagens = from mensagem in context.Set<Mensagem>()
                            join usuario in context.Set<Usuario>()
                            on mensagem.UsuarioID equals usuario.Id
                            select new Mensagem()
                            {
                                Texto = mensagem.Texto,
                                AlvoId = mensagem.AlvoId,
                                UsuarioID = usuario.Id,
                                DataEnvio = mensagem.DataEnvio,
                                NomeUsuario = usuario.UserName,
                                ID = mensagem.ID,
                                usuario = new Usuario()
                                {
                                    LinkImagem = usuario.LinkImagem
                                }
                            };
            var mensagemPreRetorno = await mensagens.ToListAsync();
            var mensagemMaisRecenteCompleta = mensagemPreRetorno.Find(m => m.ID == MensagemRecente.ID);
            List<Mensagem> mensagemRetorno = mensagemPreRetorno.Where(m => m.DataEnvio > mensagemMaisRecenteCompleta.DataEnvio).ToList();
            mensagemRetorno = mensagemRetorno.Where(m => (m.AlvoId == destino || m.UsuarioID == destino) && (m.AlvoId == origem || m.UsuarioID == origem)).ToList();
            await Clients.User(origem).SendAsync("CarregarMensagens", mensagemRetorno, destino);
        }

        public async Task EnviarMensagensPrivadas(Mensagem mensagem)
        {
            if (mensagem.UsuarioID != Context.UserIdentifier) return;

            Usuario usuarioOrigem = await userManager.FindByIdAsync(mensagem.UsuarioID);
            Usuario usuarioDestino = await userManager.FindByIdAsync(mensagem.AlvoId);

            if (usuarioDestino == null) return;

            mensagem.DataEnvio = DateTime.Now;

            if (mensagem.Texto.Trim() == "") return;

            context.Mensagem.Add(mensagem);
            context.SaveChanges();
            Mensagem retorno = new Mensagem
            {
                NomeUsuario = mensagem.NomeUsuario,
                Texto = mensagem.Texto,
                DataEnvio = mensagem.DataEnvio,
                UsuarioID = mensagem.UsuarioID,
                AlvoId = mensagem.AlvoId,
                usuario = new Usuario { LinkImagem = usuarioOrigem.LinkImagem}
            };

            string[] usuarios = new string[] { usuarioDestino.UserName, usuarioOrigem.UserName};
            Array.Sort(usuarios, 0, 2);
            string group = usuarios[0] + usuarios[1];

            await Clients.Group(group).SendAsync("ReceberMensagemPrivada", retorno, usuarioDestino.Id);
            await Clients.User(usuarioDestino.Id).SendAsync("NotificacaoMensagemNova", usuarioOrigem.Id);
        }

        public Task EntrarGrupo(string destino)
        {
            var origem = Context.User.Identity.Name;
            string[] usuarios = new string[] { destino, origem};
            Array.Sort(usuarios, 0, 2);
            string group = usuarios[0] + usuarios[1];
            return Groups.AddToGroupAsync(Context.ConnectionId, group);
        }
        public Task SairGrupo(string destino)
        {
            var origem = Context.User.Identity.Name;
            string[] usuarios = new string[] { destino, origem };
            Array.Sort(usuarios, 0, 2);
            string group = usuarios[0] + usuarios[1];
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
        }
    }
}
