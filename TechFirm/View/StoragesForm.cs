using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using TechFirm.Models;

namespace TechFirm.View
{
    public partial class StoragesForm : KryptonForm, IActionHandler
    {

        private User _user;
        
        public StoragesForm()
        {
            InitializeComponent();
            InitializeDataGridView();

        }

        void InitializeDataGridView()
        {
            storagesGridView.EditMode = DataGridViewEditMode.EditProgrammatically;
            storagesGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            storagesGridView.AutoGenerateColumns = true;
            storagesGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            storagesGridView.RowHeadersVisible = false;
            storagesGridView.RowTemplate.Height = 50;

            RefreshDataGridView();

            storagesGridView.MouseClick += StoragesGridView_MouseClick;
        }

        private void StoragesGridView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                return;

            ContextMenuStrip menu = new ContextMenuStrip();
            int position = storagesGridView.HitTest(e.X, e.Y).RowIndex;

            if (position >= 0)
            {
                storagesGridView.ClearSelection();
                storagesGridView.Rows[position].Selected = true;
                menu.Items.Add("Edit").Name = "Edit";
                menu.Items.Add("Delete").Name = "Delete";
            }
            else
            {
                menu.Items.Add("Add").Name = "Add";
            }

            menu.Show(storagesGridView, new Point(e.X, e.Y));
            menu.ItemClicked += Menu_ItemClicked;
        }

        private void Menu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch ( e.ClickedItem.Name )
                {
                    case "Add":
                        AddStorage();
                break;
                    case "Edit":
                        EditStorage();
                break;
                    case "Delete":
                        DeleteStorage();
                break;
            }
        }

        private void EditStorage()
        {
            var storage = storagesGridView.SelectedRows[0].DataBoundItem as Storage;
            if (storage == null)
                return;

            StorageForm storageForm = new StorageForm();
            storageForm.InitializeData(storage, DialogMode.Edit, this);
            storageForm.Show();
        }

        private void DeleteStorage()
        {
            if (MessageBox.Show("Are you sure", "", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            Storage storage = storagesGridView.SelectedRows[0].DataBoundItem as Storage;
            if (storage == null)
            {
                return;
            }

            using (var db = new DatabaseContext())
            {
                Storage dbStorage = db.Storages.FirstOrDefault(s => s.Id == storage.Id);

                if (dbStorage != null)
                {
                    db.Storages.Remove(dbStorage);
                    db.SaveChanges();
                }
            }

            storagesGridView.DataSource = new DatabaseContext().Storages.ToList();
        }

        private void AddStorage()
        {
            StorageForm storageForm = new StorageForm();
            storageForm.InitializeData( DialogMode.Add, this );
            storageForm.ShowDialog();
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

        private void storagesGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            
            Storage storage = (Storage)storagesGridView.Rows[e.RowIndex].DataBoundItem;
            if (storage == null)
                return;

            StorageForm storageForm = new StorageForm();
            storageForm.InitializeData(storage, DialogMode.Preview, this);
            storageForm.Show();

        }

        private void addStorageButton_Click(object sender, EventArgs e)
        {
            AddStorage();
        }

        public void OnSaveAction()
        {
            storagesGridView.DataSource = new DatabaseContext().Storages.ToList();
        }

        private void addressTextBox_TextChanged(object sender, EventArgs e)
        {
            RefreshDataGridView();
        }

        private void RefreshDataGridView()
        {
            storagesGridView.DataSource = new DatabaseContext().Storages.Where(s => s.Address.Contains(addressTextBox.Text)).ToList();
        }
    }
}
