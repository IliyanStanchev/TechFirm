using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using TechFirm.Models;

namespace TechFirm.View
{
    public partial class DeliveryForm : KryptonForm, IDeliveryProductActionHandler
    {
        class ViewData
        {
            public string Name { get; set; }

            public long Count { get; set; }

            public double Amount { get; set; }

            public DeliveryProduct DeliveryProduct;
        }

        private Delivery _delivery;
        private DialogMode _dialogMode;
        private IActionHandler _actionHandler;

        private List<DeliveryProduct> _deliveryProducts;

        public DeliveryForm()
        {
            InitializeComponent();

            using (var instance = new DatabaseContext())
            {
                providerComboBox.DataSource = instance.Providers.ToList();
                storageComboBox.DataSource = instance.Storages.ToList();
            }
        }

        public void InitializeData(DialogMode dialogMode, IActionHandler actionHandler)
        {
            Delivery delivery = new Delivery();
            InitializeData(delivery, dialogMode, actionHandler);
        }
        
        public void InitializeData( Delivery delivery, DialogMode dialogMode, IActionHandler actionHandler )
        {
            _delivery = delivery;
            _dialogMode = dialogMode;
            _actionHandler = actionHandler;

            providerComboBox.SelectedItem = delivery.Provider;
            storageComboBox.SelectedItem = delivery.Storage;

            _deliveryProducts = new DatabaseContext().DeliveryProducts.Where(x => x.Delivery.Id == delivery.Id).ToList();

            deliveryProductsDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;
            deliveryProductsDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            deliveryProductsDataGridView.AutoGenerateColumns = true;
            deliveryProductsDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            deliveryProductsDataGridView.RowHeadersVisible = false;

            deliveryProductsDataGridView.RowTemplate.Height = 50;

            deliveryProductsDataGridView.MouseClick += DeliveryDataGridViewMouseClick;

            RefreshDeliveryProducts();

            if (_dialogMode == DialogMode.Preview)
            {
                providerComboBox.Enabled = false;
                storageComboBox.Enabled = false;
                saveButton.Text = "OK";
            }
        }

        private void DeliveryDataGridViewMouseClick(object sender, MouseEventArgs e)
        {
            if (_dialogMode == DialogMode.Preview)
                return;

            if (e.Button == MouseButtons.Left)
                return;

            ContextMenuStrip menu = new ContextMenuStrip();
            int position = deliveryProductsDataGridView.HitTest(e.X, e.Y).RowIndex;

            if (position >= 0)
            {
                deliveryProductsDataGridView.ClearSelection();
                deliveryProductsDataGridView.Rows[position].Selected = true;
                menu.Items.Add("Edit").Name = "Edit";
                menu.Items.Add("Delete").Name = "Delete";
            }
            else
            {
                menu.Items.Add("Add").Name = "Add";
            }

            menu.Show(deliveryProductsDataGridView, new Point(e.X, e.Y));
            menu.ItemClicked += Menu_ItemClicked;
        }

