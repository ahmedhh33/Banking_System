using System.Collections.Generic;
using System;
using System.IO;
using System.Text.Json;
using System.Linq;

namespace BankSystems
{
    internal class Program
    {
        private static BankData bankData;
        private static DataFieling dataFieling = new DataFieling();
        private static ManagingUser managingUser;
        private static ManagingBankAccount managingBankAccount;
        private static int currentUserId = -1;
        static void Main(string[] args)
        {
            LoadBankData();
            managingUser = new ManagingUser(bankData);
            managingBankAccount = new ManagingBankAccount(bankData);

            while (true)
            {
                Console.WriteLine("Bank System Menu:");
                Console.WriteLine("1. Register");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. Exit");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        RegisterUser();
                        break;
                    case "2":
                        LoginUser();
                        break;
                    case "3":
                        SaveBankData();
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        private static void LoadBankData()
        {
            bankData = dataFieling.LoadData();
        }

        private static void SaveBankData()
        {
            dataFieling.SaveData(bankData);
        }

        private static void RegisterUser()
        {
            try
            {
                Console.WriteLine("Enter your name:");
                string name = Console.ReadLine();

                Console.WriteLine("Enter your email:");
                string email = Console.ReadLine();

                Console.WriteLine("Enter your password:");
                string password = Console.ReadLine();

                if (managingUser.RegesterNewUser(name, email, password))
                {
                    Console.WriteLine("Registration successful. You can now login.");
                }
                else
                {
                    Console.WriteLine("User with this email already exists. Please use a different email.");
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid input. Please enter a valid number.");
            }
            
        }

        private static void LoginUser()
        {
            Console.WriteLine("Enter your email:");
            string email = Console.ReadLine();

            Console.WriteLine("Enter your password:");
            string password = Console.ReadLine();

            var user = managingUser.Authentication(email, password);

            if (user != null)
            {
                currentUserId = bankData.Users.IndexOf(user);
                ShowLoggedInMenu();
            }
            else
            {
                Console.WriteLine("Invalid email or password. Please try again.");
            }
        }

        private static void ShowLoggedInMenu()
        {
            while (true)
            {
                Console.WriteLine("Logged-in Menu:");
                Console.WriteLine("1. Create Bank Account");
                Console.WriteLine("2. Deposit");
                Console.WriteLine("3. Withdraw");
                Console.WriteLine("4. Logout");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        CreateBankAccount();
                        break;
                    case "2":
                        Deposit();
                        break;
                    case "3":
                        Withdraw();
                        break;
                    case "4":
                        currentUserId = -1;
                        SaveBankData();
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        private static void CreateBankAccount()
        {
            Console.WriteLine("Enter account holder name:");
            string accountHolderName = Console.ReadLine();

            var newAccount = managingBankAccount.createBankAccount(accountHolderName);

            Console.WriteLine($"Account created successfully. Your account number is {newAccount.accountNumber}.");
        }

        private static void Deposit()
        {
            try
            {
                Console.WriteLine("Enter account number:");
                int accountNumber = int.Parse(Console.ReadLine());

                Console.WriteLine("Enter deposit amount:");
                decimal amount = decimal.Parse(Console.ReadLine());

                if (managingBankAccount.Deposit(accountNumber, amount))
                {
                    Console.WriteLine($"Deposit of {amount:OMR} successful.");
                }
                else
                {
                    Console.WriteLine("Invalid account number or deposit amount.");
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid input. Please enter a valid number.");
            }
            catch (OverflowException)
            {
                Console.WriteLine("Input is too large. Please enter a smaller number.");
            }
        }

        private static void Withdraw()
        {
            try
            {
                Console.WriteLine("Enter account number:");
                int accountNumber = int.Parse(Console.ReadLine());

                Console.WriteLine("Enter withdrawal amount:");
                decimal amount = decimal.Parse(Console.ReadLine());

                if (managingBankAccount.Withdraw(accountNumber, amount))
                {
                    Console.WriteLine($"Withdrawal of {amount: OMR} successful.");
                }
                else
                {
                    Console.WriteLine("Invalid account number, insufficient funds, or invalid withdrawal amount.");
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid input. Please enter a valid number.");
            }
            catch (OverflowException)
            {
                Console.WriteLine("Input is too large. Please enter a smaller number.");
            }
        }
    }
    }
    public class DataFieling
    {
        private readonly string dataFilePath = "bankdata.json";

        public BankData LoadData()
        {
            if (File.Exists(dataFilePath))
            {
                string jsonData = File.ReadAllText(dataFilePath);
                return JsonSerializer.Deserialize<BankData>(jsonData);
            }
            else
            {
                return new BankData(); // Create a new instance if the file doesn't exist
            }
        }

        public void SaveData(BankData data)
        {
            string jsonData = JsonSerializer.Serialize(data);
            File.WriteAllText(dataFilePath, jsonData);
        }
    }



    public class User
    {
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }

        public User(string name, string email, string password)
        {
            this.name = name;
            this.email = email;
            this.password = password;
        }
    }

    public class BankAccount
    {
        public int accountNumber { get; set; }
        public string accountholdername { get; set; }
        public decimal balance { get; set; }

        public BankAccount(int accountNumber, string accountholdername, decimal balance)
        {
            this.accountNumber = accountNumber;
            this.accountholdername = accountholdername;
            this.balance = balance;
        }
    }

    public class Transaction
    {
        public int accountNumber { get; set; }
        public TransactionType Type { get; set; }
        public decimal amount { get; set; }
        public DateTime TimeInfo { get; set; }

        public Transaction(int accountNumber, TransactionType Type, decimal amount, DateTime TimeInfo)
        {
            this.accountNumber = accountNumber;
            this.Type = Type;
            this.amount = amount;
            this.TimeInfo = TimeInfo;
        }
    }

    public enum TransactionType
    {
        Deposit,
        Withsrawal,
        Transfer
    }

    public class BankData
    {
        public List<User> Users { get; set; }
 
        public List<BankAccount> Accounts { get; set; }
        public List<Transaction> Transactions { get; set; }
    public BankData()
    {
        // Initialize Users as an empty list to avoid null reference exceptions
        Users = new List<User>();
        Accounts = new List<BankAccount>();
        Transactions = new List<Transaction>();
    }
}

    public class ManagingUser
    {
        private BankData bankData;

        public ManagingUser(BankData bankData)
        {
            this.bankData = bankData;
        }

        public bool RegesterNewUser(string name, string email, string password)
        {
        if (bankData.Users.Any(u => u.email == email))
            return false; //check if there already email same as this
        else
        {
            User newUser = new User(name, email, password);
            bankData.Users.Add(newUser);
            return true;
        }

        }

        public User Authentication(string email, string password)
        {
            return bankData.Users.FirstOrDefault(u => u.email == email && u.password == password);
            //authenticate the email and password if there are mashed
        }
    }

    public class ManagingBankAccount
    {
        private BankData bankData;

        public ManagingBankAccount(BankData bankData)
        {
            this.bankData = bankData;
        }

        public BankAccount createBankAccount(string accountholdername)
        {
            BankAccount newAccount = new BankAccount(GenerateAccountNumber(), accountholdername, 0);
            bankData.Accounts.Add(newAccount);
            return newAccount;
        }

        public bool Deposit(int accountNumber, decimal amount)
        {
            var account = bankData.Accounts.FirstOrDefault(a => a.accountNumber == accountNumber);
            if (account == null || amount <= 0)
            { return false; }

            account.balance += amount;

            Transaction transaction = new Transaction(accountNumber, TransactionType.Deposit, amount, DateTime.Now);
            bankData.Transactions.Add(transaction);
            return true;
        }
        public bool Withdraw(int accountNumber, decimal amount)
        {
            var account = bankData.Accounts.FirstOrDefault(a => a.accountNumber == accountNumber);
            if (account == null || amount <= 0 || amount > account.balance)
            { return false; }

            account.balance -= amount;

            Transaction transaction = new Transaction(accountNumber, TransactionType.Withsrawal, amount, DateTime.Now);
            bankData.Transactions.Add(transaction);
            return true;

        }

        private int GenerateAccountNumber()
        {
        Random random = new Random();
            int accountNumber;
        do
        {
            accountNumber = random.Next(1000, 9999);
        }
        while (bankData.Accounts.Any(a => a.accountNumber == accountNumber));
            {
                return accountNumber;
            }
        }
    }
