using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSimulation.Models
{
    enum BankTransactionType
    {
        //Átutalás, Átutalás díj elszámolás, Bejövő tranzakció
        //Készpénzfelvitel díj elszámolás, Készpénzfelvétel ügyfél számláról
        //Havidíj bevétel
        //Hitelkamat bevétel

        [Description("Átutalás")]
        Transfer,
        [Description("Átutalás díj elszámolás")]
        TransferCharge,
        [Description("Bejövő tranzakció")]
        IncomingTransaction,
        [Description("Készpénzfelvitel díj elszámolás")]
        CashWithdrawalCharge,
        [Description("Készpénzfelvétel ügyfél számlájáról")]
        CashWithdrawalFromCustomerAccount,
        [Description("Havidíj bevétel")]
        MonthlyIncome,
        [Description("Hitelkamat bevétel")]
        CreditInterestIncome
    }
}
