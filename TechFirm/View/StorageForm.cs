using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;

namespace TechFirm.View
{
    public partial class StorageForm : KryptonForm
    {
        class ViewData
        {
            public string Product { get; set; }

            public double Price { get; set; }

            public int Quantity { get; set; }
        }

        private Storage _storage;
        private DialogMode _dialogMode;
        private IActionHandler _actionHandler;

        public StorageForm()
        {
            InitializeComponent();
        }

        public void InitializeData(DialogMode dialogMode, IActionHandler actionHandler)
        {
            Storage storage = new Storage();
            InitializeData(storage, dialogMode, actionHandler);
        }
        
        public void InitializeData( Storage storage, DialogMode dialogMode, IActionHandler actionHandler )
        {
            _storage = storage;
            _dialogMode = dialogMode;
            _actionHandler = actionHandler;

            phoneNumberTextBox.Text = _storage.PhoneNumber;
            addressTextBox.Text = _storage.Address;

            if (_dialogMode == DialogMode.Preview)
            {
                phoneNumberTextBox.Enabled = false;
                addressTextBox.Enabled = false;
                saveButton.Enabled = false;
                saveButton.Text = "OK";
            }

            storageProductGridView.RowTemplate.Height = 50;
            storageProductGridView.AutoGenerateColumns = true;
            storageProductGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            storageProductGridView.RowHeadersVisible = false;
            storageProductGridView.RowTemplate.Height = 50;

            List<ViewData> viewDataList = new List<ViewData>();
            var storageProducts = new DatabaseContext().StorageProducts.Where(sp => sp.Storage.Id == _storage.Id).ToList();

            foreach (var storageProduct in storageProducts)
            {
                viewDataList.Add(new ViewData
                {
                    Price = storageProduct.Product.Price,
                    Product = storageProduct.Product.Name,
                    Quantity = storageProduct.Count
                });
            }

            storageProductGridView.DataSource = viewDataList;
        }

        private void goBackButton_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            _storage.PhoneNumber = phoneNumberTextBox.Text;
            _storage.Address = addressTextBox.Text;

            if (!ValidateControls())
                return;

            if (!ValidateData())
                return;

            using (var instance = new DatabaseContext())
            {
                if (_dialogMode == DialogMode.Add)
                    instance.Storages.Add(_storage);
                else if (_dialogMode == DialogMode.Edit)
                    instance.Storages.AddOrUpdate(_storage);

                instance.SaveChanges();
            }

            _actionHandler.OnSaveAction();
            Hide();
        }

        private bool ValidateControls()
        {
            if (string.IsNullOrEmpty(phoneNumberTextBox.Text))
            {
                MessageBox.Show("Phone number cannot be empty.", "", MessageBoxButtons.OK);
                return false;
            }

            if (string.IsNullOrEmpty(addressTextBox.Text))
            {
                MessageBox.Show("Address cannot be empty.", "", MessageBoxButtons.OK);
                return false;
            }

            return true;
        }

        private bool ValidateData()
        {
            using (var instance = new DatabaseContext())
            {
                if (instance.Storages.Any(st => st.Address == _storage.Address && st.Id != _storage.Id))
                {
                    MessageBox.Show("Storage with this address already exists.", "", MessageBoxButtons.OK);
                    return false;
                }
            }
            
            return true;
        }
    }
}
