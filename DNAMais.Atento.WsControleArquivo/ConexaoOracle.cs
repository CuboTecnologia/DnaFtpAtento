using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;

namespace DNAMais.Atento.WsControleArquivo
{
    public class ConexaoOracle : IDisposable
    {
        private OracleConnection m_conexao;
        private OracleCommand m_comando;

        public ConexaoOracle(string procedure)
        {
            m_conexao = new OracleConnection();
            m_conexao.ConnectionString =
                ConfigurationManager.ConnectionStrings["connDNA"].ToString();

            m_comando = new OracleCommand();
            m_comando.CommandType = CommandType.StoredProcedure;
            m_comando.CommandText = procedure;
            m_comando.Connection = m_conexao;

            m_conexao.Open();
        }

        public ConexaoOracle()
        {
            m_conexao = new OracleConnection();
            m_conexao.ConnectionString =
                ConfigurationManager.ConnectionStrings["connDNA"].ToString();

            m_comando = new OracleCommand();
            m_comando.CommandType = CommandType.Text;
            m_comando.Connection = m_conexao;

            m_conexao.Open();
        }

        public void AddParametroIN(string nome, object valor)
        {
            m_comando.Parameters.Add(new OracleParameter(nome, valor));
        }

        public void AddParametroOUT(string nome)
        {
            OracleParameter paramOUT = new OracleParameter();

            paramOUT.ParameterName = nome;
            paramOUT.Direction = ParameterDirection.Output;
            paramOUT.OracleDbType = OracleDbType.Int32;
            paramOUT.Size = 10;

            m_comando.Parameters.Add(paramOUT);
        }

        public bool ExecutarDML(out string mensagem)
        {
            m_comando.ExecuteNonQuery();

            mensagem = "Executado com sucesso.";

            return true;
        }

        public OracleDataReader ObterLeitor(string comando)
        {
            m_comando.CommandText = comando;
            return m_comando.ExecuteReader();
        }

        public int ObterParamOUT(string nome)
        {
            return Convert.ToInt32(m_comando.Parameters[nome].Value);
        }

        public void Dispose()
        {
            m_conexao.Close();

            m_comando.Dispose();
            m_conexao.Dispose();
        }
    }
}
