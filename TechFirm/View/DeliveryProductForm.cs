using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using TechFirm.Models;

namespace TechFirm.View
{
    public partial class DeliveryProductForm : KryptonForm
    {
        private DeliveryProduct _deliveryProduct;
        private int _index;
        
        private DialogMode _dialogMode;
        private IDeliveryProductActionHandler _actionHandler;

        public DeliveryProductForm()
        {
            InitializeComponent();

            productComboBox.DataSource = new DatabaseContext().Products.ToList();
            productDescriptionText.Enabled = false;
            productPriceText.Enabled = false;
            productMinimalCountText.Enabled = false;
        }

        public void InitializeData( IDeliveryProductActionHandler actionHandler)
        {
            DeliveryProduct product = new DeliveryProduct();
            InitializeData(product, DialogMode.Add, actionHandler);
        }
        
        public void InitializeData(DeliveryProduct deliveryProduct, int index, IDeliveryProductActionHandler actionHandler)
        {
            _index = index;
            InitializeData(deliveryProduct, DialogMode.Edit, actionHandler);
        }

        public void InitializeData( DeliveryProduct deliveryProduct, DialogMode dialogMode, IDeliveryProductActionHandler actionHandler )
        {
            _deliveryProduct = deliveryProduct;
            _dialogMode = dialogMode;
            _actionHandler = actionHandler;

            productComboBox.SelectedItem = deliveryProduct.Product;
            RefreshProduct();

            vendorPriceText.Text = _deliveryProduct.Price.ToString(CultureInfo.InvariantCulture);
            vendorCountText.Text = _deliveryProduct.Count.ToString(CultureInfo.InvariantCulture);

            if (_dialogMode == DialogMode.Preview)
            {
                productComboBox.Enabled = false;
                vendorPriceText.Enabled = false;
                vendorCountText.Enabled = false;
                
                saveButton.Text = "OK";
            }
        }

        private void goBackButton_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            _deliveryProduct.Count = Convert.ToInt32(vendorCountText.Text);
            _deliveryProduct.Price = Convert.ToDouble(vendorPriceText.Text);
            _deliveryProduct.Product = (Product)productComboBox.SelectedItem;

            if (!ValidateControls())
                return;

            switch (_dialogMode)
            {
                case DialogMode.Preview:
                    break;
                case DialogMode.Edit:
                    _actionHandler.OnEditAction(_index, _deliveryProduct);
                    break;
                case DialogMode.Add:
                    _actionHandler.OnAddAction(_deliveryProduct);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            Hide();
        }

        private bool ValidateControls()
        {
            if (productComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select product");
                return false;
            }
            
            if (string.IsNullOrEmpty(vendorCountText.Text))
            {
                MessageBox.Show("Count cannot be empty.", "", MessageBoxButtons.OK);
                return false;
            }

            if (string.IsNullOrEmpty(vendorPriceText.Text))
            {
                MessageBox.Show("Vendor price cannot be empty.", "", MessageBoxButtons.OK);
                return false;
            }

            return true;
        }

        private void productComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshProduct();
        }

        void RefreshProduct()
        {
            Product product = (Product)productComboBox.SelectedItem;

            if (product == null)
                return;
            
            productMinimalCountText.Text = product.MinimalCount.ToString();
            productPriceText.Text = product.Price.ToString(CultureInfo.InvariantCulture);
            productDescriptionText.Text = product.Description;
        }

        private void vendorPriceText_TextChanged(object sender, EventArgs e)
        {
            RefreshTotalAmount();
        }

        private void vendorCountText_TextChanged(object sender, EventArgs e)
        {
            RefreshTotalAmount();
        }

        private void RefreshTotalAmount()
        {
            if (string.IsNullOrEmpty(vendorPriceText.Text) || string.IsNullOrEmpty(vendorCountText.Text))
                return;

            double price = Convert.ToDouble(vendorPriceText.Text);
            int count = Convert.ToInt32(vendorCountText.Text);

            totalAmountLabel.Text = "Total amount: " + (price * count).ToString(CultureInfo.InvariantCulture);
        }
    }
}
