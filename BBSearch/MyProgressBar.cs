using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BBSSearch
{
    public interface IObserve
    {
        void Increment(int n);
    }
    public class MyProgressBar : IObserve
    {
        ProgressBar m_pro;
        public MyProgressBar(ProgressBar pro, int total)
        {
            m_pro = pro;
            m_pro.Maximum = total;
        }
        public void Increment(int n)
        {
            m_pro.Increment(n);
        }
    }
}
