using DnaMais.Atento.Web.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

namespace DnaMais.Atento.Web.Repositories
{
    public sealed class LayoutEntradaRepository
    {
        //Criar Proc
        #region GetAll

        public IEnumerable<LayoutEntradaModel> GetAll()
        {
            var layouts = new Collection<LayoutEntradaModel>();

            using (var conn = new OracleConnection(ServerConfiguration.ConnectionString))
            {
                try
                {
                    using (var command = conn.CreateCommand())
                    {
                        command.CommandText = "SELECT ID_LAYOUT_ENTRADA, NM_LAYOUT_ENTRADA FROM LAYOUT_ENTRADA";
                        conn.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                layouts.Add(new LayoutEntradaModel
                                {
                                    Codigo = (int)reader["ID_LAYOUT_ENTRADA"],
                                    Nome = reader["NM_LAYOUT_ENTRADA"].ToString()
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