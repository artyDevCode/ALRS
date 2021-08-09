using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

using System.Data;
using System.Data.Common;
using System.Configuration;
using System.IO;

namespace ALRSSystem.Models
{
    public class ALRSDB : DbContext
    {

        public ALRSDB()
            : base("name=ALRSDB")
        {
        }
        
        public DbSet<ALRS> ALRS { get; set; }
        public DbSet<RequestorPosition> RequestorPosition { get; set; }
        public DbSet<CalendarData> tblCalendarData { get; set; }
        public DbSet<ApprovalList> tblApprovalList { get; set; }
        public DbSet<Log> tblLog { get; set; }
        public DbSet<Access> tblAccess { get; set; }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            //modelBuilder.Entity<Correspondance>()
            //.HasOptional(WillCascadeOnDelete(false);

        }

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


    }
}

