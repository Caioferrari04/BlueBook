using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlueBook.Models
{
    public class Mensagem
    {
        public int ID { get; set; }

        public string NomeUsuario { get; set; }

        public string AlvoNome { get; set; }

        [Range(1,300,ErrorMessage = "Mensagem deve ter no minimo 1 caracter e no máximo 300!")]
        public string Texto { get; set; }

        public DateTime DataEnvio { get; set; }

        public string UsuarioID { get; set; }

        public Usuario usuario { get; set; }

        public string AlvoId { get; set; }

        public Usuario Alvo { get; set; }
    }
}
