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
    public partial class Registration : Form
    {
        RegistrationBoard board;
        public Registration()
        {
            InitializeComponent();
            board = new RegistrationBoard(label3, textBox1, textBox2, textBox3, checkBox1, this);
        }
        class RegistrationBoard
        {
            Label info; TextBox name_box, password_box, confirm_password_box;
            CheckBox check; Registration form;
            public RegistrationBoard(Label info, TextBox name_box, TextBox password_box, TextBox confirm_password_box, CheckBox check, Registration form)
            {
                this.info = info; this.name_box = name_box; this.password_box = password_box;
                this.check = check; this.form = form; this.confirm_password_box = confirm_password_box;
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
            public void TrySignUp()
            {
                name_box.Text = name_box.Text.Trim();
                password_box.Text = password_box.Text.Trim();
                confirm_password_box.Text = confirm_password_box.Text.Trim();
                info.Text = "Сообщение:";
                try
                {
                    var user = new User(name_box.Text, password_box.Text, confirm_password_box.Text);
                    if (user.CorrectUser() && !user.UsernameExist())
                    {
                        user.CreateUser();
                        form.Hide();
                        MainForm mf = new MainForm();
                        mf.FormClosed += (s, args) => form.Close();
                        mf.Show();
                    }
                    else
                        info.Text = "Сообщение: имя пользователя уже занято";
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
            board.TrySignUp();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Registration_Load(object sender, EventArgs e)
        {

        }
    }
}
