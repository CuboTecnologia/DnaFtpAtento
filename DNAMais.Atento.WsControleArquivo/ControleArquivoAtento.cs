using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNAMais.Atento.WsControleArquivo
{
    public class ControleArquivoAtento
    {
        public int? Id { get; set; }
        public DateTime? DataRegistro { get; set; }
        public LayoutEntradaAtento LayoutEntrada { get; set; }
        public LayoutSaidaAtento LayoutSaida { get; set; }
        public string NomeArquivoEntrada { get; set; }
        public string NomeArquivoUnzip { get; set; }
        public string NomeArquivoSaida { get; set; }
        public DateTime? DataInicioExecucao { get; set; }
        public DateTime? DataTerminoExecucao { get; set; }
        public int? QtdeItensRecebidos { get; set; }
        public int? QtdeItensExportados { get; set; }
        public StatusArquivoAtento Status { get; set; }

        public ControleArquivoAtento()
        {
            LayoutEntrada = new LayoutEntradaAtento();
            LayoutSaida = new LayoutSaidaAtento();
            Status = new StatusArquivoAtento();
        }
    }
}
