using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DnaMais.Atento.Web.Models
{
    public class UsuarioModel
    {
        [Key]
        [MaxLength(20, ErrorMessage = "Máximo de 20 caracteres")]
        public string Login { get; set; }

        [Required]
        [MaxLength(20, ErrorMessage = "Máximo de 20 caracteres")]
        public string Senha { get; set; }

        [Required]
        [MaxLength(20,ErrorMessage="Máximo de 20 caracteres")]
        [Display(Name = "Confirmar Senha")]
        public string ConfirmarSenha { get; set; }

        [Required]
        [Display(Name = "Nome/Apelido")]
        [MaxLength(20, ErrorMessage = "Máximo de 20 caracteres")]
        public string Usuario { get; set; }

        [Required]
        [Display(Name = "E-Mail")]
        [MaxLength(20, ErrorMessage = "Máximo de 20 caracteres")]
        public string Email { get; set; }

        [Required]
        public string TipoUsuario { get; set; }


        public string DescricaoGrupo { get; set; }
        public GrupoUsuarioModel Grupos { get; set; }

    }
}