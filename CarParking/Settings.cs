using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parking
{
    static class Settings
    {
        public readonly static Dictionary<CarType, double> Prices = new Dictionary<CarType, double>
        {
            {CarType.Truck, 5 },
            {CarType.Passenger, 3 },
            {CarType.Bus, 2 },
            {CarType.Motorcycle, 1 }
        };
        public readonly static int ParkingSpace = 10;
        public readonly static double Fine = 2;
        public readonly static int Timeout = 3;
    }
}
