using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Калина
{
    public partial class Form4 : Form
    {
        Label[] label;
        string richTextBox1;
        int var_data_length;
        string[] Rkeys;
        public Form4(int datal, string rb, string[] rk)
        {
            richTextBox1 = rb;
            var_data_length = datal;
            Rkeys = rk;

            InitializeComponent();

            Label[] l = { label1, label2, label3, label4, label5,label6,label7,label8,label9, label10,
                label11, label12, label13, label14, label15,label16,label17,label18,label19, label20,
                label21, label22, label23, label24, label25,label26,label27,label28,label29, label30,
                label31, label32, label33, label34, label35,label36,label37,label38,label39, label40,
                label41, label42, label43, label44, label45,label46,label47,label48,label49, label50,
                label51, label52, label53, label54, label55,label56,label57,label58,label59, label60,
                label61, label62, label63, label64, label65,label66,label67,label68,label69, label70,
                label71, label72, label73, label74, label75,label76,label77,label78,label79, label80};

            Label[] ll = { label87, label88, label89, label90, label91,label92,label93,label94,label95,
                    label96,label97,label98,label99,label100,label101,label102};
            foreach (Label l1 in ll)
            {
                l1.Text = "";
            }
            foreach (Label ll1 in l)
            {
                ll1.Text = "";
            }
            label103.Text = "";
            label81.Text = "";
            label82.Text = "";
            label83.Text = "";
            label84.Text = "";
            label85.Text = "";
            label86.Text = "";
            label = l;
        }

        int r = 0;

        byte[,] mydata;
        byte[,] buffer;
        int t = 0;
        private void button1_Click(object sender, EventArgs e)
        {



            if (r < 11)
            {
                label103.Text = "Раунд " + (r + 1).ToString();
                label81.Text = "";
                label82.Text = "";
                label83.Text = "";
                label84.Text = "";
                label85.Text = "";
                int clabel = 0;
                foreach (Label ll in label)
                {
                    ll.Text = "";
                }
                State key = new State();
                int cols = 0;
                switch (var_data_length)
                {
                    case 128:
                        cols = 2;
                        break;
                    case 256:
                        cols = 4;
                        break;
                    case 512:
                        cols = 8;
                        break;
                }
                if (r == 0)
                {
                    int bl_l = var_data_length / 4;
                    string text1 = richTextBox1;
                    if (text1.Length % bl_l != 0)
                    {
                        if (text1.Length < bl_l)
                        {
                            int dop = bl_l - text1.Length;
                            text1 += "10";
                            for (int i = 0; i < dop - 1; i++)
                            {
                                text1 += "00";
                            }
                        }
                        else
                        {
                            int dop = text1.Length % bl_l;
                            text1 += "1";
                            for (int i = 0; i < 32 - dop - 1; i++)
                            {
                                text1 += "0";
                            }
                        }
                    }

                    int kblock = (int)(text1.Length / bl_l);

                    string[] bl = new string[kblock];

                    int blk = 0;

                    mydata = new byte[8, cols];
                    buffer = new byte[8, cols];
                    for (int l = 0; l < kblock; l++)
                    {
                        bl[l] = text1.Substring(blk, 16 * cols);
                        blk += 32;
                    }

                    numHex nh = new numHex();



                    int tt = 0;
                    int nn = 0;
                    for (int j = 0; j < cols; j++)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            mydata[i, j] = (byte)nh.ConvertInt(bl[t].Substring(nn, 2));
                            nn += 2;

                        }
                    }


                    label86.Text = "Вхідні дані";
                    int cll = 0;
                    Label[] ll = { label87, label88, label89, label90, label91,label92,label93,label94,label95,
                    label96,label97,label98,label99,label100,label101,label102};
                    foreach (Label l in ll)
                    {
                        l.Text = "";
                    }
                    for (int j = cols - 1; j >= 0; j--)
                    {
                        for (int i = 7; i >= 0; i--)
                        {
                            string s = mydata[i, j].ToString("x");
                            if (s.Length < 2)
                            {
                                s = "0" + s;
                            }
                            ll[cll].Text += s;
                            cll++;
                        }
                    }
                }
                string[] buff = Rkeys[r].Split('/');
                int[] num = new int[buff.Length];
                int c = 0;
                foreach (string s1 in buff)
                {
                    if (s1 != "")
                    {
                        num[c] = Int32.Parse(s1);
                        c++;
                    }
                }
                #region keys
                byte[,] data = new byte[8, cols];
                c = 0;
                for (int j = 0; j < cols; j++)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        data[i, j] = (byte)num[c];
                        c++;
                    }
                }
                label81.Text = "Раундовий ключ";
                for (int j = 0; j < cols; j++)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        string s = data[i, j].ToString("x");
                        if (s.Length < 2)
                        {
                            s = "0" + s;
                        }
                        label[clabel].Text += s;
                        clabel++;
                    }
                }
                #endregion
                if (r == 0)
                {
                    clabel = 64;
                    buffer = key.addNumbers(data, mydata, cols);
                    label85.Text = "ADD_RKEY";
                    for (int j = 0; j < cols; j++)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            string s = buffer[i, j].ToString("x");
                            if (s.Length < 2)
                            {
                                s = "0" + s;
                            }
                            label[clabel].Text += s;
                            clabel++;
                        }
                    }
                }
                else if (r == Rkeys.Length - 1)
                {
                    label82.Text = "S_BOX";
                    buffer = key.sBoxes(buffer, cols);
                    for (int j = 0; j < cols; j++)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            string s = buffer[i, j].ToString("x");
                            if (s.Length < 2)
                            {
                                s = "0" + s;
                            }
                            label[clabel].Text += s;
                            clabel++;
                        }
                    }
                    label83.Text = "S_ROW";
                    buffer = key.S_Rows(buffer, cols);
                    for (int j = 0; j < cols; j++)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            string s = buffer[i, j].ToString("x");
                            if (s.Length < 2)
                            {
                                s = "0" + s;
                            }
                            label[clabel].Text += s;
                            clabel++;
                        }
                    }
                    label84.Text = "M_COL";
                    buffer = key.M_Columns(buffer, cols);
                    for (int j = 0; j < cols; j++)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            string s = buffer[i, j].ToString("x");
                            if (s.Length < 2)
                            {
                                s = "0" + s;
                            }
                            label[clabel].Text += s;
                            clabel++;
                        }
                    }
                    label85.Text = "Зашифровані дані (ADD_RKEY)";
                    buffer = key.addNumbers(data, buffer, cols);


                    for (int j = 0; j < cols; j++)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            string s = buffer[i, j].ToString("x");
                            if (s.Length < 2)
                            {
                                s = "0" + s;
                            }
                            label[clabel].Text += s;
                            clabel++;
                        }
                    }
                    // richTextBox2.Text += Environment.NewLine;
                }
                else
                {
                    label82.Text = "S_BOX";
                    buffer = key.sBoxes(buffer, cols);
                    for (int j = 0; j < cols; j++)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            string s = buffer[i, j].ToString("x");
                            if (s.Length < 2)
                            {
                                s = "0" + s;
                            }
                            label[clabel].Text += s;
                            clabel++;
                        }
                    }
                    label83.Text = "S_ROW";
                    buffer = key.S_Rows(buffer, cols);
                    for (int j = 0; j < cols; j++)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            string s = buffer[i, j].ToString("x");
                            if (s.Length < 2)
                            {
                                s = "0" + s;
                            }
                            label[clabel].Text += s;
                            clabel++;
                        }
                    }
                    label84.Text = "M_COL";
                    buffer = key.M_Columns(buffer, cols);
                    for (int j = 0; j < cols; j++)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            string s = buffer[i, j].ToString("x");
                            if (s.Length < 2)
                            {
                                s = "0" + s;
                            }
                            label[clabel].Text += s;
                            clabel++;
                        }
                    }
                    label85.Text = "XOR";
                    buffer = key.xor_rkey(buffer, data, cols);
                    for (int j = 0; j < cols; j++)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            string s = buffer[i, j].ToString("x");
                            if (s.Length < 2)
                            {
                                s = "0" + s;
                            }
                            label[clabel].Text += s;
                            clabel++;
                        }
                    }
                }
                r++;





            }
            else
            {
                t++;
                r = 0;
            }
        }

    }
}

