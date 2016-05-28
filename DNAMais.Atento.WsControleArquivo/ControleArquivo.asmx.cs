using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace DNAMais.Atento.WsControleArquivo
{
    /// <summary>
    /// Summary description for ControleArquivo
    /// </summary>
    [WebService(Namespace = "http://www.dnamais.com.br/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class ControleArquivo : System.Web.Services.WebService
    {

        [WebMethod]
        public List<ControleArquivoAtento> ListarArquivosPendentesProcessamento()
        {
            List<ControleArquivoAtento> lstRetorno = new List<ControleArquivoAtento>();

            ControleArquivoAtento objItem;

            using (ConexaoOracle db = new ConexaoOracle())
            {
                using (OracleDataReader leitor = db.ObterLeitor("SELECT * FROM CONTROLE_ARQ_ATENTO WHERE CD_STATUS_CONTROLE_ARQUIVO = 1"))
                {
                    while (leitor.Read())
                    {
                        objItem = new ControleArquivoAtento();

                        objItem.Id = (int)leitor["ID_CONTROLE"];
                        objItem.DataRegistro = (DateTime)leitor["DT_REGISTRO"];
                        objItem.LayoutEntrada.Id = (int)leitor["ID_LAYOUT_ENTRADA"];
                        objItem.LayoutSaida.Id = (int)leitor["ID_LAYOUT_SAIDA"];
                        objItem.NomeArquivoEntrada = leitor["NM_ARQUIVO_ENTRADA"].ToString();
                        objItem.NomeArquivoUnzip = leitor["NM_ARQUIVO_UNZIP"] == DBNull.Value ? null : leitor["NM_ARQUIVO_UNZIP"].ToString();
                        objItem.NomeArquivoSaida = leitor["NM_ARQUIVO_SAIDA"] == DBNull.Value ? null : leitor["NM_ARQUIVO_SAIDA"].ToString();
                        objItem.Status.Id = (short)leitor["CD_STATUS_CONTROLE_ARQUIVO"];

                        objItem.DataInicioExecucao = leitor["DT_INICIO_EXECUCAO"] == DBNull.Value ? null : (DateTime?)leitor["DT_INICIO_EXECUCAO"];
                        objItem.DataTerminoExecucao = leitor["DT_TERMINO_EXECUCAO"] == DBNull.Value ? null : (DateTime?)leitor["DT_TERMINO_EXECUCAO"];
                        objItem.QtdeItensRecebidos = leitor["QT_ITENS_RECEBIDOS"] == DBNull.Value ? null : (int?)leitor["QT_ITENS_RECEBIDOS"];
                        objItem.QtdeItensExportados = leitor["QT_ITENS_EXPORTADOS"] == DBNull.Value ? null : (int?)leitor["QT_ITENS_EXPORTADOS"];

                        lstRetorno.Add(objItem);
                    }
                }
            }

            return lstRetorno;
        }

        [WebMethod]
        public List<ControleArquivoAtento> ListarArquivosPendentesImportacao()
        {
            List<ControleArquivoAtento> lstRetorno = new List<ControleArquivoAtento>();

            ControleArquivoAtento objItem;

            using (ConexaoOracle db = new ConexaoOracle())
            {
                using (OracleDataReader leitor = db.ObterLeitor("SELECT * FROM CONTROLE_ARQ_ATENTO WHERE CD_STATUS_CONTROLE_ARQUIVO = 2"))
                {
                    while (leitor.Read())
                    {
                        objItem = new ControleArquivoAtento();

                        objItem.Id = (int)leitor["ID_CONTROLE"];
                        objItem.DataRegistro = (DateTime)leitor["DT_REGISTRO"];
                        objItem.LayoutEntrada.Id = (int)leitor["ID_LAYOUT_ENTRADA"];
                        objItem.LayoutSaida.Id = (int)leitor["ID_LAYOUT_SAIDA"];
                        objItem.NomeArquivoEntrada = leitor["NM_ARQUIVO_ENTRADA"].ToString();
                        objItem.NomeArquivoUnzip = leitor["NM_ARQUIVO_UNZIP"] == DBNull.Value ? null : leitor["NM_ARQUIVO_UNZIP"].ToString();
                        objItem.NomeArquivoSaida = leitor["NM_ARQUIVO_SAIDA"] == DBNull.Value ? null : leitor["NM_ARQUIVO_SAIDA"].ToString();
                        objItem.Status.Id = (short)leitor["CD_STATUS_CONTROLE_ARQUIVO"];

                        objItem.DataInicioExecucao = leitor["DT_INICIO_EXECUCAO"] == DBNull.Value ? null : (DateTime?)leitor["DT_INICIO_EXECUCAO"];
                        objItem.DataTerminoExecucao = leitor["DT_TERMINO_EXECUCAO"] == DBNull.Value ? null : (DateTime?)leitor["DT_TERMINO_EXECUCAO"];
                        objItem.QtdeItensRecebidos = leitor["QT_ITENS_RECEBIDOS"] == DBNull.Value ? null : (int?)leitor["QT_ITENS_RECEBIDOS"];
                        objItem.QtdeItensExportados = leitor["QT_ITENS_EXPORTADOS"] == DBNull.Value ? null : (int?)leitor["QT_ITENS_EXPORTADOS"];

                        lstRetorno.Add(objItem);
                    }
                }
            }

            return lstRetorno;
        }

        [WebMethod]
        public List<ControleArquivoAtento> ListarArquivosPendentesExtracao()
        {
            List<ControleArquivoAtento> lstRetorno = new List<ControleArquivoAtento>();

            ControleArquivoAtento objItem;

            using (ConexaoOracle db = new ConexaoOracle())
            {
                using (OracleDataReader leitor = db.ObterLeitor("SELECT * FROM CONTROLE_ARQ_ATENTO WHERE CD_STATUS_CONTROLE_ARQUIVO = 3"))
                {
                    while (leitor.Read())
                    {
                        objItem = new ControleArquivoAtento();

                        objItem.Id = (int)leitor["ID_CONTROLE"];
                        objItem.DataRegistro = (DateTime)leitor["DT_REGISTRO"];
                        objItem.LayoutEntrada.Id = (int)leitor["ID_LAYOUT_ENTRADA"];
                        objItem.LayoutSaida.Id = (int)leitor["ID_LAYOUT_SAIDA"];
                        objItem.NomeArquivoEntrada = leitor["NM_ARQUIVO_ENTRADA"].ToString();
                        objItem.NomeArquivoUnzip = leitor["NM_ARQUIVO_UNZIP"] == DBNull.Value ? null : leitor["NM_ARQUIVO_UNZIP"].ToString();
                        objItem.NomeArquivoSaida = leitor["NM_ARQUIVO_SAIDA"] == DBNull.Value ? null : leitor["NM_ARQUIVO_SAIDA"].ToString();
                        objItem.Status.Id = (short)leitor["CD_STATUS_CONTROLE_ARQUIVO"];

                        objItem.DataInicioExecucao = leitor["DT_INICIO_EXECUCAO"] == DBNull.Value ? null : (DateTime?)leitor["DT_INICIO_EXECUCAO"];
                        objItem.DataTerminoExecucao = leitor["DT_TERMINO_EXECUCAO"] == DBNull.Value ? null : (DateTime?)leitor["DT_TERMINO_EXECUCAO"];
                        objItem.QtdeItensRecebidos = leitor["QT_ITENS_RECEBIDOS"] == DBNull.Value ? null : (int?)leitor["QT_ITENS_RECEBIDOS"];
                        objItem.QtdeItensExportados = leitor["QT_ITENS_EXPORTADOS"] == DBNull.Value ? null : (int?)leitor["QT_ITENS_EXPORTADOS"];

                        lstRetorno.Add(objItem);
                    }
                }
            }

            return lstRetorno;
        }

        [WebMethod]
        public List<ControleArquivoAtento> ListarArquivosPendentesExportacao()
        {
            List<ControleArquivoAtento> lstRetorno = new List<ControleArquivoAtento>();

            ControleArquivoAtento objItem;

            using (ConexaoOracle db = new ConexaoOracle())
            {
                using (OracleDataReader leitor = db.ObterLeitor("SELECT * FROM CONTROLE_ARQ_ATENTO WHERE CD_STATUS_CONTROLE_ARQUIVO = 4"))
                {
                    while (leitor.Read())
                    {
                        objItem = new ControleArquivoAtento();

                        objItem.Id = (int)leitor["ID_CONTROLE"];
                        objItem.DataRegistro = (DateTime)leitor["DT_REGISTRO"];
                        objItem.LayoutEntrada.Id = (int)leitor["ID_LAYOUT_ENTRADA"];
                        objItem.LayoutSaida.Id = (int)leitor["ID_LAYOUT_SAIDA"];
                        objItem.NomeArquivoEntrada = leitor["NM_ARQUIVO_ENTRADA"].ToString();
                        objItem.NomeArquivoUnzip = leitor["NM_ARQUIVO_UNZIP"] == DBNull.Value ? null : leitor["NM_ARQUIVO_UNZIP"].ToString();
                        objItem.NomeArquivoSaida = leitor["NM_ARQUIVO_SAIDA"] == DBNull.Value ? null : leitor["NM_ARQUIVO_SAIDA"].ToString();
                        objItem.Status.Id = (short)leitor["CD_STATUS_CONTROLE_ARQUIVO"];

                        objItem.DataInicioExecucao = leitor["DT_INICIO_EXECUCAO"] == DBNull.Value ? null : (DateTime?)leitor["DT_INICIO_EXECUCAO"];
                        objItem.DataTerminoExecucao = leitor["DT_TERMINO_EXECUCAO"] == DBNull.Value ? null : (DateTime?)leitor["DT_TERMINO_EXECUCAO"];
                        objItem.QtdeItensRecebidos = leitor["QT_ITENS_RECEBIDOS"] == DBNull.Value ? null : (int?)leitor["QT_ITENS_RECEBIDOS"];
                        objItem.QtdeItensExportados = leitor["QT_ITENS_EXPORTADOS"] == DBNull.Value ? null : (int?)leitor["QT_ITENS_EXPORTADOS"];

                        lstRetorno.Add(objItem);
                    }
                }
            }

            return lstRetorno;
        }

        [WebMethod]
        public List<ControleArquivoAtento> ListarArquivosPendentesUpload()
        {
            List<ControleArquivoAtento> lstRetorno = new List<ControleArquivoAtento>();

            ControleArquivoAtento objItem;

            using (ConexaoOracle db = new ConexaoOracle())
            {
                using (OracleDataReader leitor = db.ObterLeitor("SELECT * FROM CONTROLE_ARQ_ATENTO WHERE CD_STATUS_CONTROLE_ARQUIVO = 5"))
                {
                    while (leitor.Read())
                    {
                        objItem = new ControleArquivoAtento();

                        objItem.Id = (int)leitor["ID_CONTROLE"];
                        objItem.DataRegistro = (DateTime)leitor["DT_REGISTRO"];
                        objItem.LayoutEntrada.Id = (int)leitor["ID_LAYOUT_ENTRADA"];
                        objItem.LayoutSaida.Id = (int)leitor["ID_LAYOUT_SAIDA"];
                        objItem.NomeArquivoEntrada = leitor["NM_ARQUIVO_ENTRADA"].ToString();
                        objItem.NomeArquivoUnzip = leitor["NM_ARQUIVO_UNZIP"] == DBNull.Value ? null : leitor["NM_ARQUIVO_UNZIP"].ToString();
                        objItem.NomeArquivoSaida = leitor["NM_ARQUIVO_SAIDA"] == DBNull.Value ? null : leitor["NM_ARQUIVO_SAIDA"].ToString();
                        objItem.Status.Id = (short)leitor["CD_STATUS_CONTROLE_ARQUIVO"];

                        objItem.DataInicioExecucao = leitor["DT_INICIO_EXECUCAO"] == DBNull.Value ? null : (DateTime?)leitor["DT_INICIO_EXECUCAO"];
                        objItem.DataTerminoExecucao = leitor["DT_TERMINO_EXECUCAO"] == DBNull.Value ? null : (DateTime?)leitor["DT_TERMINO_EXECUCAO"];
                        objItem.QtdeItensRecebidos = leitor["QT_ITENS_RECEBIDOS"] == DBNull.Value ? null : (int?)leitor["QT_ITENS_RECEBIDOS"];
                        objItem.QtdeItensExportados = leitor["QT_ITENS_EXPORTADOS"] == DBNull.Value ? null : (int?)leitor["QT_ITENS_EXPORTADOS"];

                        lstRetorno.Add(objItem);
                    }
                }
            }

            return lstRetorno;
        }

        [WebMethod]
        public void AtualizarStatusDownload(int idCotroleArquivo, string nomeArquivoDescompactado, string nomeArquivoSaida)
        {
            using (ConexaoOracle db = new ConexaoOracle("PKG_CONTROLE_ARQUIVO_ATENTO.ATUALIZAR_STATUS_DOWNLOAD"))
            {
                db.AddParametroIN("P_ID_CONTROLE", idCotroleArquivo);
                db.AddParametroIN("P_NM_ARQUIVO_UNZIP", nomeArquivoDescompactado);
                db.AddParametroIN("P_NM_ARQUIVO_SAIDA", nomeArquivoSaida);

                string mensagem;
                db.ExecutarDML(out mensagem);
            }
        }

        [WebMethod]
        public void AtualizarStatusImportacao(int idCotroleArquivo)
        {
            using (ConexaoOracle db = new ConexaoOracle("PKG_CONTROLE_ARQUIVO_ATENTO.ATUALIZAR_STATUS_IMPORTACAO"))
            {
                db.AddParametroIN("P_ID_CONTROLE", idCotroleArquivo);

                string mensagem;
                db.ExecutarDML(out mensagem);
            }
        }

        [WebMethod]
        public void AtualizarStatusExtracao(int idCotroleArquivo)
        {
            using (ConexaoOracle db = new ConexaoOracle("PKG_CONTROLE_ARQUIVO_ATENTO.ATUALIZAR_STATUS_EXTRACAO"))
            {
                db.AddParametroIN("P_ID_CONTROLE", idCotroleArquivo);

                string mensagem;
                db.ExecutarDML(out mensagem);
            }
        }

        [WebMethod]
        public void AtualizarStatusExportacao(int idCotroleArquivo)
        {
            using (ConexaoOracle db = new ConexaoOracle("PKG_CONTROLE_ARQUIVO_ATENTO.ATUALIZAR_STATUS_EXPORTACAO"))
            {
                db.AddParametroIN("P_ID_CONTROLE", idCotroleArquivo);

                string mensagem;
                db.ExecutarDML(out mensagem);
            }
        }

        [WebMethod]
        public void AtualizarStatusUpload(int idCotroleArquivo)
        {
            using (ConexaoOracle db = new ConexaoOracle("PKG_CONTROLE_ARQUIVO_ATENTO.ATUALIZAR_STATUS_UPLOAD"))
            {
                db.AddParametroIN("P_ID_CONTROLE", idCotroleArquivo);

                string mensagem;
                db.ExecutarDML(out mensagem);
            }
        }
    }
}
