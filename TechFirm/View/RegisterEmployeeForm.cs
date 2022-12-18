using System;
using System.Linq;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using TechFirm.Models;

namespace TechFirm.View
{
    public partial class RegisterEmployeeForm : KryptonForm
    {
        private User _user;
        
        public RegisterEmployeeForm()
        {
            InitializeComponent();
        }

        public void InitializeData(User user)
        {
            _user = user;
        }

        private void GoBack()
        {
            MainForm mainForm = new MainForm();
            mainForm.InitializeData(_user);
            mainForm.Show();
            Hide();
        }

        private void goBackButton_Click(object sender, EventArgs e)
        {
            GoBack();
        }

        private void registerEmployeeButton_Click(object sender, EventArgs e)
        {
            using ( var instance = new DatabaseContext())
            {
                if (!ValidateControls())
                    return;

                User user = new User
                {
                    Email = emailTextBox.Text,
                    Password = passwordTextBox.Text,
                    FirstName = firstNameTextBox.Text,
                    LastName = lastNameTextBox.Text,

                    AccountInformation = new AccountInformation
                    {
                        CreationDate = DateTime.Now,
                    },

                };

                if (!ValidateData(user))
                    return;

                instance.Users.Add(user);
                instance.SaveChanges();

                MessageBox.Show("User registered successfully.", "Register Employee", MessageBoxButtons.OK);
                GoBack();
            }
        }

        private bool ValidateControls()
        {
            if (string.IsNullOrEmpty(emailTextBox.Text))
            {
                MessageBox.Show("Email cannot be empty.", "", MessageBoxButtons.OK);
                return false;
            }

            if (string.IsNullOrEmpty(passwordTextBox.Text))
            {
                MessageBox.Show("Password cannot be empty.", "", MessageBoxButtons.OK);
                return false;
            }

            if (string.IsNullOrEmpty(firstNameTextBox.Text))
            {
                MessageBox.Show("First name cannot be empty.", "", MessageBoxButtons.OK);
                return false;
            }

            if (string.IsNullOrEmpty(lastNameTextBox.Text))
            {
                MessageBox.Show("Last name cannot be empty.", "", MessageBoxButtons.OK);
                return false;
            }

            return true;
        }

        private bool ValidateData(User user)
        {
            using (var instance = new DatabaseContext())
            {
                if (instance.Users.Any(emp => emp.Email == user.Email))
                {
                    MessageBox.Show("This email already exists.", "Register Employee Error", MessageBoxButtons.OK);
                    return false;
                }
            }
            
            return true;
        }
    }
}
