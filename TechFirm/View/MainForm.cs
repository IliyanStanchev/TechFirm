using ComponentFactory.Krypton.Toolkit;
using TechFirm.Models;

namespace TechFirm.View
{
    public partial class MainForm : KryptonForm
    {
        private User _user;
        
        public MainForm()
        {
            InitializeComponent();
        }

        public void InitializeData(User employee)
        {
            _user = employee;
            welcomeLabel.Text = "Welcome, " + _user.FirstName + " " + _user.LastName;
        }

        private void loginButton_Click(object sender, System.EventArgs e)
        {
            new LoginForm().Show();
            Hide();
        }

        private void registerUserButton_Click(object sender, System.EventArgs e)
        {
            RegisterEmployeeForm registerEmployeeForm = new RegisterEmployeeForm();
            registerEmployeeForm.InitializeData(_user);
            registerEmployeeForm.Show();
            Hide();
        }

        private void storagesButton_Click(object sender, System.EventArgs e)
        {
            StoragesForm storagesForm = new StoragesForm();
            storagesForm.InitializeData(_user);
            storagesForm.Show();
            Hide();
        }

        private void productsButton_click(object sender, System.EventArgs e)
        {
            ProductsForm productsForm = new ProductsForm();
            productsForm.InitializeData(_user);
            productsForm.Show();
            Hide();
        }

        private void deliveriesButton_Click(object sender, System.EventArgs e)
        {
            DeliveriesForm deliveriesForm = new DeliveriesForm();
            deliveriesForm.InitializeData(_user);
            deliveriesForm.Show();
            Hide();
        }

        private void providersButton_Click(object sender, System.EventArgs e)
        {
            ProvidersForm providersForm = new ProvidersForm();
            providersForm.InitializeData(_user);
            providersForm.Show();
            Hide();
        }

        private void reportsButton_Click(object sender, System.EventArgs e)
        {
            ReportsForm reportsForm = new ReportsForm();
            reportsForm.InitializeData(_user);
            reportsForm.Show();
            Hide();
        }
    }
}
