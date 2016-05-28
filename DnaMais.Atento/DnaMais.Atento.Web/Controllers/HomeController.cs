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

                FormsAuthentication.SetAuthCookie(usuario.Login, false);

                return RedirectToAction("ListarArquivo", "ControleArquivo");
            }
            else
            {
                TempData["logado"] = "Usuário e/ou Senha Incorreto(s)!";

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
                TempData["newUserFail"] = "Preencha todos os Campos!";

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

            var models = _controleArquivoRepository.ListarUsuarios(Session["LoginUsuario"].ToString());

            return View(models);
        }

        #endregion

        #region Editar Usuários

        public ActionResult EditarUsuarios(string usuarioNome, string usuarioGrupo)
        {
            var models = _controleArquivoRepository.EditarUsuario(usuarioNome, usuarioGrupo);

            return View(models);
        }

        #endregion

    }
}