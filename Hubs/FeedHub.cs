using BlueBook.Controllers;
using BlueBook.Data;
using BlueBook.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
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

            if (mensagem.NomeUsuario != Context.User.Identity.Name) return;

            var usuario = context.Users.FirstOrDefault(u => u.UserName == mensagem.NomeUsuario);

            if (usuario == null) return;

            context.Mensagem.Add(mensagem);
            context.SaveChanges();

            Mensagem retorno = new Mensagem
            {
                NomeUsuario = mensagem.NomeUsuario,
                Texto = mensagem.Texto,
                DataEnvio = mensagem.DataEnvio,
                UsuarioID = usuario.Id
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

        public async Task EnviarMensagensPrivadas(Mensagem mensagem)
        {
            if (mensagem.UsuarioID != Context.UserIdentifier) return;

            Usuario usuarioOrigem = await userManager.FindByIdAsync(mensagem.UsuarioID);
            Usuario usuarioDestino = await userManager.FindByIdAsync(mensagem.AlvoId);

            if (usuarioDestino == null) return;

            mensagem.DataEnvio = DateTime.Now;

            context.Mensagem.Add(mensagem);
            context.SaveChanges();
            Mensagem retorno = new Mensagem
            {
                NomeUsuario = mensagem.NomeUsuario,
                Texto = mensagem.Texto,
                DataEnvio = mensagem.DataEnvio,
                UsuarioID = mensagem.UsuarioID
            };

            string[] usuarios = new string[] { usuarioDestino.UserName, usuarioOrigem.UserName};
            Array.Sort(usuarios, 0, 2);
            string group = usuarios[0] + usuarios[1];

            await Clients.Group(group).SendAsync("ReceberMensagemPrivada", retorno, usuarioDestino.Id);
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
