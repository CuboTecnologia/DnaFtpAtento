using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DnaMais.Atento.Web.Models;
using System.Web.Security;
using DnaMais.Atento.Web.Repositories;


namespace DnaMais.Atento.Web.Controllers
{
    public class HomeController : Controller
    {
        ControleArquivoRepository _controleArquivoRepository;

        #region Construtor

        public HomeController()
        {
            _controleArquivoRepository = new ControleArquivoRepository();
        }

        #endregion

        #region Login

        public ActionResult Index(UsuarioModel usuario)
        {
            return View(usuario);
        }

        #endregion

        #region Acessar

        public RedirectToRouteResult Acessar(UsuarioModel usuario)
        {

            _controleArquivoRepository = new ControleArquivoRepository();

            UsuarioModel usuarioRetorno = _controleArquivoRepository.ValidarUsuario(usuario.Login, usuario.Senha);
            if (usuarioRetorno != null)
            {
                Session.Add("NomeUsuario", usuarioRetorno.Usuario);
                Session.Add("LoginUsuario", usuarioRetorno.Login);
                Session.Add("EmailUsuario", usuarioRetorno.Email);
                Session.Add("TipoUsuario", usuarioRetorno.TipoUsuario);
                Session.Add("GrupoUsuario", usuarioRetorno.Grupos.Nome);

                FormsAuthentication.SetAuthCookie(usuario.Login, false);

                return RedirectToAction("ListarArquivo", "ControleArquivo");
            }
            else
            {
                TempData["verificarLogin"] = "Usuário e/ou Senha Incorreto(s)!";

                return RedirectToAction("Index", "Home");
            }

        }

        #endregion

        #region LogOff

        public RedirectToRouteResult LogOff()
        {
            Session.Clear();

            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Criar Novo Usuário

        public ActionResult CriarUsuario()
        {
            if (Session["NomeUsuario"] == null)
            {
                TempData["sessionFinalizada"] = "Sessão Finalizada. Efetue o login novamente.";
                return Redirect(System.Web.Security.FormsAuthentication.LoginUrl);
            }

            ViewData["GrupoUsuarioCkl"] = _controleArquivoRepository.GetAllGruposCkl(Session["TipoUsuario"].ToString(), Session["LoginUsuario"].ToString()).ToList().ConvertAll(x => new SelectListItem { Value = x.Codigo.ToString(), Text = x.Nome });

            return View();
        }

        #endregion

        #region Criar Novo Usuário [HttpPost]

        [HttpPost]
        public ActionResult CriarUsuario(UsuarioModel user, string[] grupoUsuario)
        {

            if (user.Login != null && user.Senha != null && user.Usuario != null)
            {
                _controleArquivoRepository = new ControleArquivoRepository();
                _controleArquivoRepository.CriarUsuario(user, grupoUsuario);

                TempData["newUser"] = "Usuário Criado com Sucesso";

                return RedirectToAction("ListarArquivo", "ControleArquivo");
            }
            else
            {
                return RedirectToAction("CriarUsuario", "Home");
            }
        }

        #endregion

        #region Listar Grupo de Usuários

        public ActionResult ListarGrupoUsuario()
        {
            ControleArquivoRepository _controleArquivoRepository = new ControleArquivoRepository();

            var models = _controleArquivoRepository.GetAllGrupos();

            return View(models);
        }

        #endregion

        #region Criar Grupo de Usuários

        public ActionResult CriarGrupoUsuario()
        {
            GrupoUsuarioModel model = new GrupoUsuarioModel();

            return View(model);

        }

        #endregion

        #region Criar Grupo de Usuários [HttpPost]

        [HttpPost]
        public ActionResult CriarGrupoUsuario(GrupoUsuarioModel model)
        {
            if (model != null)
            {
                _controleArquivoRepository = new ControleArquivoRepository();
                _controleArquivoRepository.AddGrupo(model);

                TempData["newGrupo"] = "Grupo Criado com Sucesso";

                return RedirectToAction("ListarGrupoUsuario", "Home");
            }
            else
            {
                TempData["newUserFail"] = "Preencha todos os Campos!";

                return RedirectToAction("CriarGrupoUsuario", "Home");
            }

        }

        #endregion

        #region Listar Usuários

        public ActionResult ListarUsuarios()
        {
            if (Session["NomeUsuario"] == null)
            {
                TempData["sessionFinalizada"] = "Sessão Finalizada. Efetue o login novamente.";
                return Redirect(System.Web.Security.FormsAuthentication.LoginUrl);
            }

            var models = _controleArquivoRepository.ListarUsuarios(Session["LoginUsuario"].ToString());

            return View(models);
        }

        #endregion

        #region Editar Usuário

        public ActionResult EditarUsuario(string usuarioNome, string usuarioLogin)
        {
            var models = _controleArquivoRepository.EditarUsuario(usuarioNome, usuarioLogin);

            return View(models);
        }

        #endregion

        #region Editar Usuário [HttpPost]

        [HttpPost]
        public ActionResult EditarUsuario(string txtUsuario, string txtEmail, UsuarioModel model)
        {
            _controleArquivoRepository.AtualizarUsuario(txtUsuario, txtEmail, model);

            TempData["updateUser"] = "Usuário Atualizado com Sucesso!";

            return RedirectToAction("ListarUsuarios", "Home");
        }

        #endregion

        #region Detalhar Grupo

        public ActionResult DetalharGrupo(int codigoGrupo, string nomeGrupo)
        {
            var models = _controleArquivoRepository.ListarMembrosGrupo(codigoGrupo);

            ViewBag.nomeGrupo = nomeGrupo;
            return View(models);
        }

        #endregion

        #region Editar Grupo

        public ActionResult EditarGrupo(string nomeGrupo, int codigoGrupo)
        {
            var models = _controleArquivoRepository.EditarGrupo(nomeGrupo, codigoGrupo);

            return View(models);
        }

        #endregion

        #region Editar Grupo [HttpPost]

        [HttpPost]
        public ActionResult EditarGrupo(string txtGrupo, string txtDescricao, GrupoUsuarioModel model)
        {
            _controleArquivoRepository.AtualizarGrupo(txtGrupo, txtDescricao, model);

            TempData["updateGroup"] = "Grupo Atualizado com Sucesso!";

            return RedirectToAction("ListarGrupoUsuario", "Home");
        }

        #endregion

        public ActionResult ExcluirGrupo(string nome, int codigo)
        {
           return RedirectToAction("EditarGrupo", "Home");
        }

    }
}