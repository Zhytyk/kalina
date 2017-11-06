using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;
using System.IO;

namespace Калина
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            radioButton1.Checked = true;
            radioButton4.Checked = true;
            radioButton7.Checked = true;
            button2.Enabled = false;
            button3.Enabled = false;
        }
        public string global_message = "", counter = "";
        public int var_key_length = 256, var_data_length = 256, blocks_num;

        public static byte GMul(Byte a, Byte b)
        {
            Byte p = 0;
            Byte counter;
            Byte hi_bit_set;
            for (counter = 0; counter < 8; counter++)
            {
                if ((b & 1) == 1)
                {
                    p ^= a;
                }
                hi_bit_set = (Byte)(a & 0x80);
                a <<= 1;
                if (hi_bit_set != 0)
                {
                    a ^= 0x1d; /* x^8 + x^4 + x^3 + x^2 + 1 */
                }
                b >>= 1;
            }
            return p;
        }

        public int blocks_count(string message, int data_length, bool is_hex)
        {
            int blocks_num = 0;

            if (is_hex)
            {
                if (message.Length % (data_length / 4) == 0)
                {
                    if (message.Length == 0)
                    {
                        blocks_num = 0;
                    }
                    else
                    {
                        blocks_num = (int)(message.Length / (data_length / 4));
                    }
                }
                else
                {
                    blocks_num = (int)(message.Length / (data_length / 4)) + 1;

                    while (message.Length != blocks_num * (data_length / 4))
                    {
                        message = "0" + message;
                    }
                }
            }
            else
            {
                if (message.Length % (data_length / 8) == 0)
                {
                    if (message.Length == 0)
                    {
                        blocks_num = 0;
                    }
                    else
                    {
                        blocks_num = (int)(message.Length / (data_length / 8));
                    }
                }
                else
                {
                    blocks_num = (int)(message.Length / (data_length / 8)) + 1;

                    while (message.Length != blocks_num * (data_length / 8))
                    {
                        message += '\u00ac'.ToString();
                    }

                }
            }

            global_message = message;
            return blocks_num;
        }

        public string padding(string data, int length)
        {
            string temp = data;

            if ((bool)radioButton7.Checked)
            {
                while (temp.Length != (length / 4))
                {
                    temp = "0" + temp;
                }
            }
            if ((bool)radioButton8.Checked)
            {
                while (temp.Length != (length / 8))
                {
                    temp += '\u00ac'.ToString();
                }
            }

            return temp;
        }

        public State[] Text_Message_Dividing(string message, int data_length, int blocks_num)
        {
            State[] blocks = new State[blocks_num];

            for (int i = 0; i < blocks_num; i++)
            {
                blocks[i] = new State(data_length);
                blocks[i].add_text(Encoding.Default.GetBytes(message.Substring((i * (data_length / 8)), data_length / 8)));
            }

            return blocks;
        }
        public State[] Hex_Message_Dividing(string message, int data_length, int blocks_num)
        {
            State[] blocks = new State[blocks_num];

            for (int i = 0; i < blocks_num; i++)
            {
                blocks[i] = new State(data_length);
                blocks[i].add_hex(message.Substring((i * (data_length / 4)), data_length / 4));
            }

            return blocks;
        }

        private string Encrypt_ECB(string message, State[] round_keys, int data_length, int key_length, RichTextBox richTextBox2)
        {
            int blocks_num = blocks_count(message, data_length, (bool)radioButton7.Checked);
            State[] message_div = new State[blocks_num];
            string temp = "";

            if (radioButton7.Checked == true)
            {
                message_div = Hex_Message_Dividing(global_message, data_length, blocks_num);
            }
            else
            {
                message_div = Text_Message_Dividing(global_message, data_length, blocks_num);
            }

            for (int i = 0; i < blocks_num; i++)
            {
                //  message_div[i].Encrypt(round_keys, key_length);

                if (radioButton7.Checked == true)
                {
                    temp += message_div[i].tostring();
                }
                else
                {
                    temp += Encoding.Default.GetString(message_div[i].two_dim_to_one_dim(data_length));
                }
            }

            richTextBox2.Text += temp;

            return temp;
        }

        private string Decrypt_ECB(string ciphertext, State[] round_keys, int data_length, int key_length, RichTextBox richTextBox3)
        {
            int blocks_num = blocks_count(ciphertext, data_length, (bool)radioButton7.Checked);
            State[] message_div = new State[blocks_num];
            string temp = "";

            if (radioButton7.Checked == true)
            {
                message_div = Hex_Message_Dividing(global_message, data_length, blocks_num);
            }
            else
            {
                message_div = Text_Message_Dividing(global_message, data_length, blocks_num);
            }

            for (int i = 0; i < blocks_num; i++)
            {
                // message_div[i].Decrypt(round_keys, key_length);

                if (radioButton7.Checked == true)
                {
                    temp += message_div[i].tostring();
                }
                else
                {
                    temp += Encoding.Default.GetString(message_div[i].two_dim_to_one_dim(data_length));
                }
            }

            richTextBox4.Text += temp;

            return temp;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (radioButton7.Checked == true)
            {
                e.Handled = "0123456789abcdefABCDEF".IndexOf(e.KeyChar) < 0;

                if (textBox1.Text.Length == var_key_length / 4)
                {
                    e.Handled = true;
                }
            }


            /* if (textBox1.Text.Length == var_key_length / 8)
             {
                 e.Handled = true;
             }*/
        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }
        public State key;
        public State[] round_keys;
        byte[,] Kt;
        byte[,] rand;
        private void button4_Click(object sender, EventArgs e)
        {
            textBox2.Clear();
            key = new State(var_key_length);

            if ((bool)radioButton7.Checked)
            {
                key.add_hex(padding(textBox1.Text, var_key_length));
            }
            if ((bool)radioButton8.Checked)
            {
                key.add_text(Encoding.Default.GetBytes(padding(textBox1.Text, var_key_length)));
            }

            rand = key.rozbuv(var_key_length, var_data_length);
            byte[,] b = key.firstSum(var_key_length, var_data_length); // 1
            key.S_boxes(); // 2
            key.Shift_Rows();//3
            key.Mix_Columns();//4
            key.XOR(); // 5
            key.S_boxes(); // 6
            key.Shift_Rows();//7
            key.Mix_Columns();//8
            key.change();//9
            key.S_boxes(); // 10
            key.Shift_Rows();//11
            key.Mix_Columns();//12
            Kt = new byte[8, var_data_length];
            Kt = key.data;
            int cols = 0;
            switch (var_data_length)
            {
                case 128: cols = 2;
                    break;
                case 256: cols = 4;
                    break;
                case 512: cols = 8;
                    break;
            }
            for (int j = 0; j < cols; j++)
            {
                for (int i = 0; i < 8; i++)
                {
                    textBox2.Text += Kt[i, j].ToString("x");
                }
            }

            /*
            textBox1.Text = padding(textBox1.Text, var_key_length);
            //key.show(test_block);
            round_keys = key.key_expanding(var_data_length, var_key_length);

            for (int i = 0; i < round_keys.Length; i++)
            {
                richTextBox5.Text = richTextBox5.Text + "\n" + i + "\t" + round_keys[i].tostring();
            }
            */


        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox2.Clear();
            textBox1.Clear();
            State key = new State(var_key_length);
            rand = key.random();
            key.rozbuv(var_key_length, var_data_length);
            byte[,] b = key.firstSum(var_key_length, var_data_length); // 1
            key.S_boxes(); // 2
            key.Shift_Rows();//3
            key.Mix_Columns();//4
            key.XOR(); // 5
            key.S_boxes(); // 6
            key.Shift_Rows();//7
            key.Mix_Columns();//8
            key.change();//9
            key.S_boxes(); // 10
            key.Shift_Rows();//11
            key.Mix_Columns();//12
            Kt = new byte[8, var_data_length];
            Kt = key.data;
            int cols = 0;
            switch (var_data_length)
            {
                case 128: cols = 2;
                    break;
                case 256: cols = 4;
                    break;
                case 512: cols = 8;
                    break;
            }
            for (int j = 0; j < cols; j++)
            {
                for (int i = 0; i < 8; i++)
                {
                    textBox2.Text += Kt[i, j].ToString("X");
                }
            }

            foreach (byte ss in rand)
            {
                textBox1.Text += ss.ToString("X");
            }
            // key.show(test_block);

            //  round_keys = key.key_expanding(var_data_length, var_key_length);
            /* richTextBox5.Text += "\n" + round_keys.Length + "\n";*/


            /*   for (int i = 0; i < round_keys.Length; i++)
               {
                   richTextBox5.Text = richTextBox5.Text + "\n" + i +"\t" + round_keys[i].tostring();
               }
               */

            /* if (radioButton7.Checked == true)
             {
                 textBox1.Text = key.tostring();
             }
             else
             {
                 textBox1.Text = Encoding.Default.GetString(key.two_dim_to_one_dim(var_key_length));
             }*/
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            radioButton4.Enabled = true;
            radioButton5.Enabled = true;
            radioButton6.Enabled = false;

            var_key_length = 128;
            var_data_length = 128;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            radioButton5.Checked = true;
            radioButton4.Enabled = false;
            radioButton5.Enabled = true;
            radioButton6.Enabled = true;

            var_key_length = 256;
            var_data_length = 256;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            radioButton6.Checked = true;
            radioButton4.Enabled = false;
            radioButton5.Enabled = false;
            radioButton6.Enabled = true;

            var_key_length = 512;
            var_data_length = 512;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            var_key_length = 128;

            if ((bool)radioButton7.Checked)
            {
                if (textBox1.Text.Length > var_key_length / 4)
                {
                    textBox1.Text = textBox1.Text.Remove(32);
                }
            }
            if ((bool)radioButton8.Checked)
            {
                if (textBox1.Text.Length > var_key_length / 8)
                {
                    textBox1.Text = textBox1.Text.Remove(16);
                }
            }
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            var_key_length = 256;

            if ((bool)radioButton7.Checked)
            {
                if (textBox1.Text.Length > var_key_length / 4)
                {
                    textBox1.Text = textBox1.Text.Remove(64);
                }
            }
            if ((bool)radioButton8.Checked)
            {
                if (textBox1.Text.Length > var_key_length / 8)
                {
                    textBox1.Text = textBox1.Text.Remove(32);
                }
            }
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            var_key_length = 512;
        }

        byte[,] f = new byte[8, 2];
        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox2.Clear();
            /*richTextBox2.Text = Encrypt_ECB(richTextBox1.Text, round_keys, var_data_length, var_key_length, richTextBox2);*/

            int bl_l = var_data_length / 4;
            string text1 = richTextBox1.Text;
            if (richTextBox1.Text.Length % bl_l != 0)
            {
                if (richTextBox1.Text.Length < bl_l)
                {
                    int dop = bl_l - richTextBox1.Text.Length;
                    text1 += "10";
                    for (int i = 0; i < dop - 1; i++)
                    {
                        text1 += "00";
                    }
                }
                else
                {
                    int dop = richTextBox1.Text.Length % bl_l;
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
            for (int l = 0; l < kblock; l++)
            {
                bl[l] = text1.Substring(blk, 16 * cols);
                blk += 32;
            }

            numHex nh = new numHex();

            byte[,] mydata = new byte[8, cols];
            for (int t = 0; t < bl.Length; t++)
            {
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
                byte[,] buffer = new byte[8, cols];
                for (int r = 0; r < Rkeys.Length; r++)
                {
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


                    if (r == 0)
                    {
                        buffer = key.addNumbers(data, mydata, cols);
                    }
                    else if (r == Rkeys.Length - 1)
                    {
                        buffer = key.sBoxes(buffer, cols);
                        buffer = key.S_Rows(buffer, cols);
                        buffer = key.M_Columns(buffer, cols);
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
                                richTextBox2.Text += s;
                            }
                        }
                        // richTextBox2.Text += Environment.NewLine;
                    }
                    else
                    {
                        buffer = key.sBoxes(buffer, cols);
                        buffer = key.S_Rows(buffer, cols);
                        buffer = key.M_Columns(buffer, cols);
                        buffer = key.xor_rkey(buffer, data, cols);
                    }


                }

            }
        }
        private void richTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (radioButton7.Checked == true)
            {
                e.Handled = "0123456789abcdefABCDEF".IndexOf(e.KeyChar) < 0;
            }
        }

        private void richTextBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (radioButton7.Checked == true)
            {
                e.Handled = "0123456789abcdefABCDEF".IndexOf(e.KeyChar) < 0;

                if (richTextBox3.Text.Length == var_data_length / 4)
                {
                    e.Handled = true;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            richTextBox4.Clear();
            // richTextBox4.Text = Decrypt_ECB(richTextBox3.Text, round_keys, var_data_length, var_key_length, richTextBox2);

            int bl_l = var_data_length / 4;
            string text1 = richTextBox3.Text;


            int kblock = (int)(text1.Length / bl_l);

            string[] bl = new string[kblock];

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
            int blk = 0;
            for (int l = 0; l < kblock; l++)
            {
                bl[l] = text1.Substring(blk, 16 * cols);
                blk += 32;
            }

            numHex nh = new numHex();

            byte[,] mydata = new byte[8, cols];
            for (int t = 0; t < bl.Length; t++)
            {
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
                byte[,] buffer = new byte[8, cols];
                for (int r = Rkeys.Length - 1; r >= 0; r--)
                {
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


                    if (r == 0)
                    {
                        buffer = key.subNumbers(buffer, data, cols);
                        for (int j = 0; j < cols; j++)
                        {
                            for (int i = 0; i < 8; i++)
                            {
                                string s = buffer[i, j].ToString("x");
                                if (s.Length < 2)
                                {
                                    s = "0" + s;
                                }
                                richTextBox4.Text += s;
                            }
                        }
                    }
                    else if (r == Rkeys.Length - 1)//0f0e0d0c0b0a09080706050403020100
                    {
                        buffer = key.subNumbers(mydata, data, cols);
                        buffer = key.Inv_MColumns(buffer, cols);
                        buffer = key.Inv_S_Rows(buffer, cols);
                        buffer = key.Inv_Sboxes(buffer, cols);



                        // richTextBox2.Text += Environment.NewLine;
                    }
                    else
                    {
                        buffer = key.xor_rkey(buffer, data, cols);
                        buffer = key.Inv_MColumns(buffer, cols);
                        buffer = key.Inv_S_Rows(buffer, cols);
                        buffer = key.Inv_Sboxes(buffer, cols);

                    }


                }

            }
        }


        public void _11(byte[,] b, int cols)
        {
            FileStream fs = new FileStream("1.txt", FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            for (int j = 0; j < cols; j++)
            {
                for (int i = 0; i < 8; i++)
                {
                    sw.Write(b[i, j].ToString("x"));
                }
            }
            sw.WriteLine();
            sw.Close();
            fs.Close();



        }
        public void _22(byte[,] b, int cols)
        {
            FileStream fs = new FileStream("2.txt", FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            for (int j = 0; j < cols; j++)
            {
                for (int i = 0; i < 8; i++)
                {
                    sw.Write(b[i, j].ToString("x"));
                }
            }
            sw.WriteLine();
            sw.Close();
            fs.Close();



        }
        private void таблиціЗаміниToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2();
            f2.Show();
        }

        private void таблиціЗаміниДляРозшифруванняToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 f3 = new Form3();
            f3.Show();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox5.Text = "";
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            button2.Enabled = true;
            richTextBox3.Text = "";
            richTextBox4.Text = "";
        }

        private void зашифруванняToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form4 f4 = new Form4(var_data_length, richTextBox1.Text, Rkeys);
            f4.Show();
        }

        private void richTextBox3_TextChanged(object sender, EventArgs e)
        {
            button3.Enabled = true;
        }
        public string[] Rkeys;
        private void button5_Click(object sender, EventArgs e)
        {
            State key = new State();
            string[] con = key.key_forming(var_key_length, var_data_length);
            int count = 0;
            int cols = 0;
            int cols1 = 0;
            switch (var_key_length)
            {
                case 128: cols = 2;
                    break;
                case 256: cols = 4;
                    break;
                case 512: cols = 8;
                    break;
            }
            switch (var_data_length)
            {
                case 128:
                    cols1 = 2;
                    break;
                case 256:
                    cols1 = 4;
                    break;
                case 512:
                    cols1 = 8;
                    break;
            }

            Rkeys = new string[con.Length*2-1];
            int rk = 0;
            bool fir = true;
            bool fir1 = true;
            foreach (string constanta in con)
            {
                byte[,] b_con = key.toHex(constanta, var_data_length);
                byte[,] _0_0 = null;
                if (var_key_length == var_data_length)
                {
                    _0_0 = key.zsuvv(rand, 32, count, cols);
                }
                else if (var_key_length > var_data_length)
                {
                    if (count % 4 == 0)
                    {
                        byte[,] _0 = key.zsuvv(rand, 16, count, cols);

                        byte[,] nn = new byte[8, cols/2];
                        
                            for (int j = 0; j < cols / 2; j++)
                            {
                                for (int i = 0; i < 8; i++)
                                {
                                    nn[i, j] = _0[i, j];
                                }
                            }
                        
                       // 
                        _0_0 = nn;
                    }

                    else
                    {
                        int o = (int)Math.Truncate((double)(count / 4));

                        byte[,] _0 = key.zsuvv(rand, 64,o, cols);

                        byte[,] nn = new byte[8, cols/2];
                        
                            for (int j = cols/2; j < cols; j++)
                            {
                                for (int i = 0; i < 8; i++)
                                {
                                    nn[i, j - cols/2] = _0[i, j];
                                }
                            }
                        
                        _0_0 = nn;
                    }
                }
                count += 2;

                byte[,] add_rkey = key.addNumbers(Kt, b_con, cols1);

                byte[,] kt_round = key.addNumbers(add_rkey, _0_0, cols1);

                byte[,] s_box1 = key.sBoxes(kt_round, cols1);

                byte[,] s_row1 = key.S_Rows(s_box1,cols1);

                byte[,] m_col1 = key.M_Columns(s_row1, cols1);
              //  byte[,] bb = key.Inv_MColumns(m_col1, cols);
                byte[,] xor_rkey = key.xor_rkey(add_rkey, m_col1, cols1);

                byte[,] s_box2 = key.sBoxes(xor_rkey, cols1);

                byte[,] s_row2 = key.S_Rows(s_box2, cols1);

                byte[,] m_col2 = key.M_Columns(s_row2, cols1);

                byte[,] final = key.addNumbers(m_col2, add_rkey, cols1);

                
                for (int j = 0; j < cols1; j++)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        Rkeys[rk] += final[i, j].ToString()+"/";
                    }
                }
                 

                rk += 2;
            }

            for(int ii=0;ii<con.Length*2-1;ii+=2)
            {
                if (ii == con.Length * 2 - 2) break;
                string[] buf = Rkeys[ii].Split('/');
                int[] num = new int[buf.Length];
                int c = 0;
                foreach (string s in buf)
                {
                    if (s != "")
                    {
                        num[c] = Int32.Parse(s);
                        c++;
                    }
                }

                byte[,] data = new byte[8, cols1];
                c = 0;
                for (int j = 0; j < cols1; j++)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        data[i, j] = (byte)num[c];
                        c++;
                    }
                }

                byte[,] left = key.zsuvvL(data, ((int)(var_data_length / 4) + 24), 1, cols1);

                rk = 0;
                for (int j = 0; j < cols1; j++)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        Rkeys[ii+1] += left[i, j].ToString() + "/";
                    }
                }
            }

            int ccc = 1;
            foreach (string s in Rkeys)
            {
                string[] buff = s.Split('/');
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

                byte[,] data = new byte[8, cols1];
                c = 0;
                for (int j = 0; j < cols1; j++)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        data[i, j] = (byte)num[c];
                        c++;
                    }
                }
                richTextBox5.Text += "Ключ [" + ccc + "] = ";
                for (int j = 0; j < cols1; j++)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        string s2 = data[i, j].ToString("x");
                        if (s.Length < 2)
                        {
                            s2 = "0" + s2;
                        }
                        richTextBox5.Text += s2;
                    }
                }
                richTextBox5.Text += Environment.NewLine;
                ccc++;
            }
        }
    }
}
