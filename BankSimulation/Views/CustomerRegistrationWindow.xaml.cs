using BankSimulation.Models.EF;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace BankSimulation
{
    /// <summary>
    /// Interaction logic for CustomerRegistrationWindow.xaml
    /// </summary>
    public partial class CustomerRegistrationWindow : Window
    {
        readonly List<string> errors = new List<string>();

        public CustomerRegistrationWindow()
        {
            InitializeComponent();
            GetBanksIdForComboBox();
        }
        // Regisztráció
        private void BtnRegistration_Click(object sender, RoutedEventArgs e)
        {
            errors.Clear();
            ListBoxErrors.Items.Clear();

            using (var context = new BankDBEntities())
            {
                var selectedBankId = context.Banks.Where(b => b.BankName == ComboBoxBankId.Text).Select(b => b.BankID).First();
                double customerAccountNumberBalance = 0, customerCreditAccountBalance = 0;
                if(TextBoxCustomerAccountNumberBalance.Text != "")
                {
                    customerAccountNumberBalance = double.Parse(TextBoxCustomerAccountNumberBalance.Text);
                }
                if(TextBoxCustomerCreditAccountBalance.Text != "")
                {
                    customerCreditAccountBalance = double.Parse(TextBoxCustomerCreditAccountBalance.Text);
                }

                var customer = new Customer()
                {
                    CustomerName = TextBoxCustomerName.Text,
                    CustomerPassword = PasswordBoxCustomerPassword.Password.ToString(),
                    CustomerAccountNumber = TextBoxCustomerAccountNumber.Text,
                    CustomerAccountNumberCurrency = ComboBoxSzamlaszamDeviza.Text,
                    CustomerAccountNumberBalance = customerAccountNumberBalance,
                    CustomerCreditAccount = TextBoxCustomerCreditAccount.Text,
                    CustomerCreditAccountCurrency = ComboBoxCustomerCreditAccountCurrency.Text,
                    CustomerCreditAccountBalance = customerCreditAccountBalance,
                    BankID = selectedBankId
                };

                CustomerValidator validator = new CustomerValidator();
                var results = validator.Validate(customer);

                if (results.IsValid == false)
                {
                    foreach (ValidationFailure failure in results.Errors)
                    {
                        errors.Add(failure.ErrorMessage);
                    }
                    foreach (var err in errors)
                    {
                        ListBoxErrors.Items.Add(err);
                    }
                    if (LabelErrors.Visibility == Visibility.Hidden && ListBoxErrors.Visibility == Visibility.Hidden)
                    {
                        LabelErrors.Visibility = Visibility.Visible;
                        ListBoxErrors.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    if (LabelErrors.Visibility == Visibility.Visible && ListBoxErrors.Visibility == Visibility.Visible)
                    {
                        LabelErrors.Visibility = Visibility.Hidden;
                        ListBoxErrors.Visibility = Visibility.Hidden;
                    }

                    context.Customers.Add(customer);
                    context.SaveChanges();

                    MessageBox.Show("Ügyfél hozzáadása az adatbázishoz sikeres.");
                    ResetCustomerRegistrationFields();

                };
                

            }
        }

        private void BtnRegistrationCancel_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            this.Close();
            mainWindow.Show();
        }

        private void GetBanksIdForComboBox()
        {
            try
            {
                using (var context = new BankDBEntities())
                {
                    var listOfBankNames = context.Banks.Select(b => b.BankName).ToList();
                    if (listOfBankNames != null)
                    {
                        foreach (var item in listOfBankNames)
                        {
                            ComboBoxBankId.Items.Add(item);
                        }
                        ComboBoxBankId.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("ERROR: " + exception.Message);
            }
        }

        private void ResetCustomerRegistrationFields()
        {
            TextBoxCustomerName.Text = "";
            PasswordBoxCustomerPassword.Password = "";
            TextBoxCustomerAccountNumber.Text = "";
            TextBoxCustomerAccountNumberBalance.Text = "";
            TextBoxCustomerCreditAccount.Text = "";
            TextBoxCustomerCreditAccountBalance.Text = "";
        }

        // Validáció ellenőrzése összeg megadásánál
        private void OnlyInteger_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[^0-9]*$");
            e.Handled = regex.IsMatch(e.Text);
        }

        // Space nem engedélyezett a karakterek között
        private void Space_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void TextBoxCustomerAccountNumber_PreviewKeyDown(object sender, KeyEventArgs e) 
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
            else if (TextBoxCustomerAccountNumber.Text.Length == 8 && e.Key != Key.Back && e.Key != Key.Delete)
            {
                TextBoxCustomerAccountNumber.Text += "-";
                TextBoxCustomerAccountNumber.CaretIndex = TextBoxCustomerAccountNumber.Text.Length;
            }
        }

        private void TextBoxCustomerCreditAccountNumber_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
            else if (TextBoxCustomerCreditAccount.Text.Length == 8 && e.Key != Key.Back && e.Key != Key.Delete)
            {
                TextBoxCustomerCreditAccount.Text += "-";
                TextBoxCustomerCreditAccount.CaretIndex = TextBoxCustomerAccountNumber.Text.Length;
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
    }
}
