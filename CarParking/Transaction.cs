using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parking
{
    public class Transaction
    {
        private int carId;
        private DateTime transactionDateTime;
        private double amount;

        public DateTime TransactionDateTime
        {
            get
            {
                return transactionDateTime;
            }
        }

        public int CarId
        {
            get
            {
                return carId;
            }
        }

        public double Amount
        {
            get
            {
                return amount;
            }
        }

        public Transaction(int carId, int amount)
        {
            this.transactionDateTime = DateTime.Now;
            this.carId = carId;
            this.amount = amount;
        }
    }
}
