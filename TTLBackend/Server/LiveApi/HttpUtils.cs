using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using QuazalWV;

namespace TTLBackend
{
    public static class HttpUtils
    {
        public static string ReadBody(HttpListenerRequest req)
        {
            Stream inputStream = req.InputStream;
            Encoding encoding = req.ContentEncoding;
            StreamReader reader = new StreamReader(inputStream, encoding);
            string body = reader.ReadToEnd();
            inputStream.Close();
            reader.Close();
            return body;
        }

        public static HttpListenerResponse WriteBody(HttpListenerResponse res, string bodyStr)
        {
            res.ContentEncoding = Encoding.UTF8;
            Stream outputStream = res.OutputStream;
            byte[] body = res.ContentEncoding.GetBytes(bodyStr);
            try
            {
                outputStream.Write(body, 0, body.Length);
            }
            catch(Exception e)
            {
                Log.WriteLine(1, $"[Live API] Serialization error:\n{e}", Color.Red);
            }
            outputStream.Close();
            return res;
        }

        public static HttpListenerResponse Write404(HttpListenerResponse res)
        {
            string html = "<html><body><h1>Error 404</h1><p>The requested page was not found.</p></body></html>";
            return WriteBody(res, html);
        }
    }
}
