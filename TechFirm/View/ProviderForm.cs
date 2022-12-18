using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using TechFirm.Models;

namespace TechFirm.View
{
    public partial class ProviderForm : KryptonForm
    {
        private Provider _provider;
        private DialogMode _dialogMode;
        private IActionHandler _actionHandler;

        public ProviderForm()
        {
            InitializeComponent();
        }

        public void InitializeData(DialogMode dialogMode, IActionHandler actionHandler)
        {
            Provider provider = new Provider();
            InitializeData(provider, dialogMode, actionHandler);
        }
        
        public void InitializeData( Provider provider, DialogMode dialogMode, IActionHandler actionHandler )
        {
            _provider = provider;
            _dialogMode = dialogMode;
            _actionHandler = actionHandler;

            nameTextBox.Text = _provider.Name;
            phoneNumberTextBox.Text = _provider.PhoneNumber;
            emailTextBox.Text = _provider.Email;
            addressTextBox.Text = _provider.Address;

            if (_dialogMode == DialogMode.Preview)
            {
                nameTextBox.Enabled = false;
                phoneNumberTextBox.Enabled = false;
                emailTextBox.Enabled = false;
                addressTextBox.Enabled = false;
                saveButton.Enabled = false;
                saveButton.Text = "OK";
            }
        }

        private void goBackButton_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            _provider.Name = nameTextBox.Text;
            _provider.PhoneNumber = phoneNumberTextBox.Text;
            _provider.Email = emailTextBox.Text;
            _provider.Address = addressTextBox.Text;

            if (!ValidateControls())
                return;

            if (!ValidateData())
                return;

            using (var instance = new DatabaseContext())
            {
                if (_dialogMode == DialogMode.Add)
                    instance.Providers.Add(_provider);
                else if (_dialogMode == DialogMode.Edit)
                    instance.Providers.AddOrUpdate(_provider);

                instance.SaveChanges();
            }

            _actionHandler.OnSaveAction();
            Hide();
        }

        private bool ValidateControls()
        {
            if (string.IsNullOrEmpty(nameTextBox.Text))
            {
                MessageBox.Show("Name cannot be empty.", "", MessageBoxButtons.OK);
                return false;
            }

            if (string.IsNullOrEmpty(addressTextBox.Text))
            {
                MessageBox.Show("Address cannot be empty.", "", MessageBoxButtons.OK);
                return false;
            }

            if (string.IsNullOrEmpty(emailTextBox.Text))
            {
                MessageBox.Show("Email count cannot be empty.", "", MessageBoxButtons.OK);
                return false;
            }

            if (string.IsNullOrEmpty(phoneNumberTextBox.Text))
            {
                MessageBox.Show("Phone number cannot be empty.", "", MessageBoxButtons.OK);
                return false;
            }

            return true;
        }

        private bool ValidateData()
        {
            using (var instance = new DatabaseContext())
            {
                if (instance.Providers.Any(st => st.Email == _provider.Email && st.Id != _provider.Id))
                {
                    MessageBox.Show("Provider with this email already exists.", "", MessageBoxButtons.OK);
                    return false;
                }

                if (instance.Providers.Any(st => st.PhoneNumber == _provider.PhoneNumber && st.Id != _provider.Id))
                {
                    MessageBox.Show("Provider with this phone number already exists.", "", MessageBoxButtons.OK);
                    return false;
                }
            }
            
            return true;
        }
    }
}
