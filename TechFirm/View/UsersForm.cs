using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Aspose.Cells;
using ComponentFactory.Krypton.Toolkit;
using TechFirm.Models;

namespace TechFirm.View
{
    public partial class UsersForm : KryptonForm
    {
        class ViewData
        {
            public string Email { get; set; }
            
            public string Name { get; set; }
            
            public string CreationDate { get; set; }

            public string LastLoginDate { get; set; }
        }
        
        private User _user;

        List<ViewData> _viewData = new List<ViewData>();

        public UsersForm()
        {
            InitializeComponent();

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

            deliveryDataGridView.Columns["CreationDate"].HeaderText = "Creation Date";
            deliveryDataGridView.Columns["LastLoginDate"].HeaderText = "Last Login Date";
        }

        void RefreshDataGridView()
        {
            List<User> users = new DatabaseContext().Users.Where(d=> d.Email.Contains(userTextBox.Text)).ToList();
            _viewData = new List<ViewData>();

            foreach (var user in users)
            {
                _viewData.Add(new ViewData()
                {
                    Email = user.Email,
                    Name = user.FirstName + " " + user.LastName,
                    CreationDate = user.AccountInformation.CreationDate.ToString("dd.MM.yyyy"),
                    LastLoginDate = user.AccountInformation.LastLoginDate.ToString("dd.MM.yyyy HH:mm:ss")
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

        private void exportToExcelButton_Click(object sender, EventArgs e)
        {
            const string excelTemplatePath = "UsersTemplate.xlsx";
            
            Workbook workbookForDataTable = new Workbook();

            // Create a sample DataTable for the student
            DataTable viewDataTable = new DataTable("ViewData");

            // Add multiple columns in the newly created DataTable
            viewDataTable.Columns.Add("Email", typeof(string));
            viewDataTable.Columns.Add("Name", typeof(string));
            viewDataTable.Columns.Add("Creation date", typeof(string));
            viewDataTable.Columns.Add("Last login date", typeof(string));

            foreach (ViewData view in _viewData)
            {
                DataRow dataRow = viewDataTable.NewRow();
                dataRow["Email"] = view.Email;
                dataRow["Name"] = view.Name;
                dataRow["Creation date"] = view.CreationDate;
                dataRow["Last login date"] = view.LastLoginDate;

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

        private void userTextBox_TextChanged(object sender, EventArgs e)
        {
            RefreshDataGridView();
        }
    }
}
