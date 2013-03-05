using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Windows.Forms;

namespace BBSSearch
{
    class TimeSortedPostEntry:IComparer<PostEntry>
    {
        public int Compare(PostEntry a, PostEntry b)
        {
            return -(a.Time.CompareTo(b.Time));
        }
    }
    class BoardNameSortedPostEntry : IComparer<PostEntry>
    {
        public int Compare(PostEntry a, PostEntry b)
        {
            return (a.BoardName.CompareTo(b.BoardName));
        }
    }
    class PostEntry
    {
        public string TextID;
        public string ID;
        public string Title;
        public string Time;
        public string BoardName;
        public string BoardID;
        public PostEntry(string textID, string id, string title, string time, string boardName, string boardID)
        {
            TextID = textID;
            ID = id;
            //if (title.Length > 15)
            //{
            //    Title = title.Substring(0, 15) + "...";
            //}
            //else
            {
                Title = title;
            }
            Time = time;
            BoardName = boardName;
            BoardID = boardID;
        }
        public override string ToString()
        {
            return TextID + "\t" + BoardName + "\t\t" + Title + "\t" + Time ;
        }
    }
    class BoardPost
    {
        public string BoardName;
        public string BoardID;
        public List<PostEntry> Posts;
        public BoardPost(string boardName, string boardID)
        {
            BoardName = boardName;
            BoardID = boardID;
            Posts = new List<PostEntry>();
        }
        public void Add(PostEntry pe)
        {
            Posts.Add(pe);
        }
    }
    class BoardPostParser
    {
        BoardInfo m_allBoards;
        List<BoardPost> m_allBoardPost;
        int count = 0;
        public List<BoardPost> AllBoardPost
        {
            get { return m_allBoardPost; }
        }
        public BoardPostParser(BoardInfo binfo)
        {
            m_allBoards = binfo;
            m_allBoardPost = new List<BoardPost>();
        }
        public List<BoardPost> Parse()
        {
            foreach(BoardEntry be in m_allBoards.AllBoard)
            {
                string path = Global.WorkDir + be.boardName + "_Post.xml";
                PareseSinglePath(be.boardName, be.boardID, path);
            }
            return m_allBoardPost;
        }
        private void PareseSinglePath(string boardName, string boardID, string path)
        {
            try
            {
                BoardPost bpost = new BoardPost(boardName, boardID);
                //path = @"C:\Users\SunZhao\Desktop\workspace\C_Post.xml";
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlElement element = doc["bbsbfind"];
                if (element == null)
                {
                    return;
                }
                for (int i = 0; i < element.ChildNodes.Count; i++)
                {
                    XmlNode xn = element.ChildNodes[i];
                    if (xn.Name == "po")
                    {

                        XmlAttribute time = xn.Attributes["time"];
                        XmlAttribute id = xn.Attributes["id"];
                        string title = xn.InnerText;
                        PostEntry pe = new PostEntry((++count).ToString(), id.Value, title, time.Value , boardName, boardID);
                        bpost.Add(pe);
                    }
                }
                if (bpost.Posts.Count > 0)
                {
                    m_allBoardPost.Add(bpost);
                }
            }
            catch(Exception e)
            {
                //MessageBox.Show(e.Message);
            }
           
        }
    }
}
