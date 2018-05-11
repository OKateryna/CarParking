using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parking
{
    public class Parking
    {
        private readonly List<Car> cars;
        private readonly List<Transaction> transactions;
        private double balance;

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
            get { return balance; }
            set { balance = value; }
        }

        public Parking()
        {
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
