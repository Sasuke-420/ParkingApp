using System;
using System.Collections.Generic;
using System.Linq;

class Expense
{
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public User PaidBy { get; set; }
    public List<ExpenseShare> Shares { get; set; } = new List<ExpenseShare>();
}

class ExpenseShare
{
    public User User { get; set; }
    public decimal Amount { get; set; }
}

class User
{
    public string Name { get; set; }
    public decimal Balance { get; set; }
}

class Group
{
    public List<User> Users { get; set; } = new List<User>();
    public List<Expense> Expenses { get; set; } = new List<Expense>();

    public void AddExpense(Expense expense)
    {
        Expenses.Add(expense);
    }

    public void CalculateBalances()
    {
        foreach (var user in Users)
        {
            user.Balance = 0;
        }

        foreach (var expense in Expenses)
        {
            var totalShares = expense.Shares.Sum(share => share.Amount);
            foreach (var share in expense.Shares)
            {
                share.User.Balance -= share.Amount;
                expense.PaidBy.Balance += share.Amount;
            }
        }
    }

    public List<Transaction> MinimizeTransactions()
    {
        List<Transaction> transactions = new List<Transaction>();

        var debtors = Users.Where(user => user.Balance < 0).ToList();
        var creditors = Users.Where(user => user.Balance > 0).ToList();

        debtors.Sort((user1, user2) => user1.Balance.CompareTo(user2.Balance));
        creditors.Sort((user1, user2) => user2.Balance.CompareTo(user1.Balance));

        foreach (var debtor in debtors)
        {
            while (debtor.Balance < 0)
            {
                var creditor = creditors.First();
                decimal amount = Math.Min(Math.Abs(debtor.Balance), creditor.Balance);

                debtor.Balance += amount;
                creditor.Balance -= amount;

                transactions.Add(new Transaction { FromUser = creditor, ToUser = debtor, Amount = amount });

                if (creditor.Balance == 0)
                {
                    creditors.RemoveAt(0);
                }
            }
        }

        return transactions;
    }

    public void PrintBalances()
    {
        foreach (var user in Users)
        {
            Console.WriteLine($"{user.Name}: {user.Balance:C}");
        }
    }
}

class Transaction
{
    public User FromUser { get; set; }
    public User ToUser { get; set; }
    public decimal Amount { get; set; }
}

class Program
{
    static void Main(string[] args)
    {
        User user1 = new User { Name = "Alice" };
        User user2 = new User { Name = "Bob" };
        User user3 = new User { Name = "Charlie" };

        Group group = new Group { Users = { user1, user2, user3 } };

        Expense expense1 = new Expense
        {
            Description = "Dinner",
            Amount = 100,
            PaidBy = user1,
            Shares = new List<ExpenseShare>
            {
                new ExpenseShare { User = user1, Amount = 40 },
                new ExpenseShare { User = user2, Amount = 30 },
                new ExpenseShare { User = user3, Amount = 30 }
            }
        };

        Expense expense2 = new Expense
        {
            Description = "Movie",
            Amount = 60,
            PaidBy = user2,
            Shares = new List<ExpenseShare>
            {
                new ExpenseShare { User = user1, Amount = 20 },
                new ExpenseShare { User = user2, Amount = 20 },
                new ExpenseShare { User = user3, Amount = 20 }
            }
        };

        group.AddExpense(expense1);
        group.AddExpense(expense2);

        group.CalculateBalances();

        Console.WriteLine("Original balances:");
        group.PrintBalances();

        var transactions = group.MinimizeTransactions();

        Console.WriteLine("\nMinimized transactions:");
        foreach (var transaction in transactions)
        {
            Console.WriteLine($"{transaction.FromUser.Name} owes {transaction.ToUser.Name} {transaction.Amount:C}");
        }
    }
}
