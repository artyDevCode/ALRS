using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ALRSSystem.Models;

namespace ALRSSystem.Models
{
    /// <summary>
    /// Summary description for EventManager
    /// </summary>
    public class EventManager
    {
        public class Event
        {
            public string Id { get; set; }
            public string Text { get; set; }
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
            public string AllDayEvent { get; set; }
            public string Color { get; set; }
        }


        public DataTable FilteredData(DateTime start, DateTime end)
        {
            DbDataAdapter da = CalendarDb.CreateDataAdapter("SELECT * FROM [ALRS] WHERE NOT (([ALRSEndDate] <= @start) OR ([ALRSStartDate] >= @end))");
            CalendarDb.AddParameterWithValue(da.SelectCommand, "start", start);
            CalendarDb.AddParameterWithValue(da.SelectCommand, "end", end);

            DataTable dt = new DataTable();
            da.Fill(dt);

            return dt;
        }

        public void EventEdit(string id, string name, DateTime start, DateTime end)
        {
            using (var con = CalendarDb.CreateConnection())
            {
                con.Open();

                var cmd = CalendarDb.CreateCommand("UPDATE [ALRS] SET [ALRSName] = @name, [ALRSStartDate] = @start, [ALRSEndDate] = @end WHERE [ID] = @id", con);
                CalendarDb.AddParameterWithValue(cmd, "id", id);
                CalendarDb.AddParameterWithValue(cmd, "start", start);
                CalendarDb.AddParameterWithValue(cmd, "end", end);
                CalendarDb.AddParameterWithValue(cmd, "name", name);
                cmd.ExecuteNonQuery();
            }
        }

        public void EventMove(string id, DateTime start, DateTime end)
        {
            using (var con = CalendarDb.CreateConnection())
            {
                con.Open();

                var cmd = CalendarDb.CreateCommand("UPDATE [ALRS] SET [ALRSStartDate] = @start, [ALRSEndDate] = @end WHERE [id] = @id", con);
                CalendarDb.AddParameterWithValue(cmd, "id", id);
                CalendarDb.AddParameterWithValue(cmd, "start", start);
                CalendarDb.AddParameterWithValue(cmd, "end", end);
                cmd.ExecuteNonQuery();
            }

        }

        public Event Get(string id)
        {
            var da = CalendarDb.CreateDataAdapter("SELECT * FROM [ALRS] WHERE ID = @id");
            CalendarDb.AddParameterWithValue(da.SelectCommand, "id", id);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                return new Event
                {
                    Id = id,
                    Text = (string)dr["ALRSName"],
                    Start = (DateTime)dr["ALRSStartDate"],
                    End = (DateTime)dr["ALRSEndDate"],
                    AllDayEvent=(String)dr["ALRSAllDayEvent"],
                    Color = (String)dr["ALRSColor"],
                };
            }
            return null;
        }

        public void EventCreate(DateTime start, DateTime end, string name)
        {
            using (var con = CalendarDb.CreateConnection())
            {
                con.Open();

                var cmd = CalendarDb.CreateCommand("INSERT INTO [ALRS] (ALRSStartDate, ALRSEndDate, ALRSName) VALUES (@start, @end, @name)", con);
                CalendarDb.AddParameterWithValue(cmd, "start", start);
                CalendarDb.AddParameterWithValue(cmd, "end", end);
                CalendarDb.AddParameterWithValue(cmd, "name", name);
                cmd.ExecuteNonQuery();
            }
        }


        public void EventDelete(string id)
        {
            using (var con = CalendarDb.CreateConnection())
            {
                con.Open();

                var cmd = CalendarDb.CreateCommand("DELETE FROM [event] WHERE id = @id", con);
                CalendarDb.AddParameterWithValue(cmd, "id", id);
                cmd.ExecuteNonQuery();

            }
        }
    }

}