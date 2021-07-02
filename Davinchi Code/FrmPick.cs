using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Davinchi_Code
{
    public partial class FrmPick : Form
    {
        private FrmGame frm = null;


        public FrmPick(FrmGame frm)
        {
            InitializeComponent();
            this.frm = frm;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            if (frm.getW() > 0)
            {
                frm.send_msg("DC%get%" + frm.getmNum().ToString() + "%W");
                this.Close();
            }
            else
                MessageBox.Show("하얀색 카드가 남아있지 않습니다.");
        }

        private void label2_Click(object sender, EventArgs e)
        {
            if (frm.getB() > 0)
            {
                frm.send_msg("DC%get%" + frm.getmNum().ToString() + "%B");
                this.Close();
            }
            else
                MessageBox.Show("검은색 카드가 남아있지 않습니다.");
        }

    }
}
