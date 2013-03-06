using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Xml;
using System.Threading.Tasks;

namespace BBSSearch
{
    class BoardEntry
    {
        public string boardName;
        public string boardID;
        public BoardEntry(string bn, string bid)
        {
            boardName = bn;
            boardID = bid;
        }
    }
    class BoardInfo
    {
        List<BoardEntry> m_allBoard;
        string m_welcomePicURL;
        public BoardInfo()
        {
            m_welcomePicURL = "";
            LoadWelcomePicture();
            if (LoadAllBoard() == false)
            {
                m_allBoard = null;
            }
        }
        public List<BoardEntry> AllBoard
        {
            get { return m_allBoard; }
        }

        public string WelcomePicURL
        {
            get { return m_welcomePicURL; }
        }
        public bool LoadAllBoard()
        {
            m_allBoard = new List<BoardEntry>();
            return LoadAllBoardOnLine();
        }

        private void LoadWelcomePicture()
        {
            string url = "http://bbs.fudan.edu.cn";
            StreamDownloader.DownloadFile(url, Global.WorkDir + "welcome.html");
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(Global.WorkDir + "welcome.html", Encoding.Default);
                string line = "";
                while (sr.EndOfStream == false)
                {
                    line = sr.ReadLine();
                    if (line.Contains("<img src") == true)
                    {
                        m_welcomePicURL = line.Substring(line.LastIndexOf("/"), line.Length - line.LastIndexOf("/") - 2);
                        m_welcomePicURL = "http://bbs.fudan.edu.cn/images/home" + m_welcomePicURL;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                m_welcomePicURL = "";
            }
        }
        private void LoadAllBoardOffline()
        {
            string path = Global.WorkDir + Global.BoardInfoFileName;
            StreamReader sr = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read), System.Text.Encoding.GetEncoding("gb18030"));
            string line;
            string[] word;
            while(!sr.EndOfStream)
            {
                line = sr.ReadLine();
                word = line.Split();
                m_allBoard.Add(new BoardEntry(word[0], word[1]));
            }
            sr.Close();
        }

        private bool DownloadAllBoardName(string url, string boardListFile)
        {
            try
            {
                string URL = url;
                StreamDownloader.DownloadFile(URL, Global.WorkDir + boardListFile);
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        private List<string> ParseAllBoardName(string url, string boardListFile, bool root = false)
        {
            if (DownloadAllBoardName(url, boardListFile) == false)
            {
                return null;
            }
            List<string> res = new List<string>();
            try
            {
                List<string> allBoardName = new List<string>();
                List<string> allBoardID = new List<string>();
                string path = Global.WorkDir + boardListFile;
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlElement element;
                if (root == true)
                {
                   element = doc["bbsall"];
                }
                else
                {
                    element = doc["bbsboa"];
                }
                if (element == null)
                {
                    return null;
                }
                for (int i = 0; i < element.ChildNodes.Count; i++)
                {
                    XmlNode xn = element.ChildNodes[i];
                    if (xn.Name == "brd")
                    {
                        XmlAttribute title = xn.Attributes["title"];
                        XmlAttribute dir = xn.Attributes["dir"];
                        if (dir.Value == "0")
                        {
                            res.Add(title.Value);
                        }
                        else
                        {
                            string URL = "http://bbs.fudan.edu.cn/bbs/doc?board=" + title.Value;
                            List<string> subBoardNameList = ParseAllBoardName(URL, title.Value + ".xml");
                            if (subBoardNameList != null)
                            {
                                res.AddRange(subBoardNameList);
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                return null;
            }
            return res;
        }

        private string ParseBoardIDByName(string bdn)
        {
            string bid = "-1";
            string URL = "http://bbs.fudan.edu.cn/bbs/doc?board=" + bdn;
            StreamDownloader.DownloadFile(URL, Global.WorkDir + bdn + ".xml");

            List<string> allBoardName = new List<string>();
            List<string> allBoardID = new List<string>();
            string path = Global.WorkDir + bdn + ".xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlElement element = doc["bbsdoc"];
            if (element == null)
            {
                return "-1";
            }
            for (int i = 0; i < element.ChildNodes.Count; i++)
            {
                XmlNode xn = element.ChildNodes[i];
                if (xn.Name == "brd")
                {
                    XmlAttribute boardID = xn.Attributes["bid"];
                   
                    bid = boardID.Value;
                    break;
                }
            }
            return bid;

        }
        private List<string> ParseAllBoardID(List<string> allBoardName)
        {
            List<string> allBoardID = new List<string>();
            string bid = "-1";
            try
            {
                //foreach (string bdn in allBoardName)
                Parallel.For(0, allBoardName.Count, i =>
                {
                    bid = ParseBoardIDByName(allBoardName[i]);
                    if (bid != "-1")
                    {
                        allBoardID.Add(bid);
                    }
                });
            }
            catch (Exception e)
            {
                return null;
            }
            return allBoardID;
        }

        private bool LoadAllBoardOnLine()
        {
            
            List<string> allBoardName = ParseAllBoardName("http://bbs.fudan.edu.cn/bbs/all", "bbsall.xml", true);
            if (allBoardName == null)
            {
                return false;
            }
            List<string> allBoardID = ParseAllBoardID(allBoardName);
            if (allBoardID == null)
            {
                return false;
            }
            if(allBoardName.Count != allBoardID.Count)
            {
                return false;
            }
            for (int i = 0; i < allBoardID.Count; ++i)
            {
                m_allBoard.Add(new BoardEntry(allBoardName[i], allBoardID[i]));
            }
            return true;
        }
        
    }
}
