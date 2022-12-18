using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using TechFirm.Models;

namespace TechFirm.View
{
    public partial class DeliveriesForm : KryptonForm, IActionHandler
    {
        class ViewData
        {
            public DateTime Date { get; set; }

            public string Provider { get; set; }

            public double Amount { get; set; }

            public Delivery Delivery;
        }
        
        private User _user;
        
        public DeliveriesForm()
        {
            InitializeComponent();

            dateTimePicker.Value = DateTime.Now;

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

            deliveryDataGridView.MouseClick += DeliveryDataGridViewMouseClick;
        }

        void RefreshDataGridView()
        {
            List<Delivery> deliveries = new DatabaseContext().Deliveries
                .Where(d=> DbFunctions.TruncateTime(d.Date) == dateTimePicker.Value.Date)
                .ToList();
            
            List<ViewData> viewData = new List<ViewData>();

            foreach (var delivery in deliveries)
            {
                double totalPrice = 0;

                try
                {
                    totalPrice = new DatabaseContext().DeliveryProducts
                        .Where(x => x.Delivery.Id == delivery.Id)
                        .Sum(x => x.Price * x.Count);
                }
                catch (Exception)
                {
                    totalPrice = 0;
                }
                
                viewData.Add(new ViewData()
                {
                    Date = delivery.Date,
                    Provider = delivery.Provider.Name,
                    Amount = totalPrice,
                    Delivery = delivery
                });
            }

            deliveryDataGridView.DataSource = viewData;
        }

        private void DeliveryDataGridViewMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                return;

            ContextMenuStrip menu = new ContextMenuStrip();
            int position = deliveryDataGridView.HitTest(e.X, e.Y).RowIndex;

            if (position >= 0)
            {
                deliveryDataGridView.ClearSelection();
                deliveryDataGridView.Rows[position].Selected = true;
                menu.Items.Add("View").Name = "View";
            }
            else
            {
                menu.Items.Add("Add").Name = "Add";
            }

            menu.Show(deliveryDataGridView, new Point(e.X, e.Y));
            menu.ItemClicked += Menu_ItemClicked;
        }

        private void Menu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch ( e.ClickedItem.Name )
                {
                    case "Add":
                        AddRecord();
                break;
                    case "View":
                        ViewRecord();
                break;
                }
        }

        private void ViewRecord()
        {
            ViewData viewData = deliveryDataGridView.SelectedRows[0].DataBoundItem as ViewData;
            if (viewData == null)
                return;

            DeliveryForm deliveryForm = new DeliveryForm();
            deliveryForm.InitializeData(viewData.Delivery, DialogMode.Preview, this);
            deliveryForm.Show();
        }

        private void AddRecord()
        {
            DeliveryForm deliveryForm = new DeliveryForm();
            deliveryForm.InitializeData( DialogMode.Add, this );
            deliveryForm.ShowDialog();
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

        private void deliveriesGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            ViewRecord();
        }

        public void OnSaveAction()
        {
            RefreshDataGridView();
        }

        private void addRecordButton_Click(object sender, EventArgs e)
        {
            AddRecord();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            RefreshDataGridView();
        }
    }
}
