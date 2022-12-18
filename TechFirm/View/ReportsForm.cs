using ComponentFactory.Krypton.Toolkit;
using TechFirm.Models;

namespace TechFirm.View
{
    public partial class ReportsForm : KryptonForm
    {
        private User _user;
        
        public ReportsForm()
        {
            InitializeComponent();
        }

        public void InitializeData(User employee)
        {
            _user = employee;
        }

        private void loginButton_Click(object sender, System.EventArgs e)
        {
            MainForm mainForm = new MainForm();
            mainForm.InitializeData(_user);
            mainForm.Show();
            Hide();
        }

        private void deliveredProductsInStoragesButton_Click(object sender, System.EventArgs e)
        {
            DeliveredProductsInStoragesForm deliveredProductsInStoragesForm = new DeliveredProductsInStoragesForm();
            deliveredProductsInStoragesForm.InitializeData(_user);
            deliveredProductsInStoragesForm.Show();
            Hide();
        }

        private void productDistributionButton_Click(object sender, System.EventArgs e)
        {
            ProductDistributionForm productDistributionForm = new ProductDistributionForm();
            productDistributionForm.InitializeData(_user);
            productDistributionForm.Show();
            Hide();
        }

        private void providerDeliveriesButton_Click(object sender, System.EventArgs e)
        {
            ProviderDeliveriesForm providerDeliveriesForm = new ProviderDeliveriesForm();
            providerDeliveriesForm.InitializeData(_user);
            providerDeliveriesForm.Show();
            Hide();
        }

        private void productDeliveriesButton_Click(object sender, System.EventArgs e)
        {
            ProductDeliveriesForm productDeliveriesForm = new ProductDeliveriesForm();
            productDeliveriesForm.InitializeData(_user);
            productDeliveriesForm.Show();
            Hide();
        }

        private void usersButton_Click(object sender, System.EventArgs e)
        {
            UsersForm usersForm = new UsersForm();
            usersForm.InitializeData(_user);
            usersForm.Show();
            Hide();
        }
    }
}
