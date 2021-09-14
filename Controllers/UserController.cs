using BlueBook.Data;
using BlueBook.Hubs;
using BlueBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
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
            var usuarioAtual = await userManager.FindByNameAsync(User.Identity.Name);
            ViewBag.UsuarioAtual = usuarioAtual.Id;
            var usuario = await userManager.FindByIdAsync(Id);
            return usuario != null ? View(usuario) : RedirectToAction(nameof(NotFound));
        }

        public new IActionResult NotFound()
        {
            return View();
        }

        [Route("/{origem}/{destino}")]
        public async Task<IActionResult> RemoveFriend(string origem, string destino)
        {
            Usuario usuarioOrigem = await userManager.FindByIdAsync(origem);
            Usuario usuarioDestino = await userManager.FindByIdAsync(destino);

            var amizadeOrigem = usuarioOrigem.Origem.Find(u => u.AlvoId == usuarioDestino.Id && u.OrigemId == usuarioOrigem.Id);
            var amizadeAlvo = usuarioDestino.Origem.Find(u => u.AlvoId == usuarioOrigem.Id && u.OrigemId == usuarioDestino.Id);
            usuarioDestino.Origem.Remove(amizadeAlvo);
            usuarioOrigem.Origem.Remove(amizadeOrigem);
            context.SaveChanges();
            return RedirectToAction("Index", "Feed", new { area = "" });
        }
    }
}
