using System;

namespace MoneyTrackingApplication
{
    public class Transaction
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public int Amount { get; set; }
        public string Month { get; set; }

        
        public Transaction(string name, string category, int amount, string month)
        {
            Name = name;
            Category = category;
            Amount = amount;
            Month = month;
        }

        //public Transaction()
        //{
        //}

        public static int display(int Amount, string Category)
        {
            var negative = "-" + Amount;
            var positive = "+" + Amount;

            return Category == "Expense" ? Int32.Parse(negative) : Int32.Parse(positive);
            
        }

        public static string ColorizeExpenseTransactions(string value, string Category)
        {
            if (Category == "Expense")
            {
                var input = value + (Console.ForegroundColor = ConsoleColor.Red);
                input = input.Replace("Red", string.Empty);
                return  input;                
            }

            else
            {
                return value;
            }

        }

        public static string Truncate(string value, int maxLength)
        {
            return value.Length <= maxLength ? value : value.Substring(0, maxLength - 3) + "...";
        }


    }
}

