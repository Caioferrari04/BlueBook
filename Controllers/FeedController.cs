using BlueBook.Data;
using BlueBook.Hubs;
using BlueBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
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
            List<Usuario> usuarios = await userManager.Users.Include(u => u.Mensagens).ToListAsync();
            List<Amizade> amizades = context.Amizade.Where(a => a.AlvoId == usuarioAtual.Id || a.OrigemId == usuarioAtual.Id).ToList();
            List<IdentityUser> amigos = new List<IdentityUser>();
            List<Mensagem> mensagens = context.Mensagem.Include(u => u.usuario).ToList();
            if (usuarioAtual.LinkImagem == null)
            {
                usuarioAtual.LinkImagem = "https://freepikpsd.com/media/2019/10/default-user-profile-image-png-6-Transparent-Images.png";
                context.Users.Update(usuarioAtual);
                context.SaveChanges();
            }
            foreach (Amizade amigo in amizades)
                if (amigo.AlvoId == usuarioAtual.Id) { 
                    amigos.Add(amigo.Alvo);
                }
                else if (amigo.OrigemId == usuarioAtual.Id) { 
                    amigos.Add(amigo.Origem);
                }
            ViewBag.LinkImagem = usuarioAtual.LinkImagem;
            ViewBag.nome = usuarioAtual.UserName;
            ViewBag.Mensagens = mensagens;
            ViewBag.Postagens = context.Postagem.ToList();
            ViewBag.linkPerfil = usuarioAtual.Id;
            return View(amigos);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Postar(string TxtPost, string ImgPost = null)
        {
            if (TxtPost == null) return RedirectToAction(nameof(Index));
            Usuario poster = await userManager.GetUserAsync(User);
            Postagem postagem = new Postagem()
            {
                Texto = TxtPost,
                UrlImagem = ImgPost,
                UsuarioIdPost = poster.Id,
                UsuarioPost = poster,
                DataPostagem = DateTime.Now
            };

            context.Postagem.Add(postagem);
            context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
