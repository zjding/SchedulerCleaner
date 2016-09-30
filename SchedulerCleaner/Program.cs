using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerCleaner
{
    class Program
    {
        static string connectionString;

        static void Main(string[] args)
        {
            SetConnectionString();
            SqlConnection cn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cn.Open();
            string sqlString;

            List<string> taskNames = new List<string>();

            sqlString = @"SELECT TaskName FROM Tasks WHERE Completed = 1";

            cmd.CommandText = sqlString;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                taskNames.Add(Convert.ToString(reader["TaskName"]));
            }
            reader.Close();

            TaskService ts = new TaskService();

            foreach (string taskName in taskNames)
            {
                ts.RootFolder.DeleteTask(taskName);
            }

            sqlString = @"DELETE FROM Tasks WHERE Completed = 1";
            cmd.CommandText = sqlString;
            cmd.ExecuteNonQuery();

            cn.Close();
        }

        private static void SetConnectionString()
        {
            string azureConnectionString = "Server=tcp:zjding.database.windows.net,1433;Initial Catalog=Costco;Persist Security Info=False;User ID=zjding;Password=G4indigo;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            SqlConnection cn = new SqlConnection(azureConnectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;

            cn.Open();
            string sqlString = "SELECT ConnectionString FROM DatabaseToUse WHERE bUse = 1 and ApplicationName = 'Crawler'";
            cmd.CommandText = sqlString;
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    connectionString = (reader.GetString(0)).ToString();
                }
            }
            reader.Close();
            cn.Close();
        }
    }
}
