using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlueBook.Models
{
    public class Amizade
    {
        public string OrigemId { get; set; }

        public Usuario Origem { get; set; }

        public string AlvoId { get; set; }

        public Usuario Alvo { get; set; }
    }
}
