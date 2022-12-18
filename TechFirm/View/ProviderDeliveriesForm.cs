using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Aspose.Cells;
using ComponentFactory.Krypton.Toolkit;
using TechFirm.Models;

namespace TechFirm.View
{
    public partial class ProviderDeliveriesForm : KryptonForm
    {
        class ViewData
        {
            public string Date { get; set; }
            
            public string Product { get; set; }
            
            public string Storage { get; set; }

            public int Quantity { get; set; }

            public double Price { get; set; }
        }
        
        private User _user;

        List<ViewData> _viewData = new List<ViewData>();

        public ProviderDeliveriesForm()
        {
            InitializeComponent();

            providersComboBox.DataSource = new DatabaseContext().Providers.ToList();

            InitializeDataGridView();
        }

        void InitializeDataGridView()
        {
            deliveryDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;
            deliveryDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            deliveryDataGridView.AutoGenerateColumns = true;
            deliveryDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            deliveryDataGridView.RowHeadersVisible = false;

            deliveryDataGridView.RowTemplate.Height = 50;

            RefreshDataGridView();
        }

        void RefreshDataGridView()
        {
            Provider provider = providersComboBox.SelectedItem as Provider;
            if (provider == null)
                return;

            List<DeliveryProduct> deliveryProducts = new DatabaseContext().DeliveryProducts.Where(d=> d.Delivery.Provider.Id == provider.Id).ToList();
            _viewData = new List<ViewData>();

            foreach (var deliveryProduct in deliveryProducts)
            {
                _viewData.Add(new ViewData()
                {
                    Date = deliveryProduct.Delivery.Date.ToString("dd.MM.yyyy"),
                    Product = deliveryProduct.Product.Name,
                    Storage = deliveryProduct.Delivery.Storage.Address,
                    Quantity = deliveryProduct.Count,
                    Price = deliveryProduct.Price
                });
            }

            deliveryDataGridView.DataSource = _viewData;
        }

        public void InitializeData(User user)
        {
            _user = user;
        }

        private void GoBack()
        {
            ReportsForm mainForm = new ReportsForm();
            mainForm.InitializeData(_user);
            mainForm.Show();
            Hide();
        }

        private void goBackButton_Click(object sender, EventArgs e)
        {
            GoBack();
        }

        private void storageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshDataGridView();
        }

        private void exportToExcelButton_Click(object sender, EventArgs e)
        {
            const string excelTemplatePath = "ProviderDeliveriesTemplate.xlsx";
            
            Workbook workbookForDataTable = new Workbook();

            // Create a sample DataTable for the student
            DataTable viewDataTable = new DataTable("ViewData");

            // Add multiple columns in the newly created DataTable
            viewDataTable.Columns.Add("Date", typeof(string));
            viewDataTable.Columns.Add("Product", typeof(string));
            viewDataTable.Columns.Add("Storage address", typeof(string));
            viewDataTable.Columns.Add("Quantity", typeof(int));
            viewDataTable.Columns.Add("Price", typeof(double));

            foreach (ViewData view in _viewData)
            {
                DataRow dataRow = viewDataTable.NewRow();
                dataRow["Date"] = view.Date;
                dataRow["Product"] = view.Product;
                dataRow["Storage address"] = view.Storage;
                dataRow["Quantity"] = view.Quantity;
                dataRow["Price"] = view.Price;

                viewDataTable.Rows.Add(dataRow);
            }

            ImportTableOptions importOptions = new ImportTableOptions();
            Worksheet dataTableWorksheet = workbookForDataTable.Worksheets[0];
            dataTableWorksheet.Cells.ImportData(viewDataTable, 0, 0, importOptions);
            dataTableWorksheet.AutoFitColumns();
            workbookForDataTable.Save(excelTemplatePath);

            FileInfo fileInfo = new FileInfo(excelTemplatePath);
            if (!fileInfo.Exists)
            {
                MessageBox.Show("Export to excel failed", "Error", MessageBoxButtons.OK);
                return;
            }

            System.Diagnostics.Process.Start(excelTemplatePath);

        }
    }
}
