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
            List<Likes> likes = context.Likes.ToList();
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
            ViewBag.Likes = likes;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LikePost(string LikeBtn)
        {
            if (!LikeBtn.Contains("Like") && !LikeBtn.Contains("Deslike")) return RedirectToAction(nameof(Index));

            string[] LikeInfo = LikeBtn.Split(":");
            int ID = int.Parse(LikeInfo[1]);
            string idUsuario = userManager.GetUserId(User);
            Postagem postLike = context.Postagem.FirstOrDefault(p => p.ID == ID);
            Likes like = context.Likes.FirstOrDefault(u => u.UsuarioId == idUsuario && u.PostagemId == postLike.ID);
            if ( like == null) {
                like = new Likes()
                {
                    UsuarioId = idUsuario,
                    PostagemId = postLike.ID,
                    TipoLike = LikeInfo[0]
                };
                postLike.QuantidadeLikes = like.TipoLike == "Like" ? postLike.QuantidadeLikes + 1 : postLike.QuantidadeLikes - 1;
                context.Likes.Add(like);
            } 
            else {
                postLike.QuantidadeLikes = like.TipoLike == "Like" ? postLike.QuantidadeLikes - 1 : postLike.QuantidadeLikes + 1;
                context.Likes.Remove(like);
            }
            context.Postagem.Update(postLike);
            context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
