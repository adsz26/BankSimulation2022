using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BankSimulation.Models
{
    // Ez a validáció nincs használva
    class ValidateCustomer: IDataErrorInfo
    {
        private string name;
        private string password;
        private string accountNumber;
        private string accountNumberCurrency;
        private double accountNumberBalance;
        private string creditAccount;
        private string creditAccountCurrency;
        private double creditAccountBalance;
        public string Name 
        {
            get { return name; }
            set { name = value; } 
        }
        public string Password 
        {
            get { return password; }
            set { password = value; }
        }
        public string AccountNumber 
        {
            get { return accountNumber; }
            set { accountNumber = value; }
        }
        public string AccountNumberCurrency
        {
            get { return accountNumberCurrency; }
            set { accountNumberCurrency = value; } 
        }
        public double AccountNumberBalance 
        { 
            get { return accountNumberBalance; }
            set { accountNumberBalance = value; }
        }
        public string CreditAccount 
        {
            get { return creditAccount; }
            set { creditAccount = value; }
        }
        public string CreditAccountCurrency 
        {
            get { return creditAccountCurrency; }
            set { creditAccountCurrency = value; }
        }
        public double CreditAccountBalance
        {
            get { return creditAccountBalance; }
            set { creditAccountBalance = value; } 
        }

        public string Error 
        {
            get { return null; }
        }

        public string this[string columnName]
        {
            get 
            {
                if (columnName == "Name") 
                {
                    if (string.IsNullOrEmpty(Name))
                    {
                        return "A név kötelező mező.";
                    }

                    else if (Name.Length < 5)
                    { 
                        return "A név legalább 5 karakter hosszú kell legyen.";
                    }
                }

                else if (columnName == "Password")
                {
                }

                else if (columnName == "AccountNumber")
                {
                    Regex regex = new Regex(@"^\d{8}-\d{8}$");
                    if (string.IsNullOrEmpty(AccountNumber) || !regex.IsMatch(AccountNumber))
                    {
                        return "A számlaszám formátuma helytelen.";
                    }
                }

                else if (columnName == "AccountNumberCurrency")
                {
                }

                else if (columnName == "AccountNumberBalance")
                {
                }

                else if (columnName == "CreditAccount")
                {
                    if (string.IsNullOrEmpty(CreditAccount)) 
                    {
                        return "A hitelszámlaszám mező kitöltése kötelező.";
                    }
                    Regex regex = new Regex(@"^\d{8}-\d{8}$");
                    if (string.IsNullOrEmpty(CreditAccount) || !regex.IsMatch(CreditAccount))
                    {
                        return "Hitelszámlaszám formátuma nem megfelelő - jó példa: 12345678-12345678";
                    }
                }

                else if (columnName == "CreditAccountCurrency")
                {
                }

                else if (columnName == "CreditAccountBalance")
                {
                }
                return null;
            }
        }







    }
}
