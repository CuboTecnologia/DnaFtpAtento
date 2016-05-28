using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DnaMais.Atento.Web.Models
{
    public sealed class LayoutSaidaModel
    {
        [Key]
        public int Codigo { get; set; }
        [Required]
        public string Nome { get; set; }
    }
}