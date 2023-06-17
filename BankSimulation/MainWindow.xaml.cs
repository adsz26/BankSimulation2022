using BankSimulation.Models.EF;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace BankSimulation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Regisztráció
        private void ButtonRegistrationWindow_Click(object sender, RoutedEventArgs e)
        {
           CustomerRegistrationWindow customerRegistrationWindow = new CustomerRegistrationWindow();
           this.Close();
           customerRegistrationWindow.Show();
        }

        // Bejelentkezés
        private void ButtonLoginWindow_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new BankDBEntities())
            {
                Customer customer = context.Customers.FirstOrDefault(c => c.CustomerName == TextBoxUserName.Text && c.CustomerPassword == PasswordBoxPassword.Password.ToString());
                Bank bank = context.Banks.FirstOrDefault(b => b.BankName == TextBoxUserName.Text && b.BankPassword == PasswordBoxPassword.Password.ToString());

                if (customer == null && bank == null)
                {
                    MessageBox.Show("Nincs ilyen Ugyfel/Bank az adatbázisban!");
                    ResetLoginFields();
                }
                else if (customer != null)
                {
                    LoggedInUser.SetCustomer(customer);
                    CustomerLoginWindow customerLoginWindow = new CustomerLoginWindow();
                    this.Close();
                    customerLoginWindow.Show();
                }
                else
                {
                    LoggedInUser.SetBank(bank);
                    BankLoginWindow bankLoginWindow = new BankLoginWindow();
                    this.Close();
                    bankLoginWindow.Show();
                }
            }
        }

        // Ablak mozgatás, bezárás funkciók
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void AppClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ResetLoginFields()
        {
            TextBoxUserName.Text = "";
            PasswordBoxPassword.Password = "";
        }
    }
}
