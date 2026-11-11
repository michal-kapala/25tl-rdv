using System;
using System.Collections.Generic;
using System.Drawing;

namespace QuazalWV
{
    public class MessageOfTheDay
    {
        public string Message { get; set; }
        public string Language { get; set; }

        public MessageOfTheDay(string lang)
        {
            _ = new List<List<string>>();
            Language = lang;
            try
            {
                List<List<string>> results = SqliteHelper.GetQueryResults($"SELECT msg FROM message WHERE language='{lang}'");
                Message = results[0][0];
            }
            catch(Exception e)
            {
                Message = "Error retrieving message of the day :(";
                Log.WriteLine(1, $"[Eidos RMC] {e}", Color.Red);
            }
        }
    }
}
