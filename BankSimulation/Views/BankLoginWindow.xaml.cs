using BankSimulation.Models;
using BankSimulation.Models.EF;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BankSimulation
{
    /// <summary>
    /// Interaction logic for BankLoginWindow.xaml
    /// </summary>
    public partial class BankLoginWindow : Window
    {
        public BankLoginWindow()
        {
            InitializeComponent();
            UpdateLabelBankLoginWelcomeMessage(LoggedInUser.Bank);
            UpdateLabelBankAccountMoneyBalance(LoggedInUser.Bank);
            UpdateLabelCentralBankMoneyBalance(LoggedInUser.Bank);
        }

        // Havidij, Hitelkamatdij terhelés gombok
        // Havidíj terhelés
        private void MonthlyFeeCharge_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new BankDBEntities())
            {
                var customers = (from c in context.Customers
                                join b in context.Banks on c.BankID equals b.BankID
                                where b.BankID == LoggedInUser.Bank.BankID
                                select c).ToList();

                var selectedBank = context.Banks.SingleOrDefault(b => b.BankID == LoggedInUser.Bank.BankID);
                double amount = 300;

                var transactionCustomer = new CustomerTransaction();
                var transactionBank = new BankTransaction();

                foreach (var customer in customers)
                {

                    context.CustomerTransactions.Add(new CustomerTransaction()
                    {
                        //CustomerTransactionType = "Havidíj terhelés",
                        CustomerTransactionType = CustomerTransactionType.MonthlyCharge.ToString(),
                        CustomerID = customer.CustomerID,
                        Date = DateTime.Now,
                        CustomerTransactionAmount = -amount,
                        CustomerTransactionCurrency = customer.CustomerAccountNumberCurrency,
                    });
                    context.BankTransactions.Add(new BankTransaction()
                    {
                        BankID = selectedBank.BankID,
                        Date = DateTime.Now,
                        //BankTransactionType = "Havidíj bevétel",
                        BankTransactionType = BankTransactionType.MonthlyIncome.ToString(),
                        BankTransactionAmount = amount,
                        BankTransactionCurrency = selectedBank.BankAccountMoneyCurrency
                    });

                    customer.CustomerAccountNumberBalance -= amount;
                    selectedBank.CentralBankMoneyBalance += amount;
                }

                context.SaveChanges();
                LoggedInUser.SetBank(selectedBank);
                UpdateLabelBankAccountMoneyBalance(selectedBank);
                UpdateLabelCentralBankMoneyBalance(selectedBank);
                MessageBox.Show("Havidíjak terhelve");
            }
        }

        //Hitel kamatdíj terhelés
        private void AffectedInterestRateOnLoans_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new BankDBEntities())
            {
                var customers = (from c in context.Customers
                                join b in context.Banks on c.BankID equals b.BankID
                                where (b.BankID == LoggedInUser.Bank.BankID) && (c.CustomerCreditAccountBalance < 0.0)
                                select c).ToList();

                var selectedBank = context.Banks.SingleOrDefault(b => b.BankID == LoggedInUser.Bank.BankID);

                double amount = 0;

                foreach (var customer in customers)
                {
                    amount += customer.CustomerCreditAccountBalance * -0.015;

                    context.CustomerTransactions.Add(new CustomerTransaction()
                    {
                        CustomerTransactionType = CustomerTransactionType.CreditInterestCharge.ToString(),
                        CustomerID = customer.CustomerID,
                        Date = DateTime.Now,
                        CustomerTransactionAmount = -amount,
                        CustomerTransactionCurrency = customer.CustomerAccountNumberCurrency,
                    });
                    context.BankTransactions.Add(new BankTransaction()
                    {
                        BankID = selectedBank.BankID,
                        Date = DateTime.Now,
                        BankTransactionType = BankTransactionType.CreditInterestIncome.ToString(),
                        BankTransactionAmount = amount,
                        BankTransactionCurrency = selectedBank.BankAccountMoneyCurrency
                    });

                    customer.CustomerAccountNumberBalance -= amount;
                    selectedBank.CentralBankMoneyBalance += amount;
                }

                context.SaveChanges();
                LoggedInUser.SetBank(selectedBank);
                UpdateLabelBankAccountMoneyBalance(selectedBank);
                UpdateLabelCentralBankMoneyBalance(selectedBank);
                MessageBox.Show("Hitelkamat terhelve");
            }
        }

        // Hiteligénylés
        private void LoanRequest_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new BankDBEntities())
            {
                var accountHistory = (from t in context.CustomerTransactions
                                      join c in context.Customers on t.CustomerID equals c.CustomerID
                                      join b in context.Banks on c.BankID equals b.BankID
                                      where c.BankID == LoggedInUser.Bank.BankID && t.CustomerTransactionType == CustomerTransactionType.LoanApplication.ToString() && t.LoanFromBank == 1
                                      select new { t.Date, c.CustomerName, t.CustomerTransactionType, t.CustomerTransactionAmount, t.CustomerTransactionCurrency, c.CustomerAccountNumberBalance, c.CustomerCreditAccountBalance }).ToList();

                DataGridLoanRequest.ItemsSource = accountHistory;

                if (accountHistory.Count == 0)
                {
                    DataGridLoanRequest.Visibility = Visibility.Hidden;
                    return;
                } else
                {
                    
                    DataGridLoanRequest.Visibility = Visibility.Visible;
                    buttonGrid.Visibility = Visibility.Hidden;

                    DataGridLoanRequest.Columns[0].Header = "Dátum";
                    DataGridLoanRequest.Columns[1].Header = "Ügyfél neve";
                    DataGridLoanRequest.Columns[2].Header = "Tranzakció típusa";
                    DataGridLoanRequest.Columns[3].Header = "Kért összeg";
                    DataGridLoanRequest.Columns[4].Header = "Deviza";
                    DataGridLoanRequest.Columns[5].Header = "Számlaszám egyenlege";
                    DataGridLoanRequest.Columns[6].Header = "Hitelszámla egyenlege";
                }
            }
        }

        private void AcceptLoanRequest_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridLoanRequest.SelectedIndex < 0)
            {
                return;
            }
            using (var context = new BankDBEntities())
            {
                var listofLoanRequest = (from t in context.CustomerTransactions
                                        join c in context.Customers on t.CustomerID equals c.CustomerID
                                        join b in context.Banks on c.BankID equals b.BankID
                                        where c.BankID == LoggedInUser.Bank.BankID && t.CustomerTransactionType == CustomerTransactionType.LoanApplication.ToString() && t.LoanFromBank == 1
                                        select new { t.CustomerTransactionType, t.Date, t.CustomerTransactionAmount, t.CustomerTransactionCurrency, t.CustomerTransactionID, t.CustomerID }).ToList();
                if (listofLoanRequest == null)
                {
                    return;
                }

                int id = listofLoanRequest[DataGridLoanRequest.SelectedIndex].CustomerTransactionID;
                var getTransaction = context.CustomerTransactions
                    .Where(t => t.CustomerTransactionID == id)
                    .FirstOrDefault();

                if (getTransaction == null)
                {
                    return;
                }
                getTransaction.LoanFromBank = 2;
                getTransaction.CustomerTransactionType = CustomerTransactionType.LoanApplicationAccepted.ToString();

                int customerId = listofLoanRequest[DataGridLoanRequest.SelectedIndex].CustomerID;
                var customer = context.Customers.Where(c => c.CustomerID == customerId).FirstOrDefault();
                customer.CustomerCreditAccountBalance -= listofLoanRequest[DataGridLoanRequest.SelectedIndex].CustomerTransactionAmount;
                customer.CustomerAccountNumberBalance += listofLoanRequest[DataGridLoanRequest.SelectedIndex].CustomerTransactionAmount;

                var selectedBank = context.Banks.Where(b => b.BankID == LoggedInUser.Bank.BankID).FirstOrDefault();
                selectedBank.BankAccountMoneyBalance += listofLoanRequest[DataGridLoanRequest.SelectedIndex].CustomerTransactionAmount;

                context.SaveChanges();

                LoggedInUser.SetBank(selectedBank);
                UpdateLabelBankAccountMoneyBalance(selectedBank);
                UpdateLabelCentralBankMoneyBalance(selectedBank);
                
                LoanRequest_Click(null, null);
            }
        }

        private void RejectLoanRequest_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridLoanRequest.SelectedIndex < 0)
            {
                return;
            }
            using (var context = new BankDBEntities())
            {
                var listofLoanRequest = (from t in context.CustomerTransactions
                                        join c in context.Customers on t.CustomerID equals c.CustomerID
                                        join b in context.Banks on c.BankID equals b.BankID
                                        where c.BankID == LoggedInUser.Bank.BankID && t.CustomerTransactionType == CustomerTransactionType.LoanApplication.ToString() && t.LoanFromBank == 1
                                        select new { t.CustomerTransactionType, t.Date, t.CustomerTransactionAmount, t.CustomerTransactionCurrency, t.CustomerTransactionID, t.CustomerID }).ToList();
                if (listofLoanRequest == null)
                {
                    return;
                }

                int id = listofLoanRequest[DataGridLoanRequest.SelectedIndex].CustomerTransactionID;
                var getTransaction = context.CustomerTransactions
                    .Where(t => t.CustomerTransactionID == id)
                    .FirstOrDefault();
                if (getTransaction == null)
                {
                    return;
                }
                getTransaction.LoanFromBank = 3;
                getTransaction.CustomerTransactionType = CustomerTransactionType.LoanApplicationRejected.ToString();
                context.SaveChanges();
                LoanRequest_Click(null, null);
            }
        }

        // Számlatörténet
        private void AccountHistory_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new BankDBEntities())
            {
                //var accountHistory = (from t in context.BankTransactions
                //                      join b in context.Banks on t.BankID equals b.BankID
                //                      where b.BankID == LoggedInUser.Bank.BankID
                //                      select new 
                //                      { 
                //                          t.Date, 
                //                          t.BankTransactionType, 
                //                          t.BankTransactionAmount, 
                //                          t.BankTransactionCurrency
                //                      })
                //                      .ToList();

                var accountHistory = (from t in context.BankTransactions
                                      where t.BankID == LoggedInUser.Bank.BankID
                                      select new
                                      {
                                          t.Date,
                                          t.BankTransactionType,
                                          t.BankTransactionAmount,
                                          t.BankTransactionCurrency
                                      })
                                      .ToList();

                DataGridBankAccountHistory.ItemsSource = accountHistory;
                DataGridBankAccountHistory.Columns[0].Header = "Dátum";
                DataGridBankAccountHistory.Columns[1].Header = "Tranzakció típusa";
                DataGridBankAccountHistory.Columns[2].Header = "Összeg";
                DataGridBankAccountHistory.Columns[3].Header = "Deviza";
            }
        }

        private void DataGridLoanRequest_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridLoanRequest.SelectedIndex < 0)
            {
                buttonGrid.Visibility = Visibility.Hidden;
            }
            else
            {
                buttonGrid.Visibility = Visibility.Visible;
            }
        }

        // Banki felület címkéinek naprakészen tartása függvényekkel
        private void UpdateLabelBankLoginWelcomeMessage(Bank b)
        {
            LabelBankLoginWelcomeMessage.Content = $"{b.BankName} bejelentkezve";
        }

        private void UpdateLabelBankAccountMoneyBalance(Bank b)
        {
            LabelBankAccountMoneyBalance.Content = $"{b.BankAccountMoneyBalance.ToString("N", CultureInfo.GetCultureInfo("ru-RU"))} Ft";
        }

        private void UpdateLabelCentralBankMoneyBalance(Bank b)
        {
            LabelCentralBankMoneyBalance.Content = $"{b.CentralBankMoneyBalance.ToString("N", CultureInfo.GetCultureInfo("ru-RU"))} Ft";
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

        private void TextBlockBankWindowName_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            using (var context = new BankDBEntities())
            {

                var linkedCustomersToLoggedInBank = (from b in context.Banks
                                                    join c in context.Customers on b.BankID equals c.BankID
                                                    where b.BankID == LoggedInUser.Bank.BankID
                                                    select new { Bank = b, Customer = c }).ToList();

                string customerNameList = $"{LoggedInUser.Bank.BankName} ügyfelei:\n(összesen = {LoggedInUser.Bank.NumberOfCustomers} db)\n\n";
                var index = 0;

                foreach (var customer in linkedCustomersToLoggedInBank)
                {
                    customerNameList += customer.Customer.CustomerName;
                    if (index < linkedCustomersToLoggedInBank.Count - 1) 
                    {
                        customerNameList += ", ";
                    }
                    index++;
                }
                MessageBox.Show(customerNameList);
            }
        }
    }
}
