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

        [Range(3, 100, ErrorMessage = "Comentário deve ter no minimo 1 caracter e no máximo 100!")]
        public string Texto { get; set; }

        public string ImagemUrl { get; set; }

        public int QuantidadeLikes { get; set; }

        public DateTime DataComentado { get; set; }

        public string UsuarioIdComentario { get; set; }

        public Usuario UsuarioComentario { get; set; }

        public int PostagemId { get; set; }

        public Postagem PostagemOriginal { get; set; }
    }
}
