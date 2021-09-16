using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlueBook.Models
{
    public class Usuario : IdentityUser
    {
        [Required(ErrorMessage = "Nome de usuário não pode ser vazia!")]
        [RegularExpression("^([a-zA-Z0-9_])*$", ErrorMessage = "Nome deve ter apenas letras, números e underline!")]
        [Display(Name = "Nome de usuário")]
        public override string UserName { get => base.UserName; set => base.UserName = value; }

        [Display(Name = "Link da imagem de perfil")]
        [Url(ErrorMessage = "Url inválida!")]
        public string LinkImagem { get; set; }

        public List<Mensagem> Mensagens { get; set; }

        public List<Postagem> Postagens { get; set; }

        public List<Comentario> Comentarios { get; set; }

        public List<Amizade> Origem { get; set; }

        public List<Amizade> Alvo { get; set; }
    }
}
