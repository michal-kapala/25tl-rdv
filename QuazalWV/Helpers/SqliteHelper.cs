using System.Collections.Generic;
using System.Data.SQLite;

namespace QuazalWV
{
    /// <summary>
    /// Helper methods specific to SQLite database.
    /// </summary>
    public static class SqliteHelper
    {
        public static SQLiteConnection Connection { get; set; }

        public static void Init()
        {
            Connection = new SQLiteConnection
            {
                ConnectionString = "Data Source=database.sqlite"
            };
            Connection.Open();
            Log.WriteLine(1, "DB loaded...");
        }

        public static List<List<string>> GetQueryResults(string query)
        {
            List<List<string>> result = new List<List<string>>();
            SQLiteCommand command = new SQLiteCommand(query, Connection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                List<string> entry = new List<string>();
                for (int i = 0; i < reader.FieldCount; i++)
                    entry.Add(reader[i].ToString());
                result.Add(entry);
            }
            reader.Close();
            reader.Dispose();
            command.Dispose();
            return result;
        }
    }
}
