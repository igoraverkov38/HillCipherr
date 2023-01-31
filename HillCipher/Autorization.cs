using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HillCipher
{
    public partial class Autorization : Form
    {
        AutorizationBoard board;
        public Autorization()
        {
            InitializeComponent();
            board = new AutorizationBoard(label3, textBox1, textBox2, checkBox1, this);
        }
        class AutorizationBoard
        {
            Label info;TextBox name_box, password_box;
            CheckBox check; Autorization form;
            public AutorizationBoard(Label info, TextBox name_box, TextBox password_box, CheckBox check, Autorization form)
            {
                this.info = info; this.name_box = name_box; this.password_box = password_box;
                this.check = check; this.form = form;
                check.CheckedChanged += CheckedChanged;
            }
            public void HidePassword()
            {
                password_box.UseSystemPasswordChar = !check.Checked;
            }
            private void CheckedChanged(object sender, EventArgs e)
            {
                HidePassword();
            }
            public void SignUp()
            {
                Registration sf = new Registration();
                sf.FormClosed += (s, args) => form.Close();
                form.Hide();
                sf.Show();
            }
            public void TryEnter()
            {
                name_box.Text = name_box.Text.Trim();
                password_box.Text = password_box.Text.Trim();
                info.Text = "Сообщение:";
                try
                {
                    User user = new User(name_box.Text, password_box.Text);
                    if (user.CorrectUser() && user.UserValid())
                    {
                        form.Hide();
                        MainForm mf = new MainForm();
                        mf.FormClosed += (s, args) => form.Close();
                        mf.Show();
                    }
                    else
                    {
                        info.Text = "Сообщение: неверное имя пользователя или пароль";
                        return;
                    }
                }
                catch (Exception err)
                {
                    info.Text = "Сообщение: "+ err.Message;
                    return;
                }

            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            board.TryEnter();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            board.SignUp();
        }
    }
}
