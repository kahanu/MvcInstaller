using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using MvcInstaller;
using MvcInstaller.Settings;
using System.Data.SqlClient;

namespace MvcInstallerV3.UnitTests
{
    [TestClass]
    public class SqlTests
    {
        [TestMethod]
        public void run_failing_sql_command_to_get_error_list()
        {
            // Arrange
            InstallerConfig config = Serializer<InstallerConfig>.Deserialize(AppDomain.CurrentDomain.BaseDirectory + @"\installer.config");
            config.Path.AppPath = @"C:\VSProjects\MvcInstaller\MvcInstallerV3.UnitTests\";

            try
            {
                RunScripts(config);
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        #region Private Methods


        private void RunScripts(InstallerConfig config)
        {
            string[] files = Directory.GetFiles(config.Path.AppPath + config.Path.RelativeSqlPath);
            foreach (string file in files)
            {
                string[] statements = GetScriptStatements(File.ReadAllText(file, new System.Text.UTF8Encoding()));
                ExecuteStatements(statements, config);
            }
        }



        /// <summary>
        /// This will execute the sql statements.
        /// </summary>
        /// <param name="tableStatements"></param>
        /// <param name="dbName"></param>
        private void ExecuteStatements(string[] tableStatements, InstallerConfig config)
        {
            if (tableStatements.Length > 0)
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    IConnectionStringComponent component = new ConnectionStringComponent(config);
                    conn.ConnectionString = component.GetConnString();
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(string.Empty, conn))
                    {
                        foreach (string statement in tableStatements)
                        {
                            command.CommandText = statement;
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        private string[] GetScriptStatements(string p)
        {
            string[] statements = p.Split(new string[] { "GO\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            return statements;
        }

        public void DisplaySqlErrors(SqlException exception)
        {
            for (int i = 0; i < exception.Errors.Count; i++)
            {
                Console.WriteLine("Index #" + i + "\n" +
                    "Source: " + exception.Errors[i].Source + "\n" +
                    "Number: " + exception.Errors[i].Number.ToString() + "\n" +
                    "State: " + exception.Errors[i].State.ToString() + "\n" +
                    "Class: " + exception.Errors[i].Class.ToString() + "\n" +
                    "Server: " + exception.Errors[i].Server + "\n" +
                    "Message: " + exception.Errors[i].Message + "\n" +
                    "Procedure: " + exception.Errors[i].Procedure + "\n" +
                    "LineNumber: " + exception.Errors[i].LineNumber.ToString());
            }
            
        }

        #endregion
    }
}
