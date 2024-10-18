using MoneyTrackingApplication;
using System;
using System.ComponentModel.Design;
using static MoneyTrackingApplication.Event;
using static System.Net.Mime.MediaTypeNames;

namespace MoneyTrackingApplication
{
    public class Program
    {
        static void Main(string[] args)
        {
            Session session = new Session();
            session.MenuCommand();

        }
    }

    public class Session
    {

        public void MenuCommand() {
            string[] menuOptions = new string[] { "Check Transactions", "Current Balance", "Add transaction to Account", "Edit or Remove transaction", "Save and Quit" };
            int menuSelect = 0;
            Event accountEvent = new Event();

            while (true)
            {
                Console.Clear();
                Console.CursorVisible = false;                
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.WriteLine("Hello and welcome to MoneyTracker! Please make a choice:");
                Console.ResetColor();

                for (int i = 0; i < menuOptions.Length; i++)
                {
                    if (i == menuSelect)
                    {
                        Console.BackgroundColor = ConsoleColor.Green;
                        Console.WriteLine(menuOptions[i] + " <--");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine(menuOptions[i]);
                    }
                }

                var keyPressed = Console.ReadKey();

                if (keyPressed.Key == ConsoleKey.DownArrow && menuSelect != menuOptions.Length - 1)
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
                            accountEvent.CheckTransactionEvents();
                            break;
                        case 1:
                            accountEvent.CheckBalance();
                            Console.ReadLine();
                            break;
                        case 2:
                            accountEvent.AddTransaction();
                            break;
                        case 3:
                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                            Console.WriteLine("Edit or Remove items");
                            Console.ResetColor();
                            accountEvent.removeEdit();
                            Console.ReadLine();
                            break;
                        case 4:
                            Console.WriteLine("Save and Quit. Goodbye!");
                            Environment.Exit(0);
                            break;
                    }
                }
            }
        }
    }

}