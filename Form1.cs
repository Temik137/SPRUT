using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Net;

namespace server
{
    public partial class Form1 : Form
    {
        public TcpClient client = new TcpClient();
        public string ip = String.Empty;
        public int port = new int();


        public Form1()
        {            
            InitializeComponent();
            button2.Visible = false;
           
        }

     private void send(TcpClient client, string text)
        {
        try
        {


                String request = "";

                request = text;
            var data = Convert.ToBase64String((System.Text.Encoding.UTF8.GetBytes(text)));
            byte[] data64 = System.Text.Encoding.UTF8.GetBytes(data);
           
                NetworkStream stream = client.GetStream();

            stream.Write(data64, 0, data64.Length);

            data64 = new Byte[40960];

            String response = String.Empty;
            Int32 bytes = stream.Read(data64, 0, data64.Length);  //try/catch

            response = System.Text.Encoding.UTF8.GetString(data64, 0, bytes);

                string strt, RES, grp, id = "";
                {
                    try
                    {
                        strt = response.Substring(0, response.IndexOf("#$"));
                        response = response.Substring(response.IndexOf("#$") + 2, response.Length - response.IndexOf("#$") - 2);
                        RES = response.Substring(0, response.IndexOf("#$"));
                        response = response.Substring(response.IndexOf("#$") + 2, response.Length - response.IndexOf("#$") - 2);
                        grp = response.Substring(0, response.IndexOf("#$"));
                        response = response.Substring(response.IndexOf("#$") + 2, response.Length - response.IndexOf("#$") - 2);
                        id = response.Substring(0, response.IndexOf("#$"));
                        response = response.Substring(response.IndexOf("#$") + 2, response.Length - response.IndexOf("#$") - 2);
                        textBox1.Invoke((ThreadStart)delegate ()
                        {
                            textBox1.Text = strt+ '\n' + '\n' + "Ответ:" + '\n' +RES + '\n' + "Группа:"+grp + '\n' + "ID ответа:"+id;
                        });
                    }
                    catch
                    {
                        //MessageBox.Show("Error in recieve of responce");
                        textBox1.Invoke((ThreadStart)delegate ()
                        {
                            textBox1.Text = "Ошибка принятия ответа №" + id;
                        });
                    }
                    
                }
                /*var decode_bytes = Convert.FromBase64String(responseData_encode);
            string responseData = Encoding.UTF8.GetString(decode_bytes);*/
                /*if (responseData.Length > 0)
                {
                        textBox1.Invoke((ThreadStart)delegate ()
                        {
                            textBox1.Text = responseData;
                        });


                }
                else
                    {
                        textBox1.Invoke((ThreadStart)delegate ()
                        {

                            textBox1.Text = "Nothing";
                        });

                    }*/
                if (!textBox4.Text.Contains("command failed"))
            {
                    button2.Invoke((ThreadStart)delegate ()
                    {
                        button2.Visible = true;
                    });
                   
            }
                stream.Close();
                client.Close();
            }

        catch (ArgumentNullException g)
        {
            textBox1.Text = g.Message;
        }
    }

        private void button1_Click(object sender, EventArgs e)
        {
            string s = comboBox1.Text + "#$" + textBox4.Text + "#$" + textBox5.Text + "#$" + (label9.Text)+"#$END";
                    
            if (client.Connected)
            {
                var thread = new Thread(() =>
               
                {
                   send(client, s);
                    

                });
                thread.Start();
            }
            else
            { 
            var thread = new Thread(() =>
                {
                    connect(port, ip);
                    Invoke((ThreadStart)delegate ()
                    {
                         send(client, s);
                       // label7.Text = Convert.ToString(Convert.ToInt32(label7.Text) + 1);

                    });
                   
                });

                thread.Start();
            }
            int i = Convert.ToInt32(label9.Text) + 1;
            label9.Text = Convert.ToString(i);
        }

      

        private void savelog_Click(object sender, EventArgs e)
    {
            string path = string.Empty;
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                     path = dialog.SelectedPath;
                }
            }
            
            string filename = @"\log_" + DateTime.Now.ToShortDateString()+".txt";
            if (path.Length > 0)
            {
                path += filename;
                File.WriteAllText(path, textBox1.Text);
                MessageBox.Show(@"File successfully saved!" ,"Success", MessageBoxButtons.OK);
            }
           
    }

        private void button3_Click(object sender, EventArgs e)
        {
            
          
            string pattern = @"^(([2]([0-4][0-9]|[5][0-5])|[0-1]?[0-9]?[0-9])[.]){3}(([2]([0-4][0-9]|[5][0-5])|[0-1]?[0-9]?[0-9]))([/][\d]{1,2}){0,1}$";
            if (Convert.ToInt32(textBox3.Text) >= 0 && Regex.IsMatch(textBox2.Text, pattern))
            {
                ip = IPAddress.Parse(textBox2.Text).ToString();

                ip = textBox2.Text;
                port = Convert.ToInt32(textBox3.Text);
                var thread = new Thread(() =>
                {
                    connect(port, ip);
                });

                thread.Start();
            }
            else
            {
                label3.Text = "Wrong Ip or Port!";
                label3.ForeColor = Color.Red;
                label3.Visible = true;
            }

        }
        private void connect(int port, string ip)
        {
            try
            {
                client = new TcpClient();
                client.Connect(ip, port);


                if (client.Connected)
                {
                    label3.Invoke((ThreadStart)delegate ()
                    {
                        label3.Text = "Connected Success";
                        label3.ForeColor = Color.Green;
                        label3.Visible = true;
                    });
                    textBox1.Invoke((ThreadStart)delegate ()
                    {
                        textBox1.ReadOnly = false;
                        textBox4.ReadOnly = false;
                    });
                    button1.Invoke((ThreadStart)delegate ()
                    {
                        button1.Enabled = true;
                    });
                    button4.Invoke((ThreadStart)delegate ()
                    {
                        button4.Visible  = true;
                    });
                    textBox3.Invoke((ThreadStart)delegate ()
                    {
                        textBox3.ReadOnly = true;
                    });
                    textBox2.Invoke((ThreadStart)delegate ()
                    {
                        textBox2.ReadOnly = true;
                    });
                    button3.Invoke((ThreadStart)delegate ()
                    {
                        button3.Visible = false;
                        button3.Enabled = false;

                    });
                    button4.Invoke((ThreadStart)delegate ()
                    {
                        button4.Enabled = true;

                    });
                   
                }
                else
                {
                    label3.Invoke((ThreadStart)delegate ()
                    {
                        label3.Text = "Not Connected";
                        label3.ForeColor = Color.Red;
                        label3.Visible = true;
                    });
                   
                }


            }
            catch (Exception con)
            {
                label3.Invoke((ThreadStart)delegate ()
                {
                    label3.Text = "Not Connected";
                    label3.ForeColor = Color.Red;
                    label3.Visible = true;
                });
              
            }


        }
           


        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Text = String.Empty;
            textBox1.ReadOnly = true;
            button1.Enabled = false;
            textBox3.ReadOnly = false;
            textBox4.ReadOnly = false;
            textBox2.ReadOnly = false;
            button3.Visible = true;
            button3.Enabled = true;
            button4.Visible = false;
            button2.Enabled = false;
            client.Close();
        
        }

        private void textBox2_MouseClick(object sender, MouseEventArgs e)
        {
            label3.Visible = false;
        }

        private void textBox3_MouseClick(object sender, MouseEventArgs e)
        {
            label3.Visible = false;
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
