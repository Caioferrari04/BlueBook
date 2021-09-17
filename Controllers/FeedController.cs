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
            List<IdentityUser> amigos = new List<IdentityUser>();
            List<Comentario> comentarios = new List<Comentario>();
            List<Postagem> postagensProcessadas = new List<Postagem>();
            List<Amizade> amizades = context.Amizade.Include(u => u.Alvo)
                    .ThenInclude(u => u.Postagens)
                    .ThenInclude(p => p.Comentarios)
                    .Include(o => o.Origem)
                    .ThenInclude(o => o.Postagens)
                    .ThenInclude(p => p.Comentarios)
                    .Where(a => a.AlvoId == usuarioAtual.Id || a.OrigemId == usuarioAtual.Id)
                    .ToList();
            if (amizades != null) {
                List<Postagem> postagens = new List<Postagem>();
                bool postagensUsuarioJaArmazenadas = false;
                foreach (Amizade amigo in amizades) { 
                    if (amigo.Alvo.Id == usuarioAtual.Id) {
                        amigos.Add(amigo.Origem); //Para carregar as informações dos amigos sob demanda, evitando enviar informações confidenciais
                        postagens.AddRange(amigo.Origem.Postagens);
                        if (postagensUsuarioJaArmazenadas == false)
                        {
                            postagens.AddRange(amigo.Alvo.Postagens);
                            postagensUsuarioJaArmazenadas = true;
                        }
                    }
                    else if (amigo.Origem.Id == usuarioAtual.Id) { 
                        amigos.Add(amigo.Alvo);
                        postagens.AddRange(amigo.Alvo.Postagens);
                        if (postagensUsuarioJaArmazenadas == false)
                        {
                            postagens.AddRange(amigo.Origem.Postagens);
                            postagensUsuarioJaArmazenadas = true;
                        }
                    }
                }
                foreach (Postagem postagem in postagens)
                {
                    Postagem postagemProcessada = new Postagem()
                    {
                        ID = postagem.ID,
                        UsuarioIdPost = postagem.UsuarioIdPost,
                        UsuarioPost = new Usuario
                        {
                            UserName = postagem.UsuarioPost.UserName,
                            LinkImagem = postagem.UsuarioPost.LinkImagem
                        },
                        DataPostagem = postagem.DataPostagem,
                        QuantidadeLikes = postagem.QuantidadeLikes,
                        Texto = postagem.Texto,
                        UrlImagem = postagem.UrlImagem,
                        Comentarios = postagem.Comentarios
                    };
                    comentarios.AddRange(postagemProcessada.Comentarios);
                    postagensProcessadas.Add(postagemProcessada);
                }
            }
            else {
                postagensProcessadas = context.Postagem.Include(c => c.Comentarios).Where(u => u.UsuarioIdPost == usuarioAtual.Id).ToList();
                foreach (Postagem postagem in postagensProcessadas)
                    comentarios.AddRange(postagem.Comentarios);
            }
            ViewBag.LinkImagem = usuarioAtual.LinkImagem;
            ViewBag.nome = usuarioAtual.UserName;
            ViewBag.Postagens = postagensProcessadas;
            ViewBag.linkPerfil = usuarioAtual.Id;
            ViewBag.Mensagens = context.Mensagem.Include(u => u.usuario).ToList();
            ViewBag.Likes = context.Likes.Include(l => l.PostagemLikada).Where(l => postagensProcessadas.Contains(l.PostagemLikada)).ToList();
            ViewBag.LikesComentarios = context.LikesComentarios.Include(lc => lc.ComentarioCurtido).Where(lc => comentarios.Contains(lc.ComentarioCurtido)).ToList();
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
