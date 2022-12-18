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
    public partial class DeliveredProductsInStoragesForm : KryptonForm
    {
        class ViewData
        {
            public DateTime Date { get; set; }

            public string Provider { get; set; }

            public string Product { get; set; }

            public int Quantity { get; set; }

            public double Price { get; set; }
        }
        
        private User _user;

        List<ViewData> _viewData = new List<ViewData>();

        public DeliveredProductsInStoragesForm()
        {
            InitializeComponent();

            dateTimePicker.Value = DateTime.Now;
            storageComboBox.DataSource = new DatabaseContext().Storages.ToList();

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
            Storage storage = storageComboBox.SelectedItem as Storage;
            if (storage == null)
                return;

            List<DeliveryProduct> deliveries = new DatabaseContext().DeliveryProducts.Where(d=> DbFunctions.TruncateTime(d.Delivery.Date) <= dateTimePicker.Value.Date && d.Delivery.Storage.Id == storage.Id).ToList();
            _viewData = new List<ViewData>();

            foreach (var delivery in deliveries)
            {
                _viewData.Add(new ViewData()
                {
                    Date = delivery.Delivery.Date,
                    Provider = delivery.Delivery.Provider.Name,
                    Product = delivery.Product.Name,
                    Quantity = delivery.Count,
                    Price = delivery.Price,
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

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            RefreshDataGridView();
        }

        private void storageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshDataGridView();
        }

        private void exportToExcelButton_Click(object sender, EventArgs e)
        {
            Workbook workbookForDataTable = new Workbook();

            DataTable viewDataTable = new DataTable("ViewData");

            viewDataTable.Columns.Add("Delivery Date", typeof(string));
            viewDataTable.Columns.Add("Provider", typeof(string));
            viewDataTable.Columns.Add("Product", typeof(string));
            viewDataTable.Columns.Add("Quantity", typeof(int));
            viewDataTable.Columns.Add("Price", typeof(double));

            foreach (ViewData view in _viewData)
            {
                DataRow dataRow = viewDataTable.NewRow();
                dataRow["Delivery Date"] = view.Date.ToString(CultureInfo.InvariantCulture);
                dataRow["Provider"] = view.Provider;
                dataRow["Product"] = view.Product;
                dataRow["Quantity"] = view.Quantity;
                dataRow["Price"] = view.Price;

                viewDataTable.Rows.Add(dataRow);
            }

            ImportTableOptions importOptions = new ImportTableOptions();
            Worksheet dataTableWorksheet = workbookForDataTable.Worksheets[0];
            dataTableWorksheet.Cells.ImportData(viewDataTable, 0, 0, importOptions);
            dataTableWorksheet.AutoFitColumns();
            workbookForDataTable.Save("DeliveredProductsInStorages.xlsx");

            FileInfo fileInfo = new FileInfo("DeliveredProductsInStorages.xlsx");
            if (!fileInfo.Exists)
            {
                MessageBox.Show("Export to excel failed", "Error", MessageBoxButtons.OK);
                return;
            }

            System.Diagnostics.Process.Start(@"DeliveredProductsInStorages.xlsx");

        }
    }
}
