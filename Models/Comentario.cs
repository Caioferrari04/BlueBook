using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlueBook.Models
{
    public class Comentario
    {
        public int Id { get; set; }

        public string Texto { get; set; }

        public string ImagemUrl { get; set; }

        public DateTime DataComentado { get; set; }

        public string UsuarioIdComentario { get; set; }

        public Usuario UsuarioComentario { get; set; }

        public int PostagemId { get; set; }

        public Postagem PostagemOriginal { get; set; }
    }
}
