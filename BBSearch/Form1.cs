using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Microsoft.Win32;
using System.Diagnostics;

namespace BBSSearch
{
    public partial class Form1 : Form
    {
        BBSClient UserClient;
        BoardInfo bbsBoards;
        List<PostEntry> cur_Res;
        public Form1()
        {
            InitializeComponent();
            bbsBoards = new BoardInfo();
            progressBar1.Maximum = bbsBoards.AllBoard.Count;
            UserClient = new BBSClient("RoundRobin", "891021", bbsBoards);
            cur_Res = new List<PostEntry>();
            progressBar1.Visible = false;
        }
        public void ShowProgress(int step)
        {
            progressBar1.Increment(step);
        }
        private void ShowResOnListBox(List<PostEntry> res)
        {
            listBox1.Items.Clear();
            foreach (PostEntry pe in res)
            {
                listBox1.Items.Add(pe);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            progressBar1.Visible = true;
            UserClient.GetPostByBoard(textBox_UserName.Text.Trim(), textBox_Time_Limit.Text.Trim(), this);
            Thread showResThread = new Thread(new ThreadStart(ShowResult));
            showResThread.Start();
        }
        private void ShowResult()
        {
            while (progressBar1.Value != progressBar1.Maximum)
            {
                Thread.Sleep(2000);
            }
            progressBar1.Value = 0;
            progressBar1.Visible = false;
            BoardPostParser bpp = new BoardPostParser(bbsBoards);
            cur_Res.Clear();
            List<BoardPost> res = bpp.Parse();
            foreach (BoardPost bp in res)
            {
                foreach (PostEntry pe in bp.Posts)
                {
                    cur_Res.Add(pe);
                    listBox1.Items.Add(pe);
                }
            }
            ShowResOnListBox(cur_Res);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (UserClient.TryLogIn())
            {
                MessageBox.Show("login success");
            }
            else
            {
                MessageBox.Show("login failure");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            UserClient.TryLogOut();
            MessageBox.Show("log out success");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            List<PostEntry> newRes = new List<PostEntry>(cur_Res);
            newRes.Sort(new TimeSortedPostEntry());
            int count = 0;
            foreach(PostEntry pe in newRes)
            {
                pe.TextID = (++count).ToString();
            }
            ShowResOnListBox(newRes);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ShowResOnListBox(cur_Res);
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            PostEntry sel = (PostEntry)listBox1.SelectedItem;
            string URL = "\"http://bbs.fudan.edu.cn/bbs/con?new=1";
            URL += ("&bid=" + sel.BoardID);
            URL += ("&f=" + sel.ID);
            URL += "\"";
            Process.Start("explorer.exe", URL);
        }
    }
}
