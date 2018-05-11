using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;

namespace Parking
{
    public class Parking
    {
        private readonly List<Car> cars;
        private readonly List<Transaction> transactions;
        private double balance;
        private Timer withdrawTimer;
        private Timer transactionTimer;
        private const int transactionWritePeriod = 60;
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

        public Parking()
        {
            cars = new List<Car>(Settings.ParkingSpace);
            transactions = new List<Transaction>();
            withdrawTimer = new Timer(OnWithdrawTimer, null, 0, Settings.Timeout);
            transactionTimer = new Timer(OnTransactionTimer, null, 0, transactionWritePeriod);
        }

        public void AddCar(Car car)
        {
            if (cars.Count >= Settings.ParkingSpace)
            {
                throw new Exception("Can not add car. Limit reached.");
            }

            cars.Add(car);
        }

        public void RemoveCar(Car car)
        {
            if (!cars.Contains(car))
            {
                throw new Exception("Car not found");
            }

            cars.Remove(car);
        }

        public void RemoveCar(int carId)
        {
            var car = GetCarById(carId);
            RemoveCar(car);
        }

        public void InputMoney(int carId, double amount)
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

        public Parking Instance
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
