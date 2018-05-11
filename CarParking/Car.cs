using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parking
{
    public class Car
    {
        private int id;
        private int balance;
        private CarType carType;
        private static int count = 1;

        public int Id
        {
            get
            {
                return id;
            }
        }

        public int Balance
        {
            get
            {
                return balance;
            }
        }
        public CarType CarType
        {
            get
            {
                return carType;
            }
        }

        public Car(int balance, CarType carType)
        {
            this.id = count++;
            this.balance = balance;
            this.carType = carType;
        }

        public void Withdraw(int amount)
        {
            balance -= amount;
        }

        public void Put(int amount)
        {
            balance += amount;
        }

    }
}
