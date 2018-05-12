using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parking
{
    class Program
    {
        private static string stageHeader = "Please select action and press enter:";
        private static string freeSpaceFormat = "There are {0} free parking space(s) out of {1} left.";
        private static string firstStage = @"1: Parking management (Add/Remove cars, input money)
2: Show transaction history for last period ({0} seconds)
3: Show total parking earnings (profit)
4: Show transactions log
5: Clear console
0: To exit";
        private static string stageOne = @"1: Add a car
2: Remove a car
3: Input money
0: To go back";
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to my Parking Lot!");
            bool exit = false;
            while (!exit)
            {
                exit = ParkingLotWork();
            }
        }

        private static bool ParkingLotWork()
        {
            try
            {
                while(true)
                {
                    int result = GetFirstStageInput();
                    switch (result)
                    {
                        case 1:
                            HandleStageOne();
                            break;
                        case 2:
                            Console.WriteLine("Transactions for last {0} seconds", Parking.transactionWritePeriod);
                            var transactions = Parking.Instance.GetTransactionsForLastPeriod();
                            foreach (var transaction in transactions)
                            {
                                Console.WriteLine(transaction);
                            }
                            Console.WriteLine("Total earnings per last {0} seconds: {1}", Parking.transactionWritePeriod, transactions.Sum(transaction => transaction.Amount));
                            break;
                        case 3:
                            Console.WriteLine("Total parking earnings: {0}", Parking.Instance.Balance);
                            break;
                        case 4:
                            Console.WriteLine("Transaction log:");
                            Console.WriteLine(Parking.Instance.GetFormattedTransactionLog());
                            break;
                        case 5:
                            Console.Clear();
                            break;
                        case 0:
                            return true;
                        default:
                            {
                                Console.WriteLine("Invalid input! Please try again.");
                                break;
                            }
                    }
                }
            }
            catch (Exception e)
            {
                Console.Clear();
                Console.WriteLine("Something went wrong, lets try again. \r\nMessage: {0}", e.Message);
                return false;
            }
        }

        private static void HandleStageOne()
        {
            
            while (true)
            {
                Console.WriteLine(stageHeader);
                ShowParkingStatus();

                Console.WriteLine(stageOne);
                switch (GetUserInput())
                {
                    case 1:
                        if (Parking.Instance.FreeParkingLotsCount() > 0)
                        {
                            AddCarFlow();
                        }
                        else
                        {
                            Console.WriteLine("Cannot add car. Parking is full!");
                            continue;
                        }

                        break;
                    case 2:
                        if (Parking.Instance.FreeParkingLotsCount() < Settings.ParkingSpace)
                        {
                            RemoveCarFlow();
                        }
                        else
                        {
                            Console.WriteLine("Cannot remove a car. The parking lot is empty!");
                        }
                        
                        break;
                    case 3:
                        if (Parking.Instance.FreeParkingLotsCount() < Settings.ParkingSpace)
                        {
                            InputMoneyFlow();
                        }
                        else
                        {
                            Console.WriteLine("Cannot insert money. The parking lot is empty!");
                        }
                       
                        break;
                    case 0:
                        return;
                    default:
                        Console.WriteLine("Invalid input! Please try again.");
                        continue;
                }
                break;
            }
        }

        private static void ShowParkingStatus()
        {
            Console.WriteLine(freeSpaceFormat, Parking.Instance.FreeParkingLotsCount(), Settings.ParkingSpace);
            if (Parking.Instance.Cars.Any())
            {
                Console.WriteLine("Cars in parking lot:");
                foreach (var car in Parking.Instance.Cars)
                {
                    Console.WriteLine(car);
                }
            }
        }

        private static void AddCarFlow()
        {
            Console.WriteLine(freeSpaceFormat, Parking.Instance.FreeParkingLotsCount(), Settings.ParkingSpace);
            while (true)
            {
                try
                {
                    Console.WriteLine("Please enter car type and press enter: \r\n1: Passenger\r\n2: Truck\r\n3: Bus \r\n4: Motorcycle\r\nEnter 0 to cancel.");
                    int input = GetUserInput();
                    if (input == 0)
                        return;

                    CarType carType = (CarType)input;
                    if (!Enum.IsDefined(typeof(CarType), carType))
                    {
                        Console.WriteLine("Invalid car type, please try again.");
                        continue;
                    }

                    Console.WriteLine("Please enter initial car balance:");
                    int balance = GetUserInput();
                    Car carToAdd = new Car(balance, carType);
                    Parking.Instance.AddCar(carToAdd);
                    Console.WriteLine("Car added! Info:\r\n{0}", carToAdd);
                    break;
                }
                catch
                {
                    Console.WriteLine("Invalid input! Please try again.");
                }
            }
        }

        private static void RemoveCarFlow()
        {
            while (true)
            {
                try
                {
                    ShowParkingStatus();
                    Console.WriteLine("Please enter car Id and press enter or 0 to cancel.");
                    int carId = GetUserInput();
                    if (carId == 0)
                        return;

                    if (Parking.Instance.RemoveCar(carId))
                    {
                        Console.WriteLine("Car with Id {0} removed!", carId);
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Could not remove car with Id: {0}. \r\nPlease make sure that the car balance is positive", carId);
                    }
                }
                catch
                {
                    Console.WriteLine("Invalid input! Please try again.");
                }
            }
        }

        private static void InputMoneyFlow()
        {
            while (true)
            {
                try
                {
                    ShowParkingStatus();
                    Console.WriteLine("Please enter car Id and press enter or 0 to cancel.");
                    int carId = GetUserInput();
                    if (carId == 0)
                        return;

                    Console.WriteLine("Please enter the amount of money you want to add or 0 to cancel: ");
                    int amountToAdd = GetUserInput();
                    if (amountToAdd == 0)
                        return;

                    //everything should go through the cash register? :)
                    //or should we get car by Id and input money into car directly?
                    Parking.Instance.PayForCar(carId, amountToAdd);

                    Console.WriteLine("Car with Id {0} balance increased successfully! Current balance:", carId, Parking.Instance.GetCarById(carId).Balance);
                    break;
                }
                catch
                {
                    Console.WriteLine("Invalid input! Please try again.");
                }
            }
        }

        private static int GetFirstStageInput()
        {
            Console.WriteLine(freeSpaceFormat, Parking.Instance.FreeParkingLotsCount(), Settings.ParkingSpace);
            Console.WriteLine(stageHeader);
            Console.WriteLine(firstStage, Parking.transactionWritePeriod);
            return GetUserInput();
        }

        private static int GetUserInput()
        {
            string input = Console.ReadLine();
            int selectedOption = Convert.ToInt32(input);
            return selectedOption;
        }
    }
}
