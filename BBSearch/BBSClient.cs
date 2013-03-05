using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Security.Policy;
using System.Threading;
using System.Windows.Forms;

namespace BBSSearch
{
    class BBSClient
    {
        string usrName;
        string password;
        CookieContainer cookieKey;
        BoardInfo bbsBoards;
        public BBSClient(string usr, string pwd, BoardInfo boards)
        {
            usrName = usr;
            password = pwd;
            cookieKey = new CookieContainer();
            bbsBoards = boards;
        }
        public bool TryLogIn()
        {
            //cookieKey.Add(new Cookie("id", "sunzhao","","bbs.fudan.edu.cn"));
            try
            {
                CookieContainer logCookie = new CookieContainer();
                ASCIIEncoding encoding = new ASCIIEncoding();
                string postData = "id=" + usrName;
                postData += ("&pw=" + password);
                postData += "&ref=/bbs/sec";
                byte[] data = encoding.GetBytes(postData);
                HttpWebRequest myRequest =
                    (HttpWebRequest)WebRequest.Create("http://bbs.fudan.edu.cn/bbs/login");
                myRequest.Method = "POST";
                myRequest.ContentType = "application/x-www-form-urlencoded";
                myRequest.ContentLength = data.Length;
                myRequest.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)";
                myRequest.CookieContainer = logCookie;
                myRequest.KeepAlive = true;
                myRequest.AllowAutoRedirect = true;

                Stream newStream = myRequest.GetRequestStream();
                newStream.Write(data, 0, data.Length);
                newStream.Close();
                HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                foreach (Cookie item in logCookie.GetCookies(myRequest.RequestUri))
                {
                    //Console.WriteLine(item.Name + "\t" + item.Value);
                    cookieKey.Add(new Cookie(item.Name, item.Value, "", "bbs.fudan.edu.cn"));
                }
                StreamDownloader.DownloadFile(Global.WorkDir + "login.html", myResponse.GetResponseStream());

                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }
        public bool TryLogOut()
        {
            try
            {
                string URL = "http://bbs.fudan.edu.cn/bbs/logout";
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(URL);
                myRequest.CookieContainer = cookieKey;
                myRequest.KeepAlive = true;
                HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                StreamDownloader.DownloadFile(Global.WorkDir + "logout.xml", myResponse.GetResponseStream());
                myResponse.Close();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }  
        }
        public void GetPostByBoard(string UserName, string timeLimit, Form1 mainForm)
        {
            int threadNum = 20;
            Thread[] CreateThread = new Thread[threadNum];
            UserPostSearcher[] tp = new UserPostSearcher[threadNum];
            BoardEntry[] boards = bbsBoards.AllBoard.ToArray();
            int taskNumPerThread = boards.Length;
            if (threadNum > 1)
            {
                taskNumPerThread = boards.Length / (threadNum - 1);
            }

            int offset = 0;
            for (int i = 0; i < threadNum - 1; ++i)
            {
                tp[i] = new UserPostSearcher(cookieKey, boards, offset, taskNumPerThread, UserName, timeLimit, mainForm);
                CreateThread[i] =
                 new Thread(new ThreadStart(tp[i].GetPostByBoard));
                CreateThread[i].Start();
                offset += taskNumPerThread;
            }
            int index = threadNum - 1;
            int leftLength = boards.Length - (threadNum - 1) * taskNumPerThread;
            UserPostSearcher tp1 = new UserPostSearcher(cookieKey, boards, offset, leftLength, usrName, timeLimit, mainForm);
            CreateThread[index] =
             new Thread(new ThreadStart(tp1.GetPostByBoard));
            CreateThread[index].Start();      
        }
       
    }
}
