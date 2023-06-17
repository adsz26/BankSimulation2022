using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSimulation.Models
{
    enum CustomerTransactionType
    {
        //Átutalás, Átutalás díja, Bejövő tranzakció
        //Készpénzfelvétel, Készpénzfelvétel díja
        //Hitelfelvetel, Hitelfelvétel elfogadva, Hitelfelvétel elutasítva
        //Havidíj terhelés
        //Hitelkamat terhelés

        [Description("Átutalás")]
        Transfer,
        [Description("Átutalás díja")]
        TransferFee,
        [Description("Bejövő tranzakció")]
        IncomingTransaction,
        [Description("Készpénzfelvétel")]
        CashWithdrawal,
        [Description("Készpénzfelvétel díja")]
        CashWithdrawalFee,
        [Description("Hitelfelvétel")]
        LoanApplication,
        [Description("Hitelfelvétel elfogadva")]
        LoanApplicationAccepted,
        [Description("Hitelfelvétel elutasítva")]
        LoanApplicationRejected,
        [Description("Havidíj terhelés")]
        MonthlyCharge,
        [Description("Hitelkamat terhelés")]
        CreditInterestCharge,
    }
}