        private void Menu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Name)
            {
                case "Add":
                    AddRecord();
                    break;
                case "Edit":
                    EditRecord();
                    break;
                case "Delete":
                    DeleteRecord();
                    break;
            }
        }

        private void DeleteRecord()
        {
            ViewData deliveryProduct = deliveryProductsDataGridView.SelectedRows[0].DataBoundItem as ViewData;
            if (deliveryProduct == null)
                return;

            if (MessageBox.Show("Are you sure", "", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            _deliveryProducts.Remove(deliveryProduct.DeliveryProduct);
            RefreshDeliveryProducts();
        }

        private void EditRecord()
        {
            ViewData deliveryProduct = deliveryProductsDataGridView.SelectedRows[0].DataBoundItem as ViewData;
            if (deliveryProduct == null)
                return;

            int index = deliveryProductsDataGridView.SelectedRows[0].Index;

            DeliveryProductForm deliveryProductForm = new DeliveryProductForm();
            deliveryProductForm.InitializeData(deliveryProduct.DeliveryProduct, index, this);
            deliveryProductForm.ShowDialog();
        }

        private void AddRecord()
        {
            DeliveryProductForm deliveryProductForm = new DeliveryProductForm();
            deliveryProductForm.InitializeData( this );
            deliveryProductForm.ShowDialog();
        }

        private void RefreshDeliveryProducts()
        {
            List<ViewData> oViewDataList = new List<ViewData>();

            foreach (var deliveryProduct in _deliveryProducts)
            {
                ViewData oViewData = new ViewData
                {
                    DeliveryProduct = deliveryProduct,
                    Name = deliveryProduct.Product.Name,
                    Count = deliveryProduct.Count,
                    Amount = deliveryProduct.Price
                };

                oViewDataList.Add(oViewData);
            }

            deliveryProductsDataGridView.DataSource = oViewDataList;
            
            double totalAmount = 0;
            foreach (var deliveryProduct in _deliveryProducts)
            {
                totalAmount += deliveryProduct.Price * deliveryProduct.Count;
            }

            totalAmountLabel.Text = "Total amount: " + totalAmount.ToString(CultureInfo.InvariantCulture);
        }

        private void goBackButton_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (_dialogMode == DialogMode.Preview)
            {
                Hide();
                return;
            }
            
            if (!ValidateControls())
                return;

            using (var instance = new DatabaseContext())
            {
                using (var transaction = instance.Database.BeginTransaction())
                {
                    _delivery.Date = DateTime.Now;
                    _delivery.Provider = instance.Providers.Find(((Provider)providerComboBox.SelectedItem).Id);
                    _delivery.Storage = instance.Storages.Find(((Storage)storageComboBox.SelectedItem).Id);

                    _delivery = instance.Deliveries.Add(_delivery);

                    foreach (var deliveryProduct in _deliveryProducts)
                    {
                        deliveryProduct.Delivery = _delivery;
                        deliveryProduct.Product = instance.Products.Find(deliveryProduct.Product.Id);
                        instance.DeliveryProducts.Add(deliveryProduct);

                        StorageProduct foundProduct = instance.StorageProducts.FirstOrDefault(x => x.Product.Id == deliveryProduct.Product.Id && x.Storage.Id == _delivery.Storage.Id);
                        if (foundProduct == null)
                        {
                            foundProduct = new StorageProduct
                            {
                                Product = deliveryProduct.Product,
                                Storage = _delivery.Storage,
                                Count = deliveryProduct.Count
                            };

                            instance.StorageProducts.Add(foundProduct);
                        }
                        else
                        {
                            foundProduct.Count += deliveryProduct.Count;
                        }
                    }

                    instance.SaveChanges();
                    transaction.Commit();
                }
            }

            _actionHandler.OnSaveAction();
            Hide();
        }

        private bool ValidateControls()
        {
            if (providerComboBox.SelectedItem == null)
            {
                KryptonMessageBox.Show("Please select provider", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (storageComboBox.SelectedItem == null)
            {
                KryptonMessageBox.Show("Please select storage", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (_deliveryProducts.Count == 0)
            {
                KryptonMessageBox.Show("Please add products", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void deliveryProductsDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            ViewData deliveryProduct = deliveryProductsDataGridView.SelectedRows[0].DataBoundItem as ViewData;
            if (deliveryProduct == null)
                return;

            DeliveryProductForm deliveryForm = new DeliveryProductForm();
            deliveryForm.InitializeData(deliveryProduct.DeliveryProduct, DialogMode.Preview, this);
            deliveryForm.Show();
        }

        public void OnAddAction(DeliveryProduct deliveryProduct)
        {
            _deliveryProducts.Add(deliveryProduct);
            RefreshDeliveryProducts();
        }

        public void OnEditAction(int index, DeliveryProduct deliveryProduct)
        {
            _deliveryProducts[index] = deliveryProduct;
            RefreshDeliveryProducts();
        }
    }
}
