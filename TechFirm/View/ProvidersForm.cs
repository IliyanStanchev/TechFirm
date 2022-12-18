using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using TechFirm.Models;

namespace TechFirm.View
{
    public partial class ProvidersForm : KryptonForm, IActionHandler
    {
        private User _user;
        
        public ProvidersForm()
        {
            InitializeComponent();
            InitializeDataGridView();
        }

        void InitializeDataGridView()
        {
            providersDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;
            providersDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            providersDataGridView.AutoGenerateColumns = true;
            providersDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            providersDataGridView.RowHeadersVisible = false;

            providersDataGridView.RowTemplate.Height = 50;

            RefreshDataGridView();
            
            providersDataGridView.MouseClick += ProvidersDataGridViewMouseClick;
        }

        private void RefreshDataGridView()
        {
            providersDataGridView.DataSource = new DatabaseContext().Providers.Where(p => p.Name.Contains( nameTextBox.Text)).ToList();
        }

        private void ProvidersDataGridViewMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                return;

            ContextMenuStrip menu = new ContextMenuStrip();
            int position = providersDataGridView.HitTest(e.X, e.Y).RowIndex;

            if (position >= 0)
            {
                providersDataGridView.ClearSelection();
                providersDataGridView.Rows[position].Selected = true;
                menu.Items.Add("Edit").Name = "Edit";
                menu.Items.Add("Delete").Name = "Delete";
            }
            else
            {
                menu.Items.Add("Add").Name = "Add";
            }

            menu.Show(providersDataGridView, new Point(e.X, e.Y));
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
            var product = providersDataGridView.SelectedRows[0].DataBoundItem as Provider;
            if (product == null)
                return;

            ProviderForm productForm = new ProviderForm();
            productForm.InitializeData(product, DialogMode.Edit, this);
            productForm.Show();
        }

        private void DeleteRecord()
        {
            if (MessageBox.Show("Are you sure", "", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            Provider product = providersDataGridView.SelectedRows[0].DataBoundItem as Provider;
            if (product == null)
            {
                return;
            }

            using (var db = new DatabaseContext())
            {
                Provider dbProvider = db.Providers.FirstOrDefault(s => s.Id == product.Id);

                if (dbProvider != null)
                {
                    db.Providers.Remove(dbProvider);
                    db.SaveChanges();
                }
            }

            providersDataGridView.DataSource = new DatabaseContext().Providers.ToList();
        }

        private void AddRecord()
        {
            ProviderForm productForm = new ProviderForm();
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
            
            Provider product = (Provider)providersDataGridView.Rows[e.RowIndex].DataBoundItem;
            if (product == null)
                return;

            ProviderForm productForm = new ProviderForm();
            productForm.InitializeData(product, DialogMode.Preview, this);
            productForm.Show();

        }

        public void OnSaveAction()
        {
            providersDataGridView.DataSource = new DatabaseContext().Providers.ToList();
        }

        private void addRecordButton_Click(object sender, EventArgs e)
        {
            AddRecord();
        }

        private void nameTextBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
