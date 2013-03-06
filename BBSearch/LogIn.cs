using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BBSSearch
{
    public partial class LogIn : Form
    {
        public string userName;
        public string password;
        public LogIn()
        {
            InitializeComponent();
            userName = "";
            password = "";
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            userName = textBoxUserName.Text.Trim();
            password = textBoxPassword.Text.Trim();
            Close();
        }
    }
}
