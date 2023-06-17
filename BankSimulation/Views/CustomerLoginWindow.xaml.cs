using BankSimulation.Models;
using BankSimulation.Models.EF;
using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace BankSimulation
{
    /// <summary>
    /// Interaction logic for CustomerLoginWindow.xaml
    /// </summary>
    public partial class CustomerLoginWindow : Window
    {
        public CustomerLoginWindow()
        {
            InitializeComponent();
            UpdateLabelCustomerLoginWelcomeMessage(LoggedInUser.Customer);
            UpdateLabelCustomerAccountNumberBalance(LoggedInUser.Customer);
            UpdateLabelCustomerCreditAccountBalance(LoggedInUser.Customer);
            TransferSelectCustomer();
        }

        // Készpénzfelvétel
        private void CashWithdrawal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double amount = Double.Parse(TextBoxCashWithdrawal.Text);

                if (amount <= LoggedInUser.Customer.CustomerAccountNumberBalance)
                {
                    using (var context = new BankDBEntities())
                    {

                        var customer = context.Customers.Single(c => c.CustomerID == LoggedInUser.Customer.CustomerID);
                        var bank = context.Banks.SingleOrDefault(b => b.BankID == customer.BankID);
                        customer.CustomerAccountNumberBalance -= amount + amount*0.01;

                        var objTransaction1 = new CustomerTransaction()
                        {
                            //CustomerTransactionType = "Keszpenzfelvetel",
                            CustomerTransactionType = CustomerTransactionType.CashWithdrawal.ToString(),
                            CustomerID = customer.CustomerID,
                            Date = DateTime.Now,
                            CustomerTransactionAmount = -amount,
                            CustomerTransactionCurrency = customer.CustomerAccountNumberCurrency,
                        };

                        var objTransaction2 = new CustomerTransaction()
                        {
                            CustomerID = customer.CustomerID,
                            //CustomerTransactionType = "Készpénzfelvétel díja",
                            CustomerTransactionType = CustomerTransactionType.CashWithdrawalFee.ToString(),
                            Date = DateTime.Now,
                            CustomerTransactionAmount = -(amount * 0.01),
                            CustomerTransactionCurrency = customer.CustomerAccountNumberCurrency
                        };

                        var objTransaction3 = new BankTransaction()
                        {
                            Date = DateTime.Now,
                            //BankTransactionType = "Készpénzfelvitel díj elszámolás",
                            BankTransactionType = BankTransactionType.CashWithdrawalCharge.ToString(),
                            BankTransactionAmount = amount,
                            BankTransactionCurrency = bank.BankAccountMoneyCurrency
                        };

                        var objTransaction4 = new BankTransaction()
                        {
                            Date = DateTime.Now,
                            //BankTransactionType = "Készpénzfelvétel ügyfél számláról",
                            BankTransactionType = BankTransactionType.CashWithdrawalFromCustomerAccount.ToString(),
                            BankTransactionAmount = -amount,
                            BankTransactionCurrency = bank.BankAccountMoneyCurrency
                        };

                        bank.BankAccountMoneyBalance -= amount;
                        bank.CentralBankMoneyBalance += amount * 0.01;
                        

                        context.CustomerTransactions.Add(objTransaction1);
                        context.CustomerTransactions.Add(objTransaction2);
                        context.BankTransactions.Add(objTransaction3);
                        context.BankTransactions.Add(objTransaction4);
                        context.SaveChanges();

                        LoggedInUser.SetCustomer(customer);
                        UpdateLabelCustomerAccountNumberBalance(LoggedInUser.Customer);
                        UpdateLabelCustomerCreditAccountBalance(LoggedInUser.Customer);

                        MessageBox.Show("Készpénzfelvétel sikeresen végrehajtva!");
                        TextBoxCashWithdrawal.Text = "";

                        //AccountHistory_Click(null, null);
                    }
                }
                else
                {
                    MessageBox.Show("Nincs elég fedezet a számlán.");
                    TextBoxCashWithdrawal.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("HIBA: " + ex.Message);
                TextBoxCashWithdrawal.Text = "";
            }


        }

        // Hitelfelvétel
        private void LoanApplication_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double amount = Double.Parse(TextBoxLoanApplication.Text);
                using (var context = new BankDBEntities())
                {

                    Customer customer = context.Customers.Single(c => c.CustomerID == LoggedInUser.Customer.CustomerID);

                    var objTransaction = new CustomerTransaction()
                    {
                        //CustomerTransactionType = "Hitelfelvetel",
                        CustomerTransactionType = CustomerTransactionType.LoanApplication.ToString(),
                        CustomerID = customer.CustomerID,
                        Date = DateTime.Now,
                        CustomerTransactionAmount = amount,
                        CustomerTransactionCurrency = customer.CustomerCreditAccountCurrency,
                        LoanFromBank = 1
                    };

                    context.CustomerTransactions.Add(objTransaction);
                    context.SaveChanges();

                    LoggedInUser.SetCustomer(customer);
                    UpdateLabelCustomerAccountNumberBalance(LoggedInUser.Customer);
                    UpdateLabelCustomerCreditAccountBalance(LoggedInUser.Customer);

                    MessageBox.Show("Hitelfelvétel igénylés elküldve!");
                    TextBoxLoanApplication.Text = "";
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba: " + ex.Message);
                TextBoxLoanApplication.Text = "";
            }
        }

        // Átutalás
        private void Transfer_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxAmount.Text.Length > 0 && (LoggedInUser.Customer.CustomerAccountNumberBalance - double.Parse(TextBoxAmount.Text)) >= 0)
            {
                double amountToTransfer = double.Parse(TextBoxAmount.Text);
                using (var context = new BankDBEntities())
                {
                    Customer customerSender = context.Customers.Single(c => c.CustomerID == LoggedInUser.Customer.CustomerID);
                    Customer customerBeneficiary = context.Customers.SingleOrDefault(c => c.CustomerName == ComboBoxBeneficiaryName.Text && c.CustomerAccountNumber == TextBoxBeneficiaryAccountNumber.Text);
                    if (customerBeneficiary != null)
                    {
                        customerSender.CustomerAccountNumberBalance -= amountToTransfer + amountToTransfer * 0.01;
                        customerBeneficiary.CustomerAccountNumberBalance += amountToTransfer;

                        var objTransaction1 = new CustomerTransaction()
                        {
                            CustomerID = customerSender.CustomerID,
                            //CustomerTransactionType = "Átutalás",
                            CustomerTransactionType = CustomerTransactionType.Transfer.ToString(),
                            Date = DateTime.Now,
                            CustomerTransactionAmount = -amountToTransfer,
                            CustomerTransactionCurrency = customerSender.CustomerAccountNumberCurrency,
                            BeneficiaryName = ComboBoxBeneficiaryName.Text,
                            BeneficiaryAccountNumber = TextBoxBeneficiaryAccountNumber.Text
                        };

                        var objTransaction2 = new CustomerTransaction()
                        {
                            CustomerID = customerSender.CustomerID,
                            //CustomerTransactionType = "Átutalás díja",
                            CustomerTransactionType = CustomerTransactionType.TransferFee.ToString(),
                            Date = DateTime.Now,
                            CustomerTransactionAmount = -(amountToTransfer * 0.01),
                            CustomerTransactionCurrency = customerSender.CustomerAccountNumberCurrency,
                            BeneficiaryName = ComboBoxBeneficiaryName.Text,
                            BeneficiaryAccountNumber = TextBoxBeneficiaryAccountNumber.Text
                        };

                        var objTransaction3 = new CustomerTransaction()
                        {
                            CustomerID = customerBeneficiary.CustomerID,
                            //CustomerTransactionType = "Bejövő tranzakció",
                            CustomerTransactionType = CustomerTransactionType.IncomingTransaction.ToString(),
                            Date = DateTime.Now,
                            CustomerTransactionAmount = amountToTransfer,
                            CustomerTransactionCurrency = customerBeneficiary.CustomerAccountNumberCurrency,
                        };

                        Bank bankIsNotDifferent = context.Banks.Single(b => b.BankID == customerSender.BankID);
                        bankIsNotDifferent.CentralBankMoneyBalance += amountToTransfer * 0.01;

                        var objTransaction4 = new BankTransaction()
                        {
                            BankID = bankIsNotDifferent.BankID,
                            //BankTransactionType = "Átutalás díj elszámolás",
                            BankTransactionType = BankTransactionType.TransferCharge.ToString(),
                            Date = DateTime.Now,
                            BankTransactionAmount = amountToTransfer * 0.01,
                            BankTransactionCurrency = bankIsNotDifferent.BankAccountMoneyCurrency
                        };

                        context.BankTransactions.Add(objTransaction4);

                        if (customerSender.BankID != customerBeneficiary.BankID)
                        {
                            Bank bankFrom = context.Banks.Single(b => b.BankID == customerSender.BankID);
                            Bank bankTo = context.Banks.Single(b => b.BankID == customerBeneficiary.BankID);

                            var objTransaction5 = new BankTransaction()
                            {
                                BankID = bankFrom.BankID,
                                //BankTransactionType = "Átutalás",
                                BankTransactionType = BankTransactionType.Transfer.ToString(),
                                Date = DateTime.Now,
                                BankTransactionAmount = -amountToTransfer,
                                BankTransactionCurrency = bankFrom.BankAccountMoneyCurrency
                            };

                            var objTransaction6 = new BankTransaction()
                            {
                                BankID = bankTo.BankID,
                                //BankTransactionType = "Bejövő tranzakció",
                                BankTransactionType = BankTransactionType.IncomingTransaction.ToString(),
                                Date = DateTime.Now,
                                BankTransactionAmount = amountToTransfer,
                                BankTransactionCurrency = bankTo.BankAccountMoneyCurrency
                            };

                            bankFrom.BankAccountMoneyBalance -= amountToTransfer;
                            bankTo.BankAccountMoneyBalance += amountToTransfer;
                            bankFrom.CentralBankMoneyBalance -= amountToTransfer;
                            bankTo.CentralBankMoneyBalance += amountToTransfer;

                            context.BankTransactions.Add(objTransaction5);
                            context.BankTransactions.Add(objTransaction6);
                        }


                        context.CustomerTransactions.Add(objTransaction1);
                        context.CustomerTransactions.Add(objTransaction2);
                        context.CustomerTransactions.Add(objTransaction3);
                        context.SaveChanges();



                        LoggedInUser.SetCustomer(customerSender);
                        UpdateLabelCustomerAccountNumberBalance(LoggedInUser.Customer);
                        UpdateLabelCustomerCreditAccountBalance(LoggedInUser.Customer);

                        MessageBox.Show("Átutalás sikeres!");
                        TextBoxAmount.Text = "";

                    }
                    else
                    {
                        MessageBox.Show("Nincs találat az adatbázisban!");
                        TextBoxAmount.Text = "";
                    }
                }
            }
            else
            {
                MessageBox.Show("Nincs elég fedezet a számlán!");
                TextBoxAmount.Text = "";
            }
        }

        private void TransferSelectCustomer()
        {
            using (var context = new BankDBEntities())
            {
                var listofOtherCustomers = context.Customers.Where(c => c.CustomerName != LoggedInUser.Customer.CustomerName).Select(c => c).ToList();
                if (listofOtherCustomers != null)
                {
                    ComboBoxBeneficiaryName.ItemsSource = listofOtherCustomers;
                    ComboBoxBeneficiaryName.DisplayMemberPath = "CustomerName";
                }
            }
        }

        // Számlatörténet

        private void AccountHistory_Click(object sender, RoutedEventArgs e)
        {

            using (var context = new BankDBEntities())
            {
                var accountHistory = (from t in context.CustomerTransactions
                                      join c in context.Customers on t.CustomerID equals c.CustomerID
                                      where c.CustomerID == LoggedInUser.Customer.CustomerID && (t.LoanFromBank == 0 || t.LoanFromBank == 2 || t.LoanFromBank == 3)
                                      select new { t.Date, t.CustomerTransactionType, t.CustomerTransactionAmount, t.CustomerTransactionCurrency, t.BeneficiaryName }).ToList();

                DataGridAccountHistory.ItemsSource = accountHistory;
                DataGridAccountHistory.Columns[0].Header = "Dátum";
                DataGridAccountHistory.Columns[1].Header = "Tranzakció típusa";
                DataGridAccountHistory.Columns[2].Header = "Összeg";
                DataGridAccountHistory.Columns[3].Header = "Deviza";
                DataGridAccountHistory.Columns[4].Header = "Kedvezményezett neve";
                DataGridAccountHistory.Visibility = Visibility.Visible;
            }
        }

        // Ügyfél felület címkéinek naprakészen tartása függvényekkel
        private void UpdateLabelCustomerAccountNumberBalance(Customer c)
        {
            LabelCustomerAccountNumberBalance.Content = $"{c.CustomerAccountNumberBalance.ToString("N", CultureInfo.GetCultureInfo("ru-RU"))} Ft";
        }

        private void UpdateLabelCustomerCreditAccountBalance(Customer c)
        {
            LabelCustomerCreditAccountBalance.Content = $"{c.CustomerCreditAccountBalance.ToString("N", CultureInfo.GetCultureInfo("ru-RU"))} Ft";
        }

        private void UpdateLabelCustomerLoginWelcomeMessage(Customer c)
        {
            LabelCustomerLoginWelcomeMessage.Content = $"{LoggedInUser.Customer.CustomerName} bejelentkezve";
        }

        // Validáció ellenőrzése összeg megadásánál
        private void Amount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[^0-9]*$");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Space_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        // Ablak mozgatás, kijelentkezés, bezárás funkciók
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            this.Close();
            mainWindow.Show();
        }

        private void AppClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void TextBlockCustomerWindowName_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            using (var context = new BankDBEntities())
            {

                var bankLinkedToLoggedInCustomer = (from c in context.Customers
                                                    join b in context.Banks on c.BankID equals b.BankID
                                                    where c.BankID == LoggedInUser.Customer.BankID
                                                    select b).FirstOrDefault();

                MessageBox.Show($"[ {LoggedInUser.Customer.CustomerName} ] ügyfélhez rendelt bank --> [ {bankLinkedToLoggedInCustomer.BankName} ]");
            }
        }
    }
}
