using DnaMais.Atento.Web.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

namespace DnaMais.Atento.Web.Repositories
{
    public sealed class LayoutSaidaRepository
    {
        //Criar Proc
        #region GetAll Layout Saída

        public IEnumerable<LayoutSaidaModel> GetAll()
        {
            var layouts = new Collection<LayoutSaidaModel>();

            using (var conn = new OracleConnection(ServerConfiguration.ConnectionString))
            {
                try
                {
                    using (var command = conn.CreateCommand())
                    {
                        command.CommandText = "SELECT ID_LAYOUT_SAIDA, NM_LAYOUT_SAIDA FROM LAYOUT_SAIDA";
                        conn.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                layouts.Add(new LayoutSaidaModel
                                {
                                    Codigo = (int)reader["ID_LAYOUT_SAIDA"],
                                    Nome = reader["NM_LAYOUT_SAIDA"].ToString()
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
            return layouts;
        }

        #endregion

    }
}