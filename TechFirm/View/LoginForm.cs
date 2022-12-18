using System;
using System.Linq;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;

namespace TechFirm.View
{

    public partial class LoginForm : KryptonForm
    {
        
        public LoginForm()
        {
            InitializeComponent();
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            using (var db = new DatabaseContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Email == emailTextBox.Text && u.Password == passwordTextBox.Text);
                if (user == null)
                {
                    MessageBox.Show("Invalid email or password");
                    return;
                }
                
                user.AccountInformation.LastLoginDate = DateTime.Now;
                db.SaveChanges();
                
                var adminForm = new MainForm();
                adminForm.InitializeData(user);
                adminForm.Show();
                Hide();
            }
        }

    }
}
