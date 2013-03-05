using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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
        public BoardInfo()
        {
            m_allBoard = new List<BoardEntry>();
            LoadAllBoard();
        }
        private void LoadAllBoard()
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
        public List<BoardEntry> AllBoard
        {
            get { return m_allBoard; }
        }
    }
}
