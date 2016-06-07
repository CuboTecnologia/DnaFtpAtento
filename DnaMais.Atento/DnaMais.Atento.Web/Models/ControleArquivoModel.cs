using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DnaMais.Atento.Web.Models
{
    public sealed class ControleArquivoModel
    {
        public ControleArquivoModel()
        {
            DataRegistro = DateTime.Now;
        }

        [Key]
        public int Codigo { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime DataRegistro { get; set; }

        [Required]
        public int CodigoLayoutEntrada { get; set; }
        public LayoutEntradaModel LayoutEntrada { get; set; }

        [Required]
        public int CodigoLayoutSaida { get; set; }
        public LayoutSaidaModel LayoutSaida { get; set; }

        public HttpPostedFileBase Arquivo { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime? DataInicioExecucao { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime? DataTerminoExecucao { get; set; }

        private string _nomeArquivoEntrada;
        private string _nomeArquivoEntradaOriginal;
        public string NomeArquivoEntrada
        {
            get
            {
                return _nomeArquivoEntrada;
            }
            set
            {
                _nomeArquivoEntrada = value;
            }
        }

        public string NomeArquivoEntradaOriginal
        {
            get
            {
                return Arquivo == null ? _nomeArquivoEntradaOriginal : Arquivo.FileName;
            }
            set
            {
                _nomeArquivoEntradaOriginal = value;
            }
        }

        [Required]
        public int CodigoStatus { get; set; }
        public StatusModel Status { get; set; }

        public string NomeArquivoDownload { get; set; }

        public string LoginSolicitante { get; set; }
        public string NomeUsuarioSolicitante { get; set; }

        public int QtdeItensRecebidos { get; set; }

        public int QtdeItensProcessados { get; set; }
        public int QtdeEnriquecidoEndereco { get; set; }
        public int QtdeEnriquecidoFone { get; set; }
        public int QtdeEnriquecidoCelular { get; set; }
        public int QtdeEnriquecidoEmail { get; set; }
    }
}