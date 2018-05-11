using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using System.Text;

namespace Parking
{
    public class Parking
    {
        public const int transactionWritePeriod = 60;
        private readonly List<Car> cars;
        private readonly List<Transaction> transactions;
        private double balance;
        private Timer withdrawTimer;
        private Timer transactionTimer;
        private const string transactionLogFileName = "Transactions.log";

        public List<Car> Cars
        {
            get
            {
                return cars;
            }
        }

        public List<Transaction> Transactions
        {
            get
            {
                return transactions;
            }
        }

        public double Balance
        {
            get
            {
                return balance;
            }
        }

        private Parking()
        {
            cars = new List<Car>(Settings.ParkingSpace);
            transactions = new List<Transaction>();
            withdrawTimer = new Timer(OnWithdrawTimer, null, 0, Settings.Timeout * 1000);
            transactionTimer = new Timer(OnTransactionTimer, null, 0, transactionWritePeriod * 1000);
        }

        public void AddCar(Car car)
        {
            if (cars.Count >= Settings.ParkingSpace)
            {
                throw new Exception("Can not add car. Limit reached.");
            }

            cars.Add(car);
        }

        public bool RemoveCar(Car car)
        {
            if (!cars.Contains(car))
            {
                throw new Exception("Car not found");
            }

            if (car.Balance < 0)
            {
                return false;
            }

            cars.Remove(car);
            return true;
        }

        public bool RemoveCar(int carId)
        {
            var car = GetCarById(carId);
            return RemoveCar(car);
        }

        public void PayForCar(int carId, double amount)
        {
            var car = GetCarById(carId);
            if (car == null)
            {
                throw new Exception("Car not found");
            }

            car.Put(amount);
        }

        public Car GetCarById(int carId)
        {
            return cars.SingleOrDefault(x => x.Id == carId);
        }

        public IEnumerable<Transaction> GetTransactionsForLastPeriod()
        {
            return transactions.Where(transaction => transaction.TransactionDateTime > DateTime.Now.AddSeconds(transactionWritePeriod * -1));
        }

        public int FreeParkingLotsCount()
        {
            return Settings.ParkingSpace - cars.Count;
        }

        public string GetFormattedTransactionLog()
        {
            StringBuilder logStringBuilder = new StringBuilder();
            using (StreamReader reader = new StreamReader(transactionLogFileName))
            {
                string line;
                while((line = reader.ReadLine()) != null)
                {
                    var lineSplit = line.Split('-');
                    logStringBuilder.AppendLine($"Earnings amount - {lineSplit[0]} on {lineSplit[1]}");
                }
            }

            return logStringBuilder.ToString();
        }

        private void OnWithdrawTimer(object state)
        {
            foreach (var car in cars)
            {
                double amount = Settings.Prices[car.CarType];
                if (car.Balance < 0)
                {
                    amount *= Settings.Fine;
                }

                car.Withdraw(amount);
                balance += amount;
                Transaction transaction = new Transaction(car.Id, amount);
                transactions.Add(transaction);
            }
        }

        private void OnTransactionTimer(object state)
        {
            var lastMinuteTransactions = GetTransactionsForLastPeriod();
            LogTransactions(lastMinuteTransactions);
        }

        private void LogTransactions(IEnumerable<Transaction> lastMinuteTransactions)
        {
            var totalAmount = lastMinuteTransactions.Sum(x => x.Amount);
            using (StreamWriter writer = new StreamWriter(transactionLogFileName, false))
            {
                string lineToWrite = string.Format("{0}-{1}", totalAmount, DateTime.Now);
                writer.WriteLine(lineToWrite);
            }
        }

        private static object lockObj = new object();
        private static Parking instance;

        public static Parking Instance
        {
            get
            {
                if (instance == null)
                {
                    lock(lockObj)
                    {
                        if (instance == null)
                        {
                            instance = new Parking();
                        }
                    }
                }

                return instance;
            }
        }
    }
}
