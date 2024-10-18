using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.SqlTypes;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Principal;
using System.Transactions;

namespace MoneyTrackingApplication
{
    public class Event
    {
        public string Filepath { get; set; } = "logfile.txt";
        private List<Transaction> account = new List<Transaction>();
        private List<Transaction> editTransaction = new List<Transaction>();        

        public void AddTransaction()
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.WriteLine("Here you can Add transaction events to your balance account - feel free to dial us on phone number 555 555 5555 for any queries");
                Console.ResetColor();
                Console.WriteLine("---------------------------");
                Console.WriteLine("Would you like to add Income or Expense to your account? (Type 'Income' or 'Expense' with capital letters) - Enter 'Exit' to go back");
                string category = Console.ReadLine();
                if (category.Equals("Exit", StringComparison.OrdinalIgnoreCase)) { break; }
                if (!category.Equals("Income") && !category.Equals("Expense")) { AddTransaction(); Console.WriteLine("Check your spelling - choose Income or Expense with capital letters!"); }

                Console.WriteLine("What is the name of the transaction? (Enter 'Exit' to go back)");

                string nameTransaction = Console.ReadLine();
                if (nameTransaction.Equals("Exit", StringComparison.OrdinalIgnoreCase)) { break; }
                if (nameTransaction == "") 
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("You have to type in a name of this transaction!");
                    Console.ResetColor();
                    AddTransaction(); 
                }

                Console.WriteLine("Which amount? (Enter 'Exit' to go back)");
                string amountInput = Console.ReadLine();
                int num;
                if (!int.TryParse(amountInput, out num))
                {
                    Console.ForegroundColor= ConsoleColor.Red;
                    Console.WriteLine("Not an integer");
                    Console.ResetColor();
                    AddTransaction();
                }
                if (amountInput.Equals("Exit", StringComparison.OrdinalIgnoreCase)) { break; }

                Console.WriteLine("For which month? - Write months capitalized (March, April, May) - (Enter 'Exit' to go back)");
                string monthOfTransaction = Console.ReadLine();
                if (monthOfTransaction.Equals("Exit", StringComparison.OrdinalIgnoreCase)) { break; }

