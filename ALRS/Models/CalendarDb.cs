using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Web;

namespace ALRSSystem.Models
{
    public class CalendarDb
    {

        private static string ConnectionString
        {
            get
            {
                bool mssql = !SqLiteFound();
                if (mssql)
                {
                    return ConfigurationManager.ConnectionStrings["ALRSDB"].ConnectionString;
                }

                if (HttpContext.Current.Session["cs"] as string == null)
                {
                    HttpContext.Current.Session["cs"] = GetNew();
                }

                return (string)HttpContext.Current.Session["cs"];
            }
        }

        private static DbProviderFactory Factory
        {
            get
            {
                return DbProviderFactories.GetFactory(FactoryName());
            }
        }

        private static string FactoryName()
        {
            if (SqLiteFound())
            {
                return "System.Data.SQLite";
            }
            return "System.Data.SqlClient";
        }

        private static string IdentityCommand()
        {
            switch (FactoryName())
            {
                case "System.Data.SQLite":
                    return "select last_insert_rowid();";
                case "System.Data.SqlClient":
                    return "select @@identity;";
                default:
                    throw new NotSupportedException("Unsupported DB factory.");
            }
        }


        private static string GetNew()
        {
            string today = DateTime.Today.ToString("yyyy-MM-dd");
            string guid = Guid.NewGuid().ToString();
            string dir = HttpContext.Current.Server.MapPath("~/App_Data/session/" + today + "/");
            string master = HttpContext.Current.Server.MapPath("~/App_Data/daypilot.sqlite");
            string path = dir + guid;

            Directory.CreateDirectory(dir);
            File.Copy(master, path);

            return String.Format("Data Source={0}", path);
        }

        private static bool SqLiteFound()
        {
            string path = HttpContext.Current.Server.MapPath("~/bin/System.Data.SQLite.dll");
            return File.Exists(path);
        }


        public static DbDataAdapter CreateDataAdapter(string select)
        {
            DbDataAdapter da = Factory.CreateDataAdapter();
            da.SelectCommand = CreateCommand(select);
            return da;
        }


        public static DbConnection CreateConnection()
        {
            DbConnection connection = Factory.CreateConnection();
            connection.ConnectionString = ConnectionString;
            return connection;
        }

        public static DbCommand CreateCommand(string text)
        {
            DbCommand command = Factory.CreateCommand();
            command.CommandText = text;
            command.Connection = CreateConnection();

            return command;
        }

        public static DbCommand CreateCommand(string text, DbConnection connection)
        {
            DbCommand command = Factory.CreateCommand();
            command.CommandText = text;
            command.Connection = connection;

            return command;
        }

        public static void AddParameterWithValue(DbCommand cmd, string name, object value)
        {
            var parameter = Factory.CreateParameter();
            parameter.Direction = ParameterDirection.Input;
            parameter.ParameterName = name;
            parameter.Value = value;
            cmd.Parameters.Add(parameter);
        }

        public static int GetIdentity(DbConnection c)
        {
            var cmd = CreateCommand(CalendarDb.IdentityCommand(), c);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

    }
}