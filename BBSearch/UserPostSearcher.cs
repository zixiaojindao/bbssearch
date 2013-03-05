using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;

namespace BBSSearch
{
    class UserPostSearcher
    {
        string searchUserName;
        string limit;
        BoardEntry[] boards;
        int offset;
        int length;
        CookieContainer cookieKey;
        Form1 mainForm;
        public delegate void IncrementProgressDelegate(int step);
        public UserPostSearcher(CookieContainer coo, BoardEntry[] bs, int off, int len, string uname , string lim, Form1 mF)
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
