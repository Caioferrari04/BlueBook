using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlueBook.Models
{
    public class Usuario : IdentityUser
    {
        public string LinkImagem { get; set; }

        public List<Mensagem> Mensagens { get; set; }

        public List<Postagem> Postagens { get; set; }

        public List<Comentario> Comentarios { get; set; }

        public List<Usuario> Amigos { get; set; }
    }
}
