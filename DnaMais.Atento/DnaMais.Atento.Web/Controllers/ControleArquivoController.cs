using DnaMais.Atento.Web.Models;
using DnaMais.Atento.Web.Repositories;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DnaMais.Atento.Web.Controllers
{
    //[Authorize]
    public class ControleArquivoController : Controller
    {
        private readonly ControleArquivoRepository _controleArquivoRepository;
        private readonly LayoutEntradaRepository _layoutEntradaRepository;
        private readonly LayoutSaidaRepository _layoutSaidaRepository;

        #region Listar Arquivo

        public ActionResult ListarArquivo()
        {
            var models = _controleArquivoRepository.GetAll(Session["LoginUsuario"].ToString());

            return View(models);

        }

        #endregion

        #region Construtor
        public ControleArquivoController()
        {
            _controleArquivoRepository = new ControleArquivoRepository();
            _layoutEntradaRepository = new LayoutEntradaRepository();
            _layoutSaidaRepository = new LayoutSaidaRepository();
        }
        #endregion

        #region Action Create / View Create
        //
        // GET: /ControleArquivo/Create
        public ActionResult Create()
        {
            ViewData["LayoutsEntrada"] = _layoutEntradaRepository.GetAll().ToList().ConvertAll(x => new SelectListItem { Value = x.Codigo.ToString(""), Text = x.Nome });
            ViewData["LayoutsSaida"] = _layoutSaidaRepository.GetAll().ToList().ConvertAll(x => new SelectListItem { Value = x.Codigo.ToString(""), Text = x.Nome });

            return View();
        }
        #endregion

        #region Create / View Create [HttpPost]
        //
        // POST: /ControleArquivo/Create
        [HttpPost]
        public ActionResult Create(HttpPostedFileBase file)
        {

            ControleArquivoModel model = new ControleArquivoModel();

            string nomeArquivoCalculado = file.FileName.Substring(0, file.FileName.LastIndexOf(".")) + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + file.FileName.Substring(file.FileName.LastIndexOf("."));

            model.CodigoLayoutEntrada = Convert.ToInt16(Request["CodigoLayoutEntrada"]);
            model.CodigoLayoutSaida = Convert.ToInt16(Request["CodigoLayoutSaida"]);
            model.NomeArquivoEntrada = nomeArquivoCalculado;
            model.Arquivo = file;
            model.LoginSolicitante = Session["LoginUsuario"].ToString();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {

                if (file != null && file.ContentLength > 0)
                {
                    // extract only the filename
                    var fileName = Path.GetFileName(file.FileName);
                    // store the file inside ~/App_Data/uploads folder
                    var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);

                }

                var request = CreateFtpRequest(nomeArquivoCalculado);

                using (var arquivo = request.GetRequestStream())
                    file.InputStream.CopyTo(arquivo);

                var result = (FtpWebResponse)request.GetResponse();

                if (result.StatusCode == FtpStatusCode.CommandOK || result.StatusCode == FtpStatusCode.ClosingData || result.StatusCode == FtpStatusCode.FileActionOK)
                {
                    _controleArquivoRepository.Add(model);

                    TempData["notice"] = "Envio Efetuado com Sucesso";

                    return RedirectToAction("ListarArquivo", "ControleArquivo");
                }
                else
                {
                    return RedirectToAction("Create");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        #endregion

        #region Upload Arquivo Ftp
        private static FtpWebRequest CreateFtpRequest(string fileName)
        {
            var caminho = ConfigurationManager.AppSettings["FtpUrl"];
            var uri = new Uri(string.Format(@"{0}{1}", caminho, fileName));
            var ftp = (FtpWebRequest)WebRequest.Create(uri);
            ftp.Method = System.Net.WebRequestMethods.Ftp.UploadFile;

            ftp.Credentials = CreateCredential();
            return ftp;
        }
        #endregion

        #region Credential Ftp
        private static NetworkCredential CreateCredential()
        {
            var username = ConfigurationManager.AppSettings["FtpUsername"];
            var password = ConfigurationManager.AppSettings["FtpPassword"];
            return new NetworkCredential(username, password);
        }
        #endregion

        #region Validar Campos DropDownList
        public Boolean ValidarCampos()
        {
            if (Request["CodigoLayoutEntrada"].ToString() == "")
            {
                return false;
            }
            if (Request["CodigoLayoutSaida"].ToString() == "")
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Download Arquivo Ftp
        public void DownloadArquivo(int itemCodigo)
        {

            string nomeArquivoDownload = _controleArquivoRepository.GetNomeDownloadArquivoById(itemCodigo);

            string FTPSaida = ConfigurationManager.AppSettings["FtpSaida"].ToString() + nomeArquivoDownload;

            try
            {
                //Create FTP Request.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(FTPSaida);
                request.Method = WebRequestMethods.Ftp.DownloadFile;

                //Enter FTP Server credentials.
                request.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["ftpUsername"], ConfigurationManager.AppSettings["ftpPassword"]);
                request.UsePassive = true;
                request.UseBinary = true;
                request.EnableSsl = false;

                //Fetch the Response and read it into a MemoryStream object.
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                using (MemoryStream stream = new MemoryStream())
                {
                    //Download the File.
                    response.GetResponseStream().CopyTo(stream);
                    Response.AddHeader("content-disposition", "attachment;filename=" + nomeArquivoDownload);
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.BinaryWrite(stream.ToArray());
                    Response.End();

                    //ACOES POS-DOWNLOAD

                }
            }
            catch (WebException ex)
            {
                throw new Exception((ex.Response as FtpWebResponse).StatusDescription);
            }
        }
        #endregion

        #region Download Arquivo Ftp [HttpPost]
        [HttpPost]
        public ActionResult DownloadArquivo()
        {
            return View();
        }
        #endregion

        #region Listar Arquivos Ftp Entrada

        public ActionResult ListarFtpEntrada()
        {
            var caminho = ConfigurationManager.AppSettings["FtpUrl"];
            var uri = new Uri(caminho);
            var ftp = (FtpWebRequest)WebRequest.Create(uri);
            ftp.Method = System.Net.WebRequestMethods.Ftp.ListDirectory;

            ftp.Credentials = CreateCredential();

            FtpWebResponse response = (FtpWebResponse)ftp.GetResponse();

            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);

            List<string> listaArquivoEntradaFtp = new List<string>();

            string[] lines = reader.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string item in lines)
            {
                listaArquivoEntradaFtp.Add(item);
            }


            ViewBag.ListaArquivoEntradaFtp = listaArquivoEntradaFtp;

            return View(ViewBag.ListaArquivoEntradaFtp);
        }

        #endregion

        #region Listar Arquivos Ftp Entrada [HttpPost]
        [HttpPost]
        public ActionResult ListarFtpEntrada(HttpPostedFileBase file)
        {
            ControleArquivoModel model = new ControleArquivoModel();

            model.NomeArquivoEntrada = file.FileName;
            model.Arquivo = file;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    // extract only the filename
                    var fileName = Path.GetFileName(file.FileName);
                    // store the file inside ~/App_Data/uploads folder
                    var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);

                }

                var request = CreateFtpRequest(file.FileName);

                using (var arquivo = request.GetRequestStream())
                    file.InputStream.CopyTo(arquivo);

                var result = (FtpWebResponse)request.GetResponse();

                if (result.StatusCode == FtpStatusCode.CommandOK || result.StatusCode == FtpStatusCode.ClosingData || result.StatusCode == FtpStatusCode.FileActionOK)
                {
                    ListarFtpEntrada();

                    TempData["notice"] = "Arquivo " + '"' + file.FileName + '"' + " Enviado com Sucesso!";

                    return View(ViewBag.ListaArquivoEntradaFtp);
                }
                else
                {
                    return RedirectToAction("ListarFtpEntrada");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Listar Arquivos Ftp Saída

        public ActionResult ListarFtpSaida()
        {
            var caminho = ConfigurationManager.AppSettings["FtpSaida"];
            var uri = new Uri(caminho);
            var ftp = (FtpWebRequest)WebRequest.Create(uri);
            ftp.Method = System.Net.WebRequestMethods.Ftp.ListDirectory;

            ftp.Credentials = CreateCredential();

            FtpWebResponse response = (FtpWebResponse)ftp.GetResponse();

            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);

            List<string> listaArquivoSaidaFtp = new List<string>();

            string[] lines = reader.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string item in lines)
            {
                listaArquivoSaidaFtp.Add(item);
            }

            ViewBag.ListaArquivoSaidaFtp = listaArquivoSaidaFtp;

            return View(ViewBag.ListaArquivoSaidaFtp);
        }

        #endregion

        #region Download Arquivos Ftp Saida

        public void DownloadFtpSaida(string nomeArquivoDownload)
        {
            string FTPSaida = ConfigurationManager.AppSettings["FtpSaida"];

            try
            {
                //Create FTP Request.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(FTPSaida + nomeArquivoDownload);
                request.Method = WebRequestMethods.Ftp.DownloadFile;

                //Enter FTP Server credentials.
                request.Credentials = CreateCredential();
                request.UsePassive = true;
                request.UseBinary = true;
                request.EnableSsl = false;

                //Fetch the Response and read it into a MemoryStream object.
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                using (MemoryStream stream = new MemoryStream())
                {
                    //Download the File.
                    response.GetResponseStream().CopyTo(stream);
                    Response.AddHeader("content-disposition", "attachment;filename=" + nomeArquivoDownload);
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.BinaryWrite(stream.ToArray());
                    Response.End();
                }
            }
            catch (WebException ex)
            {
                throw new Exception((ex.Response as FtpWebResponse).StatusDescription);
            }
        }

        #endregion

        #region Exibir Relatório de Processamento

        public ActionResult ExibirRelatorio(int itemCodigo)
        {
            var models = _controleArquivoRepository.GerarRelatorio(itemCodigo);

            return View(models);
        }

        #endregion
    }
}
