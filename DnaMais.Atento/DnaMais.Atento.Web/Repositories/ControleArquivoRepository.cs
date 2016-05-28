﻿using DnaMais.Atento.Web.Models;
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
                                    QtdeItensProcessados = reader["QT_ITENS_EXPORTADOS"] == DBNull.Value ? 0 : Convert.ToInt32(reader["QT_ITENS_EXPORTADOS"])
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
                            if (dt.Rows.Count == 1)
                            {
                                UsuarioModel objRetorno = new UsuarioModel();
                                objRetorno.Usuario = dt.Rows[0]["NM_USUARIO"].ToString();
                                objRetorno.Login = dt.Rows[0]["DS_LOGIN"].ToString();
                                objRetorno.Email = dt.Rows[0]["DS_EMAIL"].ToString();
                                objRetorno.TipoUsuario = dt.Rows[0]["CD_TIPO_USUARIO"].ToString();
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

        public void CriarUsuario(UsuarioModel user, string[] grupos)
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

        #region Select Grupo de Usuários (Grid)

        public IEnumerable<GrupoUsuarioModel> GetAllGrupos()
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
                                    Descricao = reader["DS_GRUPO_USUARIO_ATENTO"].ToString()
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

        public void AddGrupo(GrupoUsuarioModel model)
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
                        command.CommandText = "PKG_CONTROLE_ARQUIVO_ATENTO.ATUALIZAR_GRUPO_USUARIO";
                        command.Parameters.Add("P_NM_GRUPO_USUARIO_ATENTO", OracleDbType.Varchar2).Value = model.Nome;
                        command.Parameters.Add("P_DS_GRUPO_USUARIO_ATENTO", OracleDbType.Varchar2).Value = model.Descricao;

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

        //CRIAR PROC
        #region Listar Tipo de Usuários

        public IEnumerable<UsuarioModel> GetTipoUsuario()
        {
            var tipos = new Collection<UsuarioModel>();

            using (var conn = new OracleConnection(ServerConfiguration.ConnectionString))
            {
                try
                {
                    using (var command = conn.CreateCommand())
                    {

                        string strSQL = "";
                        strSQL = strSQL + " SELECT CD_TIPO_USUARIO ";
                        strSQL = strSQL + " FROM USUARIO_ATENTO ";

                        if (conn.State == System.Data.ConnectionState.Closed)
                            conn.Open();

                        command.CommandText = strSQL;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tipos.Add(new UsuarioModel
                                {
                                    TipoUsuario = reader["CD_TIPO_USUARIO"].ToString(),
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
            return tipos;
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
                                    QtdeItensRecebidos = Convert.ToInt32(reader["QT_ITENS_RECEBIDOS"]),
                                    QtdeItensProcessados = Convert.ToInt32(reader["QT_ITENS_EXPORTADOS"])
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

        public IEnumerable<UsuarioModel> ListarUsuarios(string loginUsuario)
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
                        command.Parameters.Add("RETORNO_USUARIOS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        command.ExecuteNonQuery();

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                usuarios.Add(new UsuarioModel
                                    {
                                        Usuario = reader["NM_USUARIO"].ToString(),
                                        Email = reader["DS_EMAIL"].ToString(),
                                        Grupos = new GrupoUsuarioModel
                                        {
                                            Nome = reader["NM_GRUPO_USUARIO_ATENTO"].ToString()
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

        public UsuarioModel EditarUsuario(string nomeUsuario, string grupoUsuario)
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
                        command.Parameters.Add("P_NM_GRUPO", OracleDbType.Varchar2).Value = grupoUsuario;
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
                                objRetorno.Grupos = new GrupoUsuarioModel
                                {
                                    Nome = dt.Rows[0]["NM_GRUPO_USUARIO_ATENTO"].ToString()
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

    }
}