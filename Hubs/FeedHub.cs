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

            List<Usuario> amigos = usuarioOrigem.Amigos != null ? usuarioOrigem.Amigos : new List<Usuario>();

            amigos.Add(usuarioDestino);
            usuarioOrigem.Amigos = amigos;
            context.Update(usuarioOrigem);

            amigos = usuarioDestino.Amigos != null ? usuarioDestino.Amigos : new List<Usuario>();

            amigos.Add(usuarioOrigem);
            usuarioDestino.Amigos = amigos;
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

        public async Task AbrirChatPrivado(string origem, string destino)
        {
            var usuarioOrigem = await userManager.FindByIdAsync(origem);
            var usuarioDestino = await userManager.FindByIdAsync(destino);


            var mensagensEnviadas = context.Mensagem.Where(m => m.UsuarioID == usuarioOrigem.Id && m.AlvoId == usuarioDestino.Id).ToList();
            var mensagensRecebidas = context.Mensagem.Where(m => m.UsuarioID == usuarioDestino.Id && m.AlvoId == usuarioOrigem.Id).ToList();
            var mensagens = mensagensEnviadas.Concat(mensagensRecebidas).ToList();

            await Clients.User(origem).SendAsync("CarregarMensagens", mensagens, usuarioDestino.UserName);
        }
    }
}
