using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using TechFirm.Models;

namespace TechFirm.View
{
    public partial class ProductsForm : KryptonForm, IActionHandler
    {

        private User _user;
        
        public ProductsForm()
        {
            InitializeComponent();
            InitializeDataGridView();

        }

        void InitializeDataGridView()
        {
            productsDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;
            productsDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            productsDataGridView.AutoGenerateColumns = true;
            productsDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            productsDataGridView.RowHeadersVisible = false;

            productsDataGridView.RowTemplate.Height = 50;

            RefreshDataGridView();

            productsDataGridView.MouseClick += ProductsDataGridViewMouseClick;
        }

        private void RefreshDataGridView()
        {
            productsDataGridView.DataSource = new DatabaseContext().Products.Where(p => p.Name.Contains(nameTextBox.Text)).ToList();
        }

        private void ProductsDataGridViewMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                return;

            ContextMenuStrip menu = new ContextMenuStrip();
            int position = productsDataGridView.HitTest(e.X, e.Y).RowIndex;

            if (position >= 0)
            {
                productsDataGridView.ClearSelection();
                productsDataGridView.Rows[position].Selected = true;
                menu.Items.Add("Edit").Name = "Edit";
                menu.Items.Add("Delete").Name = "Delete";
            }
            else
            {
                menu.Items.Add("Add").Name = "Add";
            }

            menu.Show(productsDataGridView, new Point(e.X, e.Y));
            menu.ItemClicked += Menu_ItemClicked;
        }

        private void Menu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch ( e.ClickedItem.Name )
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

        private void EditRecord()
        {
            var product = productsDataGridView.SelectedRows[0].DataBoundItem as Product;
            if (product == null)
                return;

            ProductForm productForm = new ProductForm();
            productForm.InitializeData(product, DialogMode.Edit, this);
            productForm.Show();
        }

        private void DeleteRecord()
        {
            if (MessageBox.Show("Are you sure", "", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            Product product = productsDataGridView.SelectedRows[0].DataBoundItem as Product;
            if (product == null)
            {
                return;
            }

            using (var db = new DatabaseContext())
            {
                Product dbProduct = db.Products.FirstOrDefault(s => s.Id == product.Id);

                if (dbProduct != null)
                {
                    db.Products.Remove(dbProduct);
                    db.SaveChanges();
                }
            }

            productsDataGridView.DataSource = new DatabaseContext().Products.ToList();
        }

        private void AddRecord()
        {
            ProductForm productForm = new ProductForm();
            productForm.InitializeData( DialogMode.Add, this );
            productForm.ShowDialog();
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

        private void productsGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            
            Product product = (Product)productsDataGridView.Rows[e.RowIndex].DataBoundItem;
            if (product == null)
                return;

            ProductForm productForm = new ProductForm();
            productForm.InitializeData(product, DialogMode.Preview, this);
            productForm.Show();

        }

        public void OnSaveAction()
        {
            productsDataGridView.DataSource = new DatabaseContext().Products.ToList();
        }

        private void addRecordButton_Click(object sender, EventArgs e)
        {
            AddRecord();
        }

        private void nameTextBox_TextChanged(object sender, EventArgs e)
        {
            RefreshDataGridView();
        }
    }
}
