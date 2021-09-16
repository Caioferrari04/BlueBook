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
            ViewBag.Mensagens = context.Mensagem.Include(u => u.usuario).ToList();
            ViewBag.Postagens = context.Postagem.ToList();
            ViewBag.Comentarios = context.Comentario.ToList();
            ViewBag.linkPerfil = usuarioAtual.Id;
            ViewBag.Likes = context.Likes.ToList();
            ViewBag.LikesComentarios = context.LikesComentarios.ToList();
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
            string idUsuario = userManager.GetUserId(User);
            Postagem postLike = context.Postagem.FirstOrDefault(p => p.ID == int.Parse(LikeInfo[1]));
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LikeComment(string LikeBtn)
        {
            if (!LikeBtn.Contains("Like") && !LikeBtn.Contains("Deslike")) return RedirectToAction(nameof(Index));

            string[] LikeInfo = LikeBtn.Split(":");
            string idUsuario = userManager.GetUserId(User);
            Comentario comentarioLike = context.Comentario.FirstOrDefault(p => p.Id == int.Parse(LikeInfo[1]));
            LikesComentarios like = context.LikesComentarios.FirstOrDefault(u => u.UsuarioId == idUsuario && u.ComentarioId == comentarioLike.Id);
            if (like == null)
            {
                like = new LikesComentarios()
                {
                    UsuarioId = idUsuario,
                    ComentarioId = comentarioLike.Id,
                    TipoLike = LikeInfo[0]
                };
                comentarioLike.QuantidadeLikes = like.TipoLike == "Like" ? comentarioLike.QuantidadeLikes + 1 : comentarioLike.QuantidadeLikes - 1;
                context.LikesComentarios.Add(like);
            }
            else
            {
                comentarioLike.QuantidadeLikes = like.TipoLike == "Like" ? comentarioLike.QuantidadeLikes - 1 : comentarioLike.QuantidadeLikes + 1;
                context.LikesComentarios.Remove(like);
            }
            context.Comentario.Update(comentarioLike);
            context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Comentar(string TxtComment, string ImgComment, int PostId)
        {
            if (TxtComment.Length > 100) return RedirectToAction(nameof(Index));

            Postagem postagemOriginal = context.Postagem.FirstOrDefault(p => p.ID == PostId);

            if (postagemOriginal == null) return RedirectToAction(nameof(Index));

            Comentario comentarioNovo = new Comentario
            {
                Texto = TxtComment,
                ImagemUrl = ImgComment,
                DataComentado = DateTime.Now,
                UsuarioIdComentario = userManager.GetUserId(User),
                PostagemId = postagemOriginal.ID,
                QuantidadeLikes = 0
            };
            context.Comentario.Add(comentarioNovo);
            context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


    }
}
