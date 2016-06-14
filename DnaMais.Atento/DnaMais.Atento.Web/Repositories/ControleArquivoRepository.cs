using DnaMais.Atento.Web.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace DnaMais.Atento.Web.Repositories
{
    public sealed class ControleArquivoRepository
    {
        #region GetAll Grid Controle Arquivo

        public IEnumerable<ControleArquivoModel> GetAll(string loginUsuario)
        {
            var controles = new Collection<ControleArquivoModel>();

            using (var conn = new OracleConnection(ServerConfiguration.ConnectionString))
            {
                try
                {
                    using (var command = conn.CreateCommand())
                    {
                        if (conn.State == System.Data.ConnectionState.Closed)
                            conn.Open();

                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PKG_CONTROLE_ARQUIVO_ATENTO.LISTAR_CONTROLE_ARQUIVO";
                        command.Parameters.Add("P_DS_LOGIN", OracleDbType.Varchar2).Value = loginUsuario;
                        command.Parameters.Add("RETORNO_CONTROLE_ARQUIVO", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                controles.Add(new ControleArquivoModel
                                {
                                    Codigo = (int)reader["ID_CONTROLE"],
                                    LayoutEntrada = new LayoutEntradaModel
                                    {
                                        Nome = reader["NM_LAYOUT_ENTRADA"].ToString()
                                    },
                                    LayoutSaida = new LayoutSaidaModel
                                    {
                                        Nome = reader["NM_LAYOUT_SAIDA"].ToString()
                                    },
                                    NomeArquivoEntrada = reader["NM_ARQUIVO_ENTRADA"].ToString(),
                                    NomeArquivoEntradaOriginal = reader["NM_ARQUIVO_ENTRADA_ORIGINAL"].ToString(),
                                    Status = new StatusModel
                                    {
                                        Codigo = Convert.ToInt16(reader["CD_STATUS_CONTROLE_ARQUIVO"]),
                                        DescricaoStatus = reader["DS_STATUS_CONTROLE_ARQUIVO"].ToString()
                                    },
                                    DataRegistro = (DateTime)reader["DT_REGISTRO"],
                                    DataInicioExecucao = reader["DT_INICIO_EXECUCAO"] != DBNull.Value ? (DateTime?)reader["DT_INICIO_EXECUCAO"] : null,
                                    DataTerminoExecucao = reader["DT_TERMINO_EXECUCAO"] != DBNull.Value ? (DateTime?)reader["DT_TERMINO_EXECUCAO"] : null,
                                    LoginSolicitante = reader["DS_LOGIN"].ToString(),
                                    NomeUsuarioSolicitante = reader["NM_USUARIO"].ToString(),
                                    QtdeItensRecebidos = reader["QT_ITENS_RECEBIDOS"] == DBNull.Value ? 0 : Convert.ToInt32(reader["QT_ITENS_RECEBIDOS"]),
                                    QtdeItensProcessados = reader["QT_ITENS_EXPORTADOS"] == DBNull.Value ? 0 : Convert.ToInt32(reader["QT_ITENS_EXPORTADOS"]),
                                    QtdeEnriquecidoEndereco = reader["QT_ENRIQUECIDO_ENDERECO"] == DBNull.Value ? 0 : Convert.ToInt32(reader["QT_ENRIQUECIDO_ENDERECO"]),
                                    QtdeEnriquecidoFone = reader["QT_ENRIQUECIDO_FONE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["QT_ENRIQUECIDO_FONE"]),
                                    QtdeEnriquecidoCelular = reader["QT_ENRIQUECIDO_CELULAR"] == DBNull.Value ? 0 : Convert.ToInt32(reader["QT_ENRIQUECIDO_CELULAR"]),
                                    QtdeEnriquecidoEmail = reader["QT_ENRIQUECIDO_EMAIL"] == DBNull.Value ? 0 : Convert.ToInt32(reader["QT_ENRIQUECIDO_EMAIL"])
                                });
                            }
                        }
                    }
                }
                finally
                {
                    conn.Close();
                }
            }
            return controles;
        }

        #endregion

        #region Insert Tabela Controle Arquivo

        public void Add(ControleArquivoModel model)
        {

            using (var conn = new OracleConnection(ServerConfiguration.ConnectionString))
            {
                using (var command = conn.CreateCommand())
                {
                    if (conn.State == System.Data.ConnectionState.Closed)
                        conn.Open();

                    try
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PKG_CONTROLE_ARQUIVO_ATENTO.ATUALIZAR_CONTROLE_ARQUIVO";
                        command.Parameters.Add("P_DT_REGISTRO", OracleDbType.Date).Value = model.DataRegistro;
                        command.Parameters.Add("P_ID_LAYOUT_ENTRADA", OracleDbType.Int32).Value = model.CodigoLayoutEntrada;
                        command.Parameters.Add("P_ID_LAYOUT_SAIDA", OracleDbType.Int32).Value = model.CodigoLayoutSaida;
                        command.Parameters.Add("P_NM_ARQUIVO_ENTRADA", OracleDbType.Varchar2).Value = model.NomeArquivoEntrada;
                        command.Parameters.Add("P_NM_ARQUIVO_ENTRADA_ORIGINAL", OracleDbType.Varchar2).Value = model.NomeArquivoEntradaOriginal;
                        command.Parameters.Add("P_DS_LOGIN", OracleDbType.Varchar2).Value = model.LoginSolicitante;

                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        #endregion

        #region Validar Usuário

        public UsuarioModel ValidarUsuario(string usuario, string senha)
        {
            UsuarioModel user = new UsuarioModel();

            using (var conn = new OracleConnection(ServerConfiguration.ConnectionString))
            {
                try
                {
                    using (var command = conn.CreateCommand())
                    {
                        if (conn.State == System.Data.ConnectionState.Closed)
                            conn.Open();

                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PKG_CONTROLE_ARQUIVO_ATENTO.VERIFICAR_LOGIN_USUARIO";
                        command.Parameters.Add("P_DS_LOGIN", OracleDbType.Varchar2).Value = usuario;
                        command.Parameters.Add("P_DS_SENHA", OracleDbType.Varchar2).Value = senha;
                        command.Parameters.Add("RETORNO_LOGIN", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        DataTable dt = new DataTable();

                        OracleDataReader reader = command.ExecuteReader();

                        if (reader.HasRows)
                        {
                            dt.Load(reader);
                            if (dt.Rows.Count != 0)
                            {
                                UsuarioModel objRetorno = new UsuarioModel();
                                objRetorno.Usuario = dt.Rows[0]["NM_USUARIO"].ToString();
                                objRetorno.Login = dt.Rows[0]["DS_LOGIN"].ToString();
                                objRetorno.Email = dt.Rows[0]["DS_EMAIL"].ToString();
                                objRetorno.TipoUsuario = dt.Rows[0]["CD_TIPO_USUARIO"].ToString();
                                objRetorno.Grupos = new GrupoUsuarioModel
                                {
                                    Nome = dt.Rows[0]["NM_GRUPO_USUARIO_ATENTO"].ToString(),
                                    Criador = dt.Rows[0]["DS_CRIADOR_GRUPO"].ToString()
                                };
                                return objRetorno;
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else
                        {
                            return null;
                        }

                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        #endregion

        #region Criar Usuário

        public void CriarUsuario(UsuarioModel user, string[] grupos, string loginUsuario)
        {

            using (var conn = new OracleConnection(ServerConfiguration.ConnectionString))
            {
                using (var command = conn.CreateCommand())
                {
                    if (conn.State == System.Data.ConnectionState.Closed)
                        conn.Open();

                    try
                    {
                        string codGrupos = null;
                        foreach (string codigoGrupo in grupos)
                        {
                            codGrupos = codGrupos + codigoGrupo + ",";
                        }
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PKG_CONTROLE_ARQUIVO_ATENTO.CRIAR_USUARIO_ATENTO";
                        command.Parameters.Add("P_DS_LOGIN", OracleDbType.Varchar2).Value = user.Login;
                        command.Parameters.Add("P_DS_SENHA", OracleDbType.Varchar2).Value = user.Senha;
                        command.Parameters.Add("P_DS_CONFIRMACAO_SENHA", OracleDbType.Varchar2).Value = user.ConfirmarSenha;
                        command.Parameters.Add("P_NM_USUARIO", OracleDbType.Varchar2).Value = user.Usuario;
                        command.Parameters.Add("P_DS_EMAIL", OracleDbType.Varchar2).Value = user.Email;
                        command.Parameters.Add("P_CD_GRUPOS", OracleDbType.Varchar2).Value = codGrupos.Substring(0, codGrupos.Length - 1);
                        command.Parameters.Add("P_CD_TIPO_USUARIO", OracleDbType.Char).Value = user.TipoUsuario;
                        command.Parameters.Add("P_DS_CRIADOR_USUARIO", OracleDbType.Varchar2).Value = loginUsuario;

                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        #endregion

        #region Download Ftp por ID

        public string GetNomeDownloadArquivoById(int itemCodigo)
        {
            string nomeArquivoDownload = null;

            using (var conn = new OracleConnection(ServerConfiguration.ConnectionString))
            {
                try
                {
                    using (var command = conn.CreateCommand())
                    {
                        if (conn.State == System.Data.ConnectionState.Closed)
                            conn.Open();

                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PKG_CONTROLE_ARQUIVO_ATENTO.BAIXAR_ARQUIVO_POR_ID";
                        command.Parameters.Add("P_ID_CONTROLE", OracleDbType.Int32).Value = itemCodigo;
                        command.Parameters.Add("RETORNO_POR_ID", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                nomeArquivoDownload = reader["NM_ARQUIVO_DOWNLOAD"].ToString();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return nomeArquivoDownload;
        }

        #endregion

        #region Listar Grupo de Usuários (Grid)

        public IEnumerable<GrupoUsuarioModel> GetAllGrupos(string loginUsuario, string tipoUsuario)
        {
            var grupos = new Collection<GrupoUsuarioModel>();

            using (var conn = new OracleConnection(ServerConfiguration.ConnectionString))
            {
                try
                {
                    using (var command = conn.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PKG_CONTROLE_ARQUIVO_ATENTO.LISTAR_GRUPO_USUARIO";
                        command.Parameters.Add("P_DS_LOGIN", OracleDbType.Varchar2).Value = loginUsuario;
                        command.Parameters.Add("P_TIPO_USUARIO", OracleDbType.Varchar2).Value = tipoUsuario;
                        command.Parameters.Add("RETORNO_GRUPO_USUARIO", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        if (conn.State == System.Data.ConnectionState.Closed)
                            conn.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                grupos.Add(new GrupoUsuarioModel
                                {
                                    Codigo = (int)reader["ID_GRUPO_USUARIO_ATENTO"],
                                    Nome = reader["NM_GRUPO_USUARIO_ATENTO"].ToString(),
                                    Descricao = reader["DS_GRUPO_USUARIO_ATENTO"].ToString(),
                                    Criador = reader["DS_CRIADOR_GRUPO"].ToString()
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return grupos;
        }

        #endregion

        #region Criar Grupo de Usuário

        public void AddGrupo(GrupoUsuarioModel model, string loginUsuario)
        {

            using (var conn = new OracleConnection(ServerConfiguration.ConnectionString))
            {
                using (var command = conn.CreateCommand())
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    try
                    {

                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PKG_CONTROLE_ARQUIVO_ATENTO.CRIAR_GRUPO_USUARIO";
                        command.Parameters.Add("P_NM_GRUPO_USUARIO_ATENTO", OracleDbType.Varchar2).Value = model.Nome;
                        command.Parameters.Add("P_DS_GRUPO_USUARIO_ATENTO", OracleDbType.Varchar2).Value = model.Descricao;
                        command.Parameters.Add("P_CRIADOR_GRUPO", OracleDbType.Varchar2).Value = loginUsuario;

                        command.ExecuteNonQuery();

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        #endregion

        #region Listar Grupo de Usuários CheckList

        public IEnumerable<GrupoUsuarioModel> GetAllGruposCkl(string tipo, string login)
        {
            var grupos = new Collection<GrupoUsuarioModel>();

            using (var conn = new OracleConnection(ServerConfiguration.ConnectionString))
            {
                try
                {
                    using (var command = conn.CreateCommand())
                    {
                        if (conn.State == System.Data.ConnectionState.Closed)
                            conn.Open();

                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PKG_CONTROLE_ARQUIVO_ATENTO.LISTAR_GRUPO_USUARIO_CKL";
                        command.Parameters.Add("P_TIPO_USUARIO", OracleDbType.Varchar2).Value = tipo;
                        command.Parameters.Add("P_DS_LOGIN", OracleDbType.Varchar2).Value = login;
                        command.Parameters.Add("RETORNO_GRUPO_USUARIO", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        command.ExecuteNonQuery();

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                grupos.Add(new GrupoUsuarioModel
                                {
                                    Codigo = (int)reader["ID_GRUPO_USUARIO_ATENTO"],
                                    Nome = reader["NM_GRUPO_USUARIO_ATENTO"].ToString(),
                                });
                            }
                        }
                    }
                }
                finally
                {
                    conn.Close();
                }
            }
            return grupos;
        }

        #endregion

        #region Gerar Relatório

        public IEnumerable<ControleArquivoModel> GerarRelatorio(int itemCodigo)
        {
            var campos = new Collection<ControleArquivoModel>();

            using (var conn = new OracleConnection(ServerConfiguration.ConnectionString))
            {
                try
                {
                    using (var command = conn.CreateCommand())
                    {
                        if (conn.State == System.Data.ConnectionState.Closed)
                            conn.Open();

                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PKG_CONTROLE_ARQUIVO_ATENTO.GERAR_RELATORIO";
                        command.Parameters.Add("P_ID_CONTROLE", OracleDbType.Int32).Value = itemCodigo;
                        command.Parameters.Add("RETORNO_RELATORIO", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                campos.Add(new ControleArquivoModel
                                {
                                    Codigo = Convert.ToInt32(reader["ID_CONTROLE"]),
                                    LayoutEntrada = new LayoutEntradaModel
                                    {
                                        Nome = reader["NM_LAYOUT_ENTRADA"].ToString()
                                    },
                                    LayoutSaida = new LayoutSaidaModel
                                    {
                                        Nome = reader["NM_LAYOUT_SAIDA"].ToString()
                                    },
                                    NomeArquivoEntradaOriginal = reader["NM_ARQUIVO_ENTRADA_ORIGINAL"].ToString(),
                                    Status = new StatusModel
                                    {
                                        DescricaoStatus = reader["DS_STATUS_CONTROLE_ARQUIVO"].ToString()
                                    },
                                    DataRegistro = (DateTime)reader["DT_REGISTRO"],
                                    DataInicioExecucao = reader["DT_INICIO_EXECUCAO"] != DBNull.Value ? (DateTime?)reader["DT_INICIO_EXECUCAO"] : null,
                                    DataTerminoExecucao = reader["DT_TERMINO_EXECUCAO"] != DBNull.Value ? (DateTime?)reader["DT_TERMINO_EXECUCAO"] : null,
                                    NomeArquivoDownload = reader["NM_ARQUIVO_DOWNLOAD"].ToString(),
                                    NomeUsuarioSolicitante = reader["NM_USUARIO"].ToString(),
                                    QtdeItensRecebidos = reader["QT_ITENS_RECEBIDOS"] == DBNull.Value ? 0 : Convert.ToInt32(reader["QT_ITENS_RECEBIDOS"]),
                                    QtdeItensProcessados = reader["QT_ITENS_EXPORTADOS"] == DBNull.Value ? 0 : Convert.ToInt32(reader["QT_ITENS_EXPORTADOS"]),
                                    QtdeEnriquecidoEndereco = reader["QT_ENRIQUECIDO_ENDERECO"] == DBNull.Value ? 0 : Convert.ToInt32(reader["QT_ENRIQUECIDO_ENDERECO"]),
                                    QtdeEnriquecidoFone = reader["QT_ENRIQUECIDO_FONE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["QT_ENRIQUECIDO_FONE"]),
                                    QtdeEnriquecidoCelular = reader["QT_ENRIQUECIDO_CELULAR"] == DBNull.Value ? 0 : Convert.ToInt32(reader["QT_ENRIQUECIDO_CELULAR"]),
                                    QtdeEnriquecidoEmail = reader["QT_ENRIQUECIDO_EMAIL"] == DBNull.Value ? 0 : Convert.ToInt32(reader["QT_ENRIQUECIDO_EMAIL"])
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return campos;
        }

        #endregion

        #region Listar Usuários

        public IEnumerable<UsuarioModel> ListarUsuarios(string loginUsuario, string tipoUsuario)
        {
            var usuarios = new Collection<UsuarioModel>();

            using (var conn = new OracleConnection(ServerConfiguration.ConnectionString))
            {
                try
                {
                    using (var command = conn.CreateCommand())
                    {
                        if (conn.State == System.Data.ConnectionState.Closed)
                            conn.Open();

                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PKG_CONTROLE_ARQUIVO_ATENTO.LISTAR_USUARIOS";
                        command.Parameters.Add("P_DS_LOGIN", OracleDbType.Varchar2).Value = loginUsuario;
                        command.Parameters.Add("P_TIPO_USUARIO", OracleDbType.Varchar2).Value = tipoUsuario;
                        command.Parameters.Add("RETORNO_USUARIOS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        command.ExecuteNonQuery();

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                usuarios.Add(new UsuarioModel
                                    {
                                        Login = reader["DS_LOGIN"].ToString(),
                                        Usuario = reader["NM_USUARIO"].ToString(),
                                        Email = reader["DS_EMAIL"].ToString(),
                                        CriadorUsuario = reader["DS_CRIADOR_USUARIO"].ToString(),

                                        Grupos = new GrupoUsuarioModel
                                        {
                                            Nome = reader["NM_GRUPO_USUARIO_ATENTO"].ToString(),
                                            Codigo = Convert.ToInt32(reader["ID_GRUPO_USUARIO_ATENTO"]),
                                            Criador = reader["DS_CRIADOR_GRUPO"].ToString()
                                        }
                                    });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return usuarios;
        }

        #endregion

        #region Editar Usuário

        public UsuarioModel EditarUsuario(string nomeUsuario, string usuarioLogin)
        {
            var usuario = new UsuarioModel();

            using (var conn = new OracleConnection(ServerConfiguration.ConnectionString))
            {
                try
                {
                    using (var command = conn.CreateCommand())
                    {
                        if (conn.State == System.Data.ConnectionState.Closed)
                            conn.Open();

                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PKG_CONTROLE_ARQUIVO_ATENTO.EDITAR_USUARIO";
                        command.Parameters.Add("P_NM_USUARIO", OracleDbType.Varchar2).Value = nomeUsuario;
                        command.Parameters.Add("P_DS_LOGIN", OracleDbType.Varchar2).Value = usuarioLogin;
                        command.Parameters.Add("RETORNO_USUARIO", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        DataTable dt = new DataTable();

                        OracleDataReader reader = command.ExecuteReader();

                        if (reader.HasRows)
                        {
                            dt.Load(reader);
                            if (dt.Rows.Count == 1)
                            {
                                UsuarioModel objRetorno = new UsuarioModel();
                                objRetorno.Usuario = dt.Rows[0]["NM_USUARIO"].ToString();
                                objRetorno.Email = dt.Rows[0]["DS_EMAIL"].ToString();
                                objRetorno.Login = dt.Rows[0]["DS_LOGIN"].ToString();

                                return objRetorno;
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        #endregion

        #region Atualizar Usuário

        public void AtualizarUsuario(string novoUsuario, string novoEmail, string[] grupoUsuario, UsuarioModel model)
        {

            using (var conn = new OracleConnection(ServerConfiguration.ConnectionString))
            {
                try
                {
                    using (var command = conn.CreateCommand())
                    {
                        string codGrupos = null;
                        foreach (string item in grupoUsuario)
                        {
                            codGrupos = codGrupos + item + ",";
                        }
                        if (conn.State == System.Data.ConnectionState.Closed)
                            conn.Open();

                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PKG_CONTROLE_ARQUIVO_ATENTO.ATUALIZAR_USUARIO";
                        command.Parameters.Add("P_DS_LOGIN", OracleDbType.Varchar2).Value = model.Login;
                        command.Parameters.Add("P_NM_NOVO_USUARIO", OracleDbType.Varchar2).Value = novoUsuario;
                        command.Parameters.Add("P_DS_NOVO_EMAIL", OracleDbType.Varchar2).Value = novoEmail;
                        command.Parameters.Add("P_CD_GRUPOS", OracleDbType.Varchar2).Value = codGrupos.Substring(0, codGrupos.Length - 1);

                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
        }

        #endregion

        #region Listar Membros do Grupo

        public IEnumerable<UsuarioModel> ListarMembrosGrupo(int codigoGrupo)
        {
            var membros = new Collection<UsuarioModel>();

            using (var conn = new OracleConnection(ServerConfiguration.ConnectionString))
            {
                using (var command = conn.CreateCommand())
                {
                    try
                    {
                        if (conn.State == System.Data.ConnectionState.Closed)
                            conn.Open();

                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PKG_CONTROLE_ARQUIVO_ATENTO.LISTAR_MEMBROS_GRUPO";
                        command.Parameters.Add("P_ID_GRUPO", OracleDbType.Int32).Value = codigoGrupo;
                        command.Parameters.Add("RETORNO_GRUPO", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        command.ExecuteNonQuery();

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                membros.Add(new UsuarioModel
                                    {
                                        Login = reader["DS_LOGIN"].ToString(),
                                        Usuario = reader["NM_USUARIO"].ToString(),
                                        Email = reader["DS_EMAIL"].ToString(),

                                        Grupos = new GrupoUsuarioModel
                                        {
                                            Nome = reader["NM_GRUPO_USUARIO_ATENTO"].ToString(),
                                            Descricao = reader["DS_GRUPO_USUARIO_ATENTO"].ToString(),
                                        }
                                    });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            return membros;
        }

        #endregion

        #region Editar Grupo

        public GrupoUsuarioModel EditarGrupo(string nomeGrupo, int codigoGrupo)
        {
            using (var conn = new OracleConnection(ServerConfiguration.ConnectionString))
            {
                try
                {
                    using (var command = conn.CreateCommand())
                    {
                        if (conn.State == System.Data.ConnectionState.Closed)
                            conn.Open();

                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PKG_CONTROLE_ARQUIVO_ATENTO.EDITAR_GRUPO";
                        command.Parameters.Add("NM_GRUPO_USUARIO_ATENTO", OracleDbType.Varchar2).Value = nomeGrupo;
                        command.Parameters.Add("DS_GRUPO_USUARIO_ATENTO", OracleDbType.Int32).Value = codigoGrupo;
                        command.Parameters.Add("RETORNO_GRUPO", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        DataTable dt = new DataTable();

                        OracleDataReader reader = command.ExecuteReader();

                        if (reader.HasRows)
                        {
                            dt.Load(reader);
                            if (dt.Rows.Count == 1)
                            {
                                GrupoUsuarioModel objRetorno = new GrupoUsuarioModel();
                                objRetorno.Codigo = Convert.ToInt32(dt.Rows[0]["ID_GRUPO_USUARIO_ATENTO"]);
                                objRetorno.Nome = dt.Rows[0]["NM_GRUPO_USUARIO_ATENTO"].ToString();
                                objRetorno.Descricao = dt.Rows[0]["DS_GRUPO_USUARIO_ATENTO"].ToString();

                                return objRetorno;
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        #endregion

        #region Atualizar Grupo

        public void AtualizarGrupo (string novoGrupo, string novaDescricao, GrupoUsuarioModel model)
        {
            using (var conn = new OracleConnection(ServerConfiguration.ConnectionString))
            {
                try
                {
                    if (conn.State == System.Data.ConnectionState.Closed)
                        conn.Open();

                    using (var command = conn.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PKG_CONTROLE_ARQUIVO_ATENTO.ATUALIZAR_GRUPO";
                        command.Parameters.Add("ID_GRUPO_USUARIO_ATENTO", OracleDbType.Int32).Value = model.Codigo;
                        command.Parameters.Add("NM_GRUPO_USUARIO_ATENTO", OracleDbType.Varchar2).Value = novoGrupo;
                        command.Parameters.Add("DS_GRUPO_USUARIO_ATENTO", OracleDbType.Varchar2).Value = novaDescricao;

                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        #endregion

        #region Verificar Usuário Controle

        public bool VerificarUsuarioControle(string usuarioLogin)
        {

            using (var conn = new OracleConnection(ServerConfiguration.ConnectionString))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                try
                {
                    using (var command = conn.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PKG_CONTROLE_ARQUIVO_ATENTO.VERIFICAR_USUARIO_CONTROLE";
                        command.Parameters.Add("P_DS_LOGIN", OracleDbType.Varchar2).Value = usuarioLogin;
                        command.Parameters.Add("RETORNO_USUARIO", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        DataTable dt = new DataTable();

                        OracleDataReader reader = command.ExecuteReader();

                        dt.Load(reader);

                        if (dt.Rows.Count != 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }

        #endregion

        #region Deletar Usuário

        public void DeletarUsuario(string usuarioLogin)
        {
            using (var conn = new OracleConnection(ServerConfiguration.ConnectionString))
            {
                try
                {
                    if (conn.State == System.Data.ConnectionState.Closed)
                        conn.Open();

                    using (var command = conn.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PKG_CONTROLE_ARQUIVO_ATENTO.DELETAR_USUARIO";
                        command.Parameters.Add("P_DS_LOGIN", OracleDbType.Varchar2).Value = usuarioLogin;

                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        #endregion

        #region Verificar Grupo Usuário

        public bool VerificarUsuarioGrupo(int codGrupo)
        {
            using (var conn = new OracleConnection(ServerConfiguration.ConnectionString))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                using (var command = conn.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PKG_CONTROLE_ARQUIVO_ATENTO.VERIFICAR_USUARIO_GRUPO";
                    command.Parameters.Add("P_CD_GRUPO", OracleDbType.Int32).Value = codGrupo;
                    command.Parameters.Add("RETORNO_CD_GRUPO", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    DataTable dt = new DataTable();
                    OracleDataReader reader = command.ExecuteReader();

                    dt.Load(reader);

                    if (dt.Rows.Count != 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        #endregion

        #region Deletar Grupo

        public void DeletarGrupo(int codGrupo)
        {
            using (var conn = new OracleConnection(ServerConfiguration.ConnectionString))
            {

                try
                {
                    if (conn.State == System.Data.ConnectionState.Closed)
                        conn.Open();

                    using (var command = conn.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PKG_CONTROLE_ARQUIVO_ATENTO.DELETAR_GRUPO";
                        command.Parameters.Add("P_CD_GRUPO", OracleDbType.Int32).Value = codGrupo;

                        command.ExecuteNonQuery();
                    }

                }
                catch (Exception ex)
                {
                    
                    throw ex;
                }
                
            }
        }

        #endregion

        #region Listar Grupos

        public List<string> GetAllGrupos(string loginUsuario)
        {
            List<string> grupos = new List<string>();

            using (var conn = new OracleConnection(ServerConfiguration.ConnectionString))
            {
                try
                {
                    if (conn.State == System.Data.ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    using (var command = conn.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PKG_CONTROLE_ARQUIVO_ATENTO.LISTAR_GRUPOS";
                        command.Parameters.Add("P_DS_LOGIN", OracleDbType.Varchar2).Value = loginUsuario;
                        command.Parameters.Add("RETORNO_GRUPO", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        command.ExecuteNonQuery();

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                grupos.Add(reader["ID_GRUPO_USUARIO_ATENTO"].ToString());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
            return grupos;
        }

        #endregion

        #region Verificar Usuário

        public bool VerificarUsuario(string loginUsuario)
        {
            using (var conn = new OracleConnection(ServerConfiguration.ConnectionString))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                using (var command = conn.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PKG_CONTROLE_ARQUIVO_ATENTO.VERIFICAR_USUARIO";
                    command.Parameters.Add("P_DS_LOGIN", OracleDbType.Varchar2).Value = loginUsuario;
                    command.Parameters.Add("RETORNO_USUARIO", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    DataTable dt = new DataTable();
                    OracleDataReader reader = command.ExecuteReader();

                    dt.Load(reader);

                    if (dt.Rows.Count == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        #endregion

    }
}