using BlueBook.Data;
using BlueBook.Hubs;
using BlueBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlueBook.Controllers
{
    [Authorize]
    public class FeedController : Controller
    {
        public readonly UserManager<Usuario> userManager;
        public readonly BookContext context;
        public FeedController(UserManager<Usuario> userManager, BookContext context)
        {
            this.userManager = userManager;
            this.context = context;
        }
        public async Task<IActionResult> Index()
        {
            var usuarioAtual = await userManager.GetUserAsync(User);
            List<IdentityUser> usuarios = context.Users.Where(u => u.UserName != usuarioAtual.UserName).ToList();
            List<Usuario> amigos = new List<Usuario>();

            if(usuarioAtual.Amigos != null) 
                foreach(Usuario amigo in usuarioAtual.Amigos) amigos.Add(amigo);

            ViewBag.NomeUsuarioLogado = usuarioAtual.UserName;
            ViewBag.LinkImagem = usuarioAtual.LinkImagem == null ? "https://freepikpsd.com/media/2019/10/default-user-profile-image-png-6-Transparent-Images.png" : usuarioAtual.LinkImagem;
            ViewBag.Mensagens = context.Mensagem.ToList();
            ViewBag.Postagens = context.Postagem.ToList();
            ViewBag.Amigos = amigos;
            ViewBag.linkPerfil = usuarioAtual.Id;
            return View(usuarios);
        }

        public async Task<IActionResult> EnviarMensagem(string Texto,[FromServices] IHubContext<FeedHub> chat)
        {
            var usuario = await userManager.GetUserAsync(User);
            Mensagem retorno = new Mensagem
            {
                Texto = Texto,
                NomeUsuario = User.Identity.Name,
                UsuarioID = usuario.Id,
                DataEnvio = DateTime.Now
            };

            context.Mensagem.Add(retorno);
            context.SaveChanges();

            await chat.Clients.All.SendAsync("ReceberMensagem", retorno);
            //return RedirectToAction(nameof(Index));
            return Ok();
        }
    }
}