                if (monthOfTransaction != "January" && monthOfTransaction != "February" && monthOfTransaction != "March" && monthOfTransaction != "April" && monthOfTransaction != "May" && monthOfTransaction != "June" && monthOfTransaction != "July" && monthOfTransaction != "August" && monthOfTransaction != "September" && monthOfTransaction != "October" && monthOfTransaction != "November" && monthOfTransaction != "December")

                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("You must enter a valid Month when the transaction is booked");
                    Console.ResetColor();
                    Console.ReadLine();
                    return;
                    
                }                              

                    // Validate amount input
                    if (!int.TryParse(amountInput, out int amountInt))
                    {
                        Console.WriteLine("Invalid amount. Please enter a valid number.");
                        continue;
                    }

                    bool found = account.Any(transaction =>
                        transaction.Name.Equals(nameTransaction, StringComparison.OrdinalIgnoreCase) &&
                        transaction.Category == category &&
                        transaction.Amount == amountInt &&
                        transaction.Month == monthOfTransaction);

                    

                if (File.Exists(Filepath))
                    {
                        using (StreamReader reader = new StreamReader(Filepath))
                        {
                            string data;
                            // Read line by line
                            while ((data = reader.ReadLine()) != null)
                            {
                                string[] eachLine = data.Split(',');

                            bool foundInLogFile = account.Any(transaction =>
                            transaction.Name.Equals(eachLine[0], StringComparison.OrdinalIgnoreCase) &&
                            transaction.Category == eachLine[1] &&
                            transaction.Amount == int.Parse(eachLine[2]) &&
                            transaction.Month == eachLine[3]);

                            if (eachLine.Length == 4 && !foundInLogFile && !found)
                                {
                                    Transaction transaction = new Transaction(eachLine[0], eachLine[1], int.Parse(eachLine[2]), eachLine[3]);
                                    account.Add(transaction);
                                }
                            }
                        }

                    }

                

                if (!found)
                {
                    Console.WriteLine($"You want to add {amountInt} to your {category} transactions. Are you sure? (Yes/No)");
                    string userConfirm = Console.ReadLine();

                    if (userConfirm.Equals("Yes", StringComparison.OrdinalIgnoreCase))
                    {
                        Transaction transaction = new Transaction(nameTransaction, category, amountInt, monthOfTransaction);
                        account.Add(transaction);

                        if (!File.Exists(Filepath))
                        {
                            StreamWriter writer = new StreamWriter(Filepath, true);
                            writer.WriteLine($"{transaction.Name},{transaction.Category},{transaction.Amount},{transaction.Month}");
                            writer.Close();
                        }

                        else
                        {
                            using (StreamWriter w = new StreamWriter(Filepath, true))
                            {
                                w.WriteLine($"{transaction.Name},{transaction.Category},{transaction.Amount},{transaction.Month}");
                            }
                        }


                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Transaction successfully added to your account!");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine("Transaction was not added.");
                        break;
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("This transaction already exists in the list!");
                    Console.ResetColor();
                }
            }
        }

        public void CheckTransactionEvents()
        {
            
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("Checking your transactions...");
            Console.ResetColor();
                        

            if (!File.Exists(Filepath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"File '{Filepath}' not found. No transactions to show.");
                Console.ResetColor();
                Console.WriteLine("Press Enter to go back to the main menu...");
                Console.ReadLine();
                return;
            }

            
            Console.WriteLine("Your current transactions are:");            
            Console.WriteLine(">>>>>>>>>>>>>>><<<<<<<<<<<<<<<");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Name".PadRight(15) + " " + "Category".PadRight(15) + " " + "Amount".PadRight(15) + " " + "Month".PadRight(15));
            Console.ResetColor();

            using (StreamReader reader = new StreamReader(Filepath))
            {
                string data;
                while ((data = reader.ReadLine()) != null)
                {
                    string[] entries = data.Split(',');

                    bool identicalProduct = account.Any(transaction =>
                            transaction.Name.Equals(entries[0], StringComparison.OrdinalIgnoreCase) &&
                            transaction.Category == entries[1] &&
                            transaction.Amount == int.Parse(entries[2]) &&
                            transaction.Month == entries[3]);

                    if (entries.Length == 4 && !identicalProduct)
                    {
                        if (int.TryParse(entries[2], out int amount) && !identicalProduct)
                        {
                            Transaction transaction = new Transaction(entries[0], entries[1], amount, entries[3]);
                            account.Add(transaction);                            
                            //Console.WriteLine($"{Transaction.Truncate(transaction.Name.PadRight(15), 15)} {Transaction.Truncate(transaction.Category.PadRight(15), 15)} {Transaction.Truncate(transaction.Amount.ToString().PadRight(15), 15)} {Transaction.Truncate(transaction.Month.PadRight(15), 15)}");
                        }

                       
                    }
                    //else
                    //{
                    //    Console.WriteLine("1");
                    //}
                }
            }

            List<Transaction> listSortedByAmount = account.OrderBy(item => item.Amount).ToList();

            foreach (Transaction transaction in listSortedByAmount) {

            Console.WriteLine($"{Transaction.Truncate(transaction.Name.PadRight(15), 15)} {Transaction.Truncate(transaction.Category.PadRight(15), 15)} {Transaction.ColorizeExpenseTransactions(Transaction.Truncate(transaction.Amount.ToString().PadRight(15), 15), transaction.Category)} {Transaction.Truncate(transaction.Month.PadRight(15), 15)}");
                Console.ResetColor();

            }

            Console.WriteLine(">>>>>>>>>>>>>>><<<<<<<<<<<<<<<");
            Console.WriteLine("Press Enter to go back to the main menu...");
            Console.ReadLine();
        }

        public void CheckBalance()
        {
            List<Transaction> sortedIncomeList = account.OrderBy(item => item.Amount).ToList();                              
           

            string[] menu = new string[] { "Current Balance", "Main Menu" };
            int menuSelect = 0;

            if (!File.Exists(Filepath))
            {
                
                
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"File '{Filepath}' not found. No transactions to show.");
                Console.ResetColor();
                Console.WriteLine("Press Enter to go back to the main menu...");
                Console.ReadLine();                
            }


            else
            {
                using (StreamReader reader = new StreamReader(Filepath))
                {
                    string data;
                    while ((data = reader.ReadLine()) != null)
                    {
                        string[] entries = data.Split(',');

                        bool found = sortedIncomeList.Any(transaction =>
                                transaction.Name.Equals(entries[0], StringComparison.OrdinalIgnoreCase) &&
                                transaction.Category == entries[1] &&
                                transaction.Amount == int.Parse(entries[2]) &&
                                transaction.Month == entries[3]);

                        if (entries.Length == 4 && !found)
                        {
                            if (int.TryParse(entries[2], out int amount) && !found)
                            {
                                Transaction transaction = new Transaction(entries[0], entries[1], amount, entries[3]);
                                sortedIncomeList.Add(transaction);
                            }
                        }
                        else
                        {
                            Console.WriteLine("");
                        }
                    }
                }

            }

            while (true)
            {
                Console.Clear();
                Console.CursorVisible = false;
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.WriteLine("Here you can find your Account Balance");
                Console.ResetColor();                

                for (int i = 0; i < menu.Length; i++)
                {
                    if (i == menuSelect)
                    {
                        Console.BackgroundColor = ConsoleColor.Green;
                        Console.WriteLine(menu[i] + " <--");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine(menu[i]);
                    }
                }

                var keyPressed = Console.ReadKey();

                if (keyPressed.Key == ConsoleKey.DownArrow && menuSelect != menu.Length - 1)
                {
                    menuSelect++;
                }
                else if (keyPressed.Key == ConsoleKey.UpArrow && menuSelect >= 1)
                {
                    menuSelect--;
                }
                else if (keyPressed.Key == ConsoleKey.Enter)
                {
                    Console.Clear();
                    switch (menuSelect)
                    {                        

                        case 0:
                            var filterEarnings = sortedIncomeList.Where(x => x.Category == "Income");
                            var filterCosts = sortedIncomeList.Where(x => x.Category == "Expense");
                            var totalIncome = sortedIncomeList.Where(x => x.Category == "Income").Select(x => x.Amount).Sum();
                            var totalExpense = sortedIncomeList.Where(x => x.Category == "Expense").Select(x => x.Amount).Sum();
                            var currBalance = totalIncome - totalExpense;

                            if (currBalance <= 0)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"Your current account balance is: {currBalance}");
                                Console.ResetColor();
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"Your current account balance is: {currBalance}");
                                Console.ResetColor();
                            }
                            Console.WriteLine(">>>>>>>>>>>>>>>>>>>>><<<<<<<<<<<<<<");                            
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine("INCOMES".PadRight(15));
                            Console.WriteLine("Name".PadRight(15) + " " + "Amount".PadRight(15) + " " + "Month".PadRight(15));
                            Console.ResetColor();
                            foreach (var item in filterEarnings)
                            {
                                Console.WriteLine($"{Transaction.Truncate(item.Name.PadRight(15), 15)} {Transaction.Truncate(item.Amount.ToString().PadRight(15), 15)} {Transaction.Truncate(item.Month.PadRight(15), 15)}");                                
                            }
                            Console.WriteLine(">>>>>>>>>>>>>>>>>>>>><<<<<<<<<<<<<<");                           
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine("EXPENSES".PadRight(15));
                            Console.WriteLine("Name".PadRight(15) + " " + "Amount".PadRight(15) + " " + "Month".PadRight(15));
                            Console.ResetColor();
                            foreach (var item in filterCosts)
                            {
                                Console.WriteLine($"{Transaction.Truncate(item.Name.PadRight(15), 15)} {Transaction.Truncate(item.Amount.ToString().PadRight(15), 15)} {Transaction.Truncate(item.Month.PadRight(15), 15)}");
                            }
                            //Console.WriteLine(" ");
                            //Console.WriteLine(" ");
                            //Console.WriteLine(" ");
                            Console.WriteLine("-----------------------------------");
                            Console.WriteLine("-----------------------------------"); 
                            Console.WriteLine("-----------------------------------");
                            Console.WriteLine("Press Enter to go back to Main Menu");
                            Console.ReadLine();
                            break;

                        case 1:
                            Session session = new Session();
                            session.MenuCommand();                                                     
                            break;                                                                 
                    }
                }
            }

        }

        public void removeEdit()
            {
                
                if (!File.Exists(Filepath))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"File '{Filepath}' not found. No transactions to show.");
                    Console.ResetColor();
                    Console.WriteLine("Press Enter to go back to the main menu...");
                    Console.ReadLine();
                    return;
                }

                using (StreamReader r = new StreamReader(Filepath))
                {
                    string data;
                    while ((data = r.ReadLine()) != null)
                    {
                        string[] line = data.Split(',');

                        if (line.Length == 4)
                        {

                            Transaction editItems = new Transaction(line[0], line[1], int.Parse(line[2]), line[3]);

                            bool found = editTransaction.Any(transaction =>
                            transaction.Name.Equals(line[0], StringComparison.OrdinalIgnoreCase) &&
                            transaction.Category == line[1] &&
                            transaction.Amount == int.Parse(line[2]) &&
                            transaction.Month == line[3]);

                            if (!found)
                            {
                                editTransaction.Add(editItems);
                            }


                        }
                    }
                }

                List<Transaction> sortedList = editTransaction.OrderBy(transactEve => transactEve.Amount).ToList();

                Console.WriteLine("Here is your balance");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Name".PadRight(15) + " " + "Category".PadRight(15) + " " + "Amount".PadRight(15) + " " + "Month".PadRight(15));
                Console.ResetColor();

                foreach (Transaction transaction in sortedList)
                {

                    Console.WriteLine($"{Transaction.Truncate(transaction.Name.PadRight(15), 15)} {Transaction.Truncate(transaction.Category.PadRight(15), 15)} {Transaction.ColorizeExpenseTransactions(Transaction.Truncate(transaction.Amount.ToString().PadRight(15), 15), transaction.Category)} {Transaction.Truncate(transaction.Month.PadRight(15), 15)}");
                    Console.ResetColor();
                }

                Console.WriteLine("-----------------------------------------------");
                Console.WriteLine("Do you want to remove a transaction event from your balance or edit? - (Remove/Edit) - Enter \"Exit\" to get back to Main Menu");
                string userChoice = Console.ReadLine();
                if (userChoice.Equals("Exit", StringComparison.OrdinalIgnoreCase))
                {
                return;
                }
                if (userChoice != null && userChoice.Equals("Remove", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Which item do you want to remove - to remove Fuel write \"Fuel\" ");
                    string deleteItem = Console.ReadLine();
                    if (deleteItem == "")
                    {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("You didn't make a valid choice!");
                    Console.ResetColor();
                    removeEdit();
                    }

                Console.WriteLine($"Are you sure you want to remove {deleteItem}? - answer with Yes/No");
                    var firstEven = sortedList.FirstOrDefault(n => n.Name == deleteItem);
                    string answer = Console.ReadLine();
                    if (answer == "Yes" && sortedList.Contains(firstEven))
                    {

                        sortedList.RemoveAll(x => x.Name == deleteItem);

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"We have removed transaction event {deleteItem}");
                        Console.ResetColor();
                        Console.WriteLine("Your balance is listed below:");


                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("Name".PadRight(15) + " " + "Category".PadRight(15) + " " + "Amount".PadRight(15) + " " + "Month".PadRight(15));
                        Console.ResetColor();

                        foreach (Transaction transaction in sortedList)
                        {
                            Console.WriteLine($"{Transaction.Truncate(transaction.Name.PadRight(15), 15)} {Transaction.Truncate(transaction.Category.PadRight(15), 15)} {Transaction.Truncate(transaction.Amount.ToString().PadRight(15), 15)} {Transaction.Truncate(transaction.Month.PadRight(15), 15)}");

                        }

                        if (!File.Exists(Filepath))
                            File.Create(Filepath);

                        TextWriter tw = new StreamWriter(Filepath, false);
                        tw.Write(string.Empty);

                        foreach (Transaction transaction in sortedList)
                        {
                            tw.WriteLine($"{transaction.Name},{transaction.Category},{transaction.Amount},{transaction.Month}");
                        }
                        tw.Close();

                        Console.WriteLine("Press Enter to go back to the main menu...");
                        Console.ReadLine();
                        return;
                    }

                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("It was for some reason not possible to process your request - Please check your spelling");
                        Console.ResetColor();
                        Console.WriteLine("Press Enter to try again");
                        Console.ReadLine();
                        removeEdit();
                    }

                }

                if (userChoice != null && userChoice.Equals("Edit", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Which transaction event would you like to edit? - For editing Rent write \"Rent\" ");
                    string theItemToEdit = Console.ReadLine();
                    if (theItemToEdit == "") 
                    {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("You didn't make a valid choice!");
                    Console.ResetColor();
                    removeEdit();
                    }
                    var index = sortedList.FindIndex(transactionEvent => transactionEvent.Name == theItemToEdit);
                    Console.WriteLine($"Are you sure you want to edit {theItemToEdit}? - (Yes/No)");
                    string userChoose = Console.ReadLine();
                    if (userChoose == "Yes")
                    {

                        Console.WriteLine("Would you like to change the name, category, amount or month of this transaction event?");
                        string decision = Console.ReadLine();

                        if (decision == "name")
                        {
                            Console.WriteLine("Write the new name");
                            string newName = Console.ReadLine();
                            sortedList[index].Name = newName;

                            TextWriter tw = new StreamWriter(Filepath, false);
                            tw.Write(string.Empty);

                            foreach (Transaction transaction in sortedList)
                            {
                                tw.WriteLine($"{transaction.Name},{transaction.Category},{transaction.Amount},{transaction.Month}");
                            }
                            tw.Close();
                        }

                        if (decision == "category")
                        {
                            Console.WriteLine("Write the new category");
                            string newCategory = Console.ReadLine();
                            sortedList[index].Category = newCategory;

                            TextWriter tw = new StreamWriter(Filepath, false);
                            tw.Write(string.Empty);

                            foreach (Transaction transaction in sortedList)
                            {
                                tw.WriteLine($"{transaction.Name},{transaction.Category},{transaction.Amount},{transaction.Month}");
                            }
                            tw.Close();
                        }

                        if (decision == "amount")
                        {
                            Console.WriteLine("Write the new amount of this transaction event");
                            string newAmount = Console.ReadLine();
                            sortedList[index].Amount = int.Parse(newAmount);

                            TextWriter tw = new StreamWriter(Filepath, false);
                            tw.Write(string.Empty);

                            foreach (Transaction transaction in sortedList)
                            {
                                tw.WriteLine($"{transaction.Name},{transaction.Category},{transaction.Amount},{transaction.Month}");
                            }
                            tw.Close();
                        }

                        if (decision == "month")
                        {
                            Console.WriteLine("Write the new month");
                            string newMonth = Console.ReadLine();
                            sortedList[index].Month = newMonth;

                            TextWriter tw = new StreamWriter(Filepath, false);
                            tw.Write(string.Empty);

                            foreach (Transaction transaction in sortedList)
                            {
                                tw.WriteLine($"{transaction.Name},{transaction.Category},{transaction.Amount},{transaction.Month}");
                            }
                            tw.Close();
                        }
                        if (decision != "name" && decision != "category" && decision != "amount" && decision != "month")
                        {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("You didn't choose a valid parameter to edit!");
                        Console.ResetColor();
                        }

                    }

                    Console.BackgroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("The edited balance is displayed below:");
                    Console.ResetColor();

                    foreach (Transaction transaction in sortedList)
                    {

                        Console.WriteLine($"{Transaction.Truncate(transaction.Name, 15)} {Transaction.Truncate(transaction.Category, 15)} {Transaction.Truncate(transaction.Amount.ToString(), 15)} {Transaction.Truncate(transaction.Month, 15)}");
                    }

                    Console.WriteLine("Press Enter to go back to main menu");
                    Console.ReadLine();


                }
            }

        }

    }



