using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Web;
namespace BBSSearch
{
    class UserPostSearcher
    {
        string searchUserName;
        string limit;
        string keyword1;
        string keyword2;
        BoardEntry[] boards;
        int offset;
        int length;
        CookieContainer cookieKey;
        Form1 mainForm;
        public delegate void IncrementProgressDelegate(int step);
        public UserPostSearcher(CookieContainer coo, BoardEntry[] bs, int off, int len, string uname , string lim, string k1, string k2, Form1 mF)
        {
            cookieKey = coo;
            boards = bs;
            offset = off;
            length = len;
            if(lim == "")
            {
                limit = "100";
            }
            else
            {
                limit = lim;
            }
           
            searchUserName = uname;
            mainForm = mF;
            keyword1 = k1;
            keyword2 = k2;
        }
        public void GetPostByBoard()
        {
            for (int i = 0; i < length; ++i)
            {
                string boardName = boards[offset].boardName;
                string boardID = boards[offset].boardID;
                Console.WriteLine(boardName);
                string URL = "http://bbs.fudan.edu.cn/bbs/bfind?";//432&t1=&t2=&t3=&user=gifed&limit=50";
                URL += ("bid=" + boardID);
                URL += ("&t1=" + System.Web.HttpUtility.UrlEncode(keyword1, System.Text.Encoding.GetEncoding("GB2312")));
                URL += ("&t2=" + System.Web.HttpUtility.UrlEncode(keyword2, System.Text.Encoding.GetEncoding("GB2312")));
                URL += ("&t3=" + "");
                URL += ("&user=" + searchUserName);
                URL += ("&limit=" + limit);
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(URL);
                myRequest.CookieContainer = cookieKey;
                myRequest.KeepAlive = true;
                HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                StreamDownloader.DownloadFile(Global.WorkDir + boardName + "_Post.xml", myResponse.GetResponseStream());
                myResponse.Close();
                //Thread.Sleep(1000);
                ++offset;
                IncrementProgressDelegate IncrementProgress = new IncrementProgressDelegate(mainForm.ShowProgress);
                mainForm.Invoke(IncrementProgress, new object[] { 1 });  
                
            }
        }
    }
}
