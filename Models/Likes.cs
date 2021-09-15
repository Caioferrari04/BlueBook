using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlueBook.Models
{
    public class Likes
    {
        public string UsuarioId { get; set; }

        public Usuario LikeDeUsuario { get; set; }

        public int PostagemId { get; set; }

        public Postagem PostagemLikada { get; set; }

        public string TipoLike { get; set; }
    }

    public class LikesComentarios
    {
        public string UsuarioId { get; set; }

        public Usuario LikeDeUsuario { get; set; }

        public int ComentarioId { get; set; }

        public Comentario ComentarioCurtido { get; set; }

        public string TipoLike { get; set; }
    }
}
