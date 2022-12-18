using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;

namespace TechFirm.View
{
    public partial class ProductForm : KryptonForm
    {
        private Product _product;
        private DialogMode _dialogMode;
        private IActionHandler _actionHandler;

        public ProductForm()
        {
            InitializeComponent();
        }

        public void InitializeData(DialogMode dialogMode, IActionHandler actionHandler)
        {
            Product product = new Product();
            InitializeData(product, dialogMode, actionHandler);
        }
        
        public void InitializeData( Product product, DialogMode dialogMode, IActionHandler actionHandler )
        {
            _product = product;
            _dialogMode = dialogMode;
            _actionHandler = actionHandler;

            nameTextBox.Text = _product.Name;
            priceTextBox.Text = _product.Price.ToString(CultureInfo.InvariantCulture);
            minimalCountTextBox.Text = _product.MinimalCount.ToString(CultureInfo.InvariantCulture);
            descriptionTextBox.Text = _product.Description;

            if (_dialogMode == DialogMode.Preview)
            {
                nameTextBox.Enabled = false;
                priceTextBox.Enabled = false;
                minimalCountTextBox.Enabled = false;
                descriptionTextBox.Enabled = false;
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
            _product.Name = nameTextBox.Text;
            _product.Price = Convert.ToDouble(priceTextBox.Text);
            _product.MinimalCount = Convert.ToInt32(minimalCountTextBox.Text);
            _product.Description = descriptionTextBox.Text;

            if (!ValidateControls())
                return;

            if (!ValidateData())
                return;

            using (var instance = new DatabaseContext())
            {
                if (_dialogMode == DialogMode.Add)
                    instance.Products.Add(_product);
                else if (_dialogMode == DialogMode.Edit)
                    instance.Products.AddOrUpdate(_product);

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

            if (string.IsNullOrEmpty(descriptionTextBox.Text))
            {
                MessageBox.Show("Description cannot be empty.", "", MessageBoxButtons.OK);
                return false;
            }

            if (string.IsNullOrEmpty(minimalCountTextBox.Text))
            {
                MessageBox.Show("Minimal count cannot be empty.", "", MessageBoxButtons.OK);
                return false;
            }

            if (string.IsNullOrEmpty(priceTextBox.Text))
            {
                MessageBox.Show("Price cannot be empty.", "", MessageBoxButtons.OK);
                return false;
            }

            return true;
        }

        private bool ValidateData()
        {
            using (var instance = new DatabaseContext())
            {
                if (instance.Storages.Any(st => st.Address == _product.Name && st.Id != _product.Id))
                {
                    MessageBox.Show("Product with this name already exists.", "", MessageBoxButtons.OK);
                    return false;
                }
            }
            
            return true;
        }
    }
}
