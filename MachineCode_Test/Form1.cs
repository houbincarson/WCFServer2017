using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WcfService;

namespace MachineCode_Test
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
    }

    private void btnGet_Click(object sender, EventArgs e)
    {
      textBox1.Text =
      MachineCode.getInstance().Hash;
    }
  }
}
//京华   MF7097G06704851-BFEBFBFF0001067A BFEBFBFF0001067A BFEBFBFF0001067A BFEBFBFF0001067A
//美意年 ..CN708217C350M1.-BFEBFBFF000006F2 BFEBFBFF000006F2
//全城热恋 109633400000162-BFEBFBFF000106E5 BFEBFBFF000106E5 BFEBFBFF000106E5 BFEBFBFF000106E5
