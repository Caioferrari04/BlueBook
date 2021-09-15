using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlueBook.Models
{
    public class ComentarioDeComentario
    {
        public int ComentarioFonteId { get; set; }

        public Comentario ComentarioFonte { get; set; }

        public int ComentarioFilhoId { get; set; }

        public Comentario ComentarioFilho { get; set; }
    }
}
