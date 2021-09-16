using BlueBook.Data;
using BlueBook.Hubs;
using BlueBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlueBook.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        public readonly UserManager<Usuario> userManager;
        public readonly BookContext context;
        public UserController(UserManager<Usuario> userManager, BookContext context)
        {
            this.userManager = userManager;
            this.context = context;
        }
        public async Task<IActionResult> Index(string Id)
        {
            Usuario usuarioAtual = await userManager.GetUserAsync(User);
            Usuario usuario = await userManager.FindByIdAsync(Id);
            if(usuarioAtual.Id != usuario.Id) { 
                ViewBag.UsuarioAtual = usuarioAtual.Id;
                List<Amizade> amizades = context.Amizade.Where(a => a.AlvoId == usuarioAtual.Id || a.OrigemId == usuarioAtual.Id).ToList();
                foreach (Amizade amizade in amizades)
                    if (amizade.AlvoId == usuario.Id || amizade.OrigemId == usuario.Id)
                        ViewBag.AmigoOuNao = true;
                if (ViewBag.AmigoOuNao == null) ViewBag.AmigoOuNao = false;
                return usuario != null ? View(usuario) : RedirectToAction(nameof(NotFound));
            }
            RouteValues idPagina = new RouteValues() { Id = usuario.Id };
            return RedirectToAction(nameof(PaginaUsuario), idPagina);
        }

        public new IActionResult NotFound() => View();

        public async Task<IActionResult> RemoveFriend(string destino)
        {
            Usuario usuarioOrigem = await userManager.GetUserAsync(User);
            Usuario usuarioDestino = await userManager.FindByIdAsync(destino);
            Amizade amizade = context.Amizade.Find(usuarioOrigem.Id, usuarioDestino.Id);
            amizade = amizade == null ? context.Amizade.Find(usuarioDestino.Id, usuarioOrigem.Id) : amizade;
            if(amizade != null) { 
                context.Amizade.Remove(amizade);
                context.SaveChanges();
                return RedirectToAction("Index", "Feed", new { area = "" });
            }
            return RedirectToAction(nameof(NotFound));
        }

        public async Task<IActionResult> PaginaUsuario(string id)
        {
            Usuario usuarioAtual = await userManager.GetUserAsync(User);
            return id == usuarioAtual.Id ? View(usuarioAtual) : RedirectToAction(nameof(NotFound));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PaginaUsuario(Usuario usuarioAtualizado)
        {
            Usuario usuarioAtual = await userManager.GetUserAsync(User);

            if (!ModelState.IsValid) return View(usuarioAtualizado);

            usuarioAtual.UserName = usuarioAtualizado.UserName != null ? usuarioAtualizado.UserName.Trim() : usuarioAtual.UserName;
            usuarioAtual.NormalizedUserName = usuarioAtualizado.UserName != null ? usuarioAtualizado.UserName.Trim().ToUpper().Normalize() : usuarioAtual.UserName;
            usuarioAtual.LinkImagem = usuarioAtualizado.LinkImagem == null ? "https://freepikpsd.com/media/2019/10/default-user-profile-image-png-6-Transparent-Images.png" : usuarioAtualizado.LinkImagem;
            context.Users.Update(usuarioAtual);
            context.SaveChanges();
            /*Foi necessário utilizar um objeto aqui pois utilizar uma string levava o Redirect a identificar a string
             como um controller.*/
            RouteValues IdPagina = new RouteValues() { Id = usuarioAtual.Id };
            return RedirectToAction(nameof(PaginaUsuario), IdPagina);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PesquisarUsuario(string NomeUsuario)
        {
            Usuario usuario = await userManager.FindByNameAsync(NomeUsuario);
            RouteValues idPagina = new RouteValues() { Id = usuario.Id };
            return usuario != null ? RedirectToAction(nameof(Index), idPagina) : RedirectToAction(nameof(NotFound));
        }
    }

    public class RouteValues
    {
        public string Id { get; set; }
    }
}
