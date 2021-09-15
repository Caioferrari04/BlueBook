using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlueBook.Models
{
    public class Postagem
    {
        public int ID { get; set; }

        [Range(1, 300, ErrorMessage = "Mensagem deve ter no minimo 1 caracter e no máximo 300!")]
        public string Texto { get; set; }

        public int Likes { get; set; }

        public DateTime DataPostagem { get; set; }

        public string UrlImagem { get; set; }

        public string UsuarioIdPost { get; set; }

        public Usuario UsuarioPost { get; set; }

        public List<Comentario> Comentarios { get; set; }
    }
}
