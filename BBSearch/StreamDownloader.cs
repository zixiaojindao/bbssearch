using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
namespace BBSSearch
{
    class StreamDownloader
    {
        public static void DownloadFile(string name, Stream st)
        {
            int bufferSize = 2048;
            byte[] buffer = new byte[bufferSize];
            BinaryReader sr = new BinaryReader(st);
            BinaryWriter sw = new BinaryWriter(new FileStream(name, FileMode.Create, FileAccess.Write));
            int readBytes = -1;
            do
            {
                readBytes = sr.Read(buffer, 0, bufferSize);
                if (readBytes < 0)
                {
                    throw new Exception("receive error");
                }
                sw.Write(buffer, 0, readBytes);
            } while (readBytes != 0);
            sr.Close();
            sw.Close();
            sw.Dispose();
        }

        public static bool DownloadFile(string url, string filePath)
        {
            try
            {
                string URL = url;
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(URL);
                myRequest.KeepAlive = true;
                HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                StreamDownloader.DownloadFile(filePath, myResponse.GetResponseStream());
                myResponse.Close();
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
    }
}
