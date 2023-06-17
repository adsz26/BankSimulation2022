using BankSimulation.Models.EF;

namespace BankSimulation
{
    public static class LoggedInUser //: ObservableObject
    {
        public static Customer Customer { get; set; }
        public static Bank Bank { get; set; }

        public static void SetCustomer(Customer c)
        {
            Customer = c;
        }

        public static void SetBank(Bank b)
        {
            Bank = b;
        }
    }
}
