using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DnaMais.Atento.Web.Models
{
    public class StatusModel
    {
        [Key]
        public short Codigo { get; set; }
        [Required]
        public string DescricaoStatus { get; set; }
    }
}