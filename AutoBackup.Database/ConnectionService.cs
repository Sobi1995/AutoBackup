using AutoBackup.ConsoleApp.Model.Dto;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AutoBackup.Database
{
    public class  ConnectionService : IConnectionService
    {
        public bool checkConnectinString(string connectionString)
        {

            try
            {
                 var connectionModel = GetConnectionDetiles(connectionString);
                using (var connection = new SqlConnection(connectionString))
                {
                    using (var command = new SqlCommand($"SELECT db_id('{connectionModel.InitialCatalog}')", connection))
                    {
                        connection.Open();
                        return (command.ExecuteScalar() != DBNull.Value);
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return true;
        }

        public ConnectionDetilesModel GetConnectionDetiles(string connection)
        {
          
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connection);
            return new ConnectionDetilesModel() { 
            DataSource=builder.DataSource,
            InitialCatalog=builder.InitialCatalog,
            Password=builder.Password,
            UserID=builder.UserID
            
            };
        }

    }

}
