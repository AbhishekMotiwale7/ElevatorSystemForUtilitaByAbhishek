using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatoreSystem
{
    public class Program
    {

        static int totalNumberOfFloors;
        static IElevator iElevatorRef;
        public static void Main(string[] args)
        {
            Console.Write(new string(' ', (Console.WindowWidth) / 2));
            Console.WriteLine("//--------------------------------------------------------// \n ");
            Console.Write(new string(' ', (Console.WindowWidth) / 2));
            Console.WriteLine("Welcome to Utilita's elevator system.");
            Console.Write(new string(' ', (Console.WindowWidth) / 2));
            Console.WriteLine("Please considered below points. \n");
            Console.Write(new string(' ', (Console.WindowWidth) / 2));
            Console.WriteLine("1) Average weight of person is 50kg.");
            Console.Write(new string(' ', (Console.WindowWidth) / 2));
            Console.WriteLine("2) Elevator maximum capacity is 200KG.");
            Console.Write(new string(' ', (Console.WindowWidth) / 2));
            Console.WriteLine("3) At a time 5 passengers are allowed in an Elevator.");
            Console.Write(new string(' ', (Console.WindowWidth) / 2));
            Console.WriteLine("4) Elevator is at GROUND FLOOR.");
            Console.Write(new string(' ', (Console.WindowWidth) / 2));
            Console.WriteLine("5) Please press Q/q to exit.\n ");
            Console.Write(new string(' ', (Console.WindowWidth) / 2));
            Console.WriteLine("//--------------------------------------------------------// \n ");


        Start:
            Console.WriteLine("How many floors are in the building?");
            string tempFloors = string.Empty;
            tempFloors = Console.ReadLine();

            if (Int32.TryParse(tempFloors, out totalNumberOfFloors))
            {
                iElevatorRef = new Elevator(totalNumberOfFloors);
            }
            else if (tempFloors == Common.QUIT)
            {
                Common.QuitApplication();
            }
            else
            {
                Common.WrongInput();
                goto Start;
            }

        SpFloor:
            Console.WriteLine("Call elevator on specific floor.");
            string spFloor = Console.ReadLine();
            int callOnFloor;
            bool isElevatorGoingUp = true;
            if (Int32.TryParse(spFloor, out callOnFloor))
            {
                if (Common.ValidateFloorInput(callOnFloor, totalNumberOfFloors))
                {
                    isElevatorGoingUp = iElevatorRef.FloorButtonPress(callOnFloor);
                }
                else goto SpFloor;
            }
            else if (spFloor == Common.QUIT)
            {
                Common.QuitApplication();
            }
            else
            {
                Common.WrongInput();
                goto Start;
            }

            int personCount;
            Console.WriteLine("Please enter number of passengers.");
            string tempPassengerCnt = Console.ReadLine();

            if (Int32.TryParse(tempPassengerCnt, out personCount))
            {
                if (personCount == 0 || personCount <= 0)
                {
                    Console.WriteLine("Please enter valid number.");
                    goto SpFloor;
                }
                else
                {
                    //if (personCount > Common.allowedPassenger) 
                    //{

                    //}

                    Dictionary<string, int> objPassengersinLift = new Dictionary<string, int>();
                    objPassengersinLift = iElevatorRef.AddPassenger(personCount);

                    if (isElevatorGoingUp)
                    {
                        List<KeyValuePair<string, int>> objPassengersGoingUp = new List<KeyValuePair<string, int>>();
                        objPassengersGoingUp = objPassengersinLift.Where(x => x.Value > callOnFloor).OrderBy(x => x.Value).ToList();

                        foreach (var item in objPassengersGoingUp)
                        {
                            iElevatorRef.FloorButtonPress(item.Value);
                            Console.WriteLine("Passenger {0} going out..!!", item.Key);
                            objPassengersinLift.Remove(item.Key);
                            Console.WriteLine("Number of passenger's in Elevator: {0} \n", objPassengersinLift.Count);
                            Thread.Sleep(3000);
                        }
                        isElevatorGoingUp = false;
                    }
                    if (!isElevatorGoingUp)
                    {
                        List<KeyValuePair<string, int>> objPassengersGoingDown = new List<KeyValuePair<string, int>>();
                        objPassengersGoingDown = objPassengersinLift.Where(x => x.Value <= callOnFloor).OrderByDescending(x => x.Value).ToList();

                        foreach (var item in objPassengersGoingDown)
                        {
                            iElevatorRef.FloorButtonPress(item.Value);
                            Console.WriteLine("Passenger {0} going out..!!", item.Key);
                            objPassengersinLift.Remove(item.Key);
                            Console.WriteLine("Number of passenger's in Elevator: {0} \n", objPassengersinLift.Count);
                            Thread.Sleep(3000);
                        }

                    }

                    if (objPassengersinLift.Count == 0)
                    {
                        Console.WriteLine("\n All passengers are out..!!");
                        Console.WriteLine("Elevator is ideal..!!");
                        Thread.Sleep(2000);
                        goto SpFloor;
                    }
                }
            }
            else if (tempPassengerCnt == Common.QUIT)
            {
                Common.QuitApplication();
            }
            else
            {
                Common.WrongInput();
                goto Start;
            }

        }


    }

    public static class Common
    {
        public static string QUIT = "q";
        public static int allowedPassenger = 5;
        public static int elevatorMaxLoad = 250;


        public enum ElevatorStatus
        {
            UP,
            STOPPED,
            DOWN
        }


        public static Boolean ValidateFloorInput(int floor, int totalNumberOfFloors)
        {
            if (floor > totalNumberOfFloors || floor < 0)
            {
                Console.WriteLine("We have {0} floors", totalNumberOfFloors);
                Console.WriteLine("Please enter floor number between 0 to {0}", totalNumberOfFloors);
                return false;
            }
            return true;

        }

        public static void QuitApplication()
        {
            Console.WriteLine("Thank you for using our system.! \n ");
            Thread.Sleep(2000);
            Console.WriteLine("Have a nice day ahead.!!");
            Thread.Sleep(2000);
            Console.Clear();
            Environment.Exit(0);
        }

        public static void WrongInput()
        {
            Console.WriteLine("Wrong input...!!");
            Console.WriteLine("Restarting the system...!!");
            Console.Beep();
            Thread.Sleep(2000);
            Console.Clear();
        }
    }

    public interface IElevator
    {
        void StopOnFloor(int floorNum);
        void GoingUp(int floorNum);
        void GoingDown(int floorNum);
        void OnSameFloor();
        bool FloorButtonPress(int floorNum);
        Dictionary<string, int> AddPassenger(int count);
    }

    public class Elevator : IElevator
    {
        private bool[] destinationFloorList;
        public int currentFloor = 0;
        private int topfloor;
        public Common.ElevatorStatus status = Common.ElevatorStatus.STOPPED;


        public Elevator(int totalNumberOfFloors = 10)
        {
            destinationFloorList = new bool[totalNumberOfFloors + 1];
            topfloor = totalNumberOfFloors;
        }

        public void StopOnFloor(int floorNum)
        {
            status = Common.ElevatorStatus.STOPPED;
            currentFloor = floorNum;
            destinationFloorList[floorNum] = false;
            Console.WriteLine("Stopped at floor {0}", floorNum);
        }

        public void GoingDown(int floorNum)
        {
            Console.WriteLine("Elevator is moving down");

            for (int i = currentFloor; i >= 1; i--)
            {

                if (destinationFloorList[i])
                {
                    StopOnFloor(floorNum);
                    break;
                }
                else
                {
                    Console.WriteLine("Elevator is at floor {0}", i);
                    Thread.Sleep(2000);
                    continue;
                }
            }

            Console.WriteLine("Doors are opening");
            status = Common.ElevatorStatus.STOPPED;
            Console.WriteLine("Waiting..");
        }

        public void GoingUp(int floorNum)
        {
            Console.WriteLine("Elevator is moving up");
            for (int i = currentFloor; i <= topfloor; i++)
            {
                if (destinationFloorList[i])
                { StopOnFloor(floorNum); break; }
                else
                {
                    Console.WriteLine("Elevator is at floor {0}", i);
                    Thread.Sleep(2000);
                    continue;
                }
            }

            status = Common.ElevatorStatus.STOPPED;
            Console.WriteLine("Doors are opening..!!");
            Console.WriteLine("Waiting..");
        }

        public void OnSameFloor()
        {
            Console.WriteLine("That's our current floor");
        }

        public bool FloorButtonPress(int floorNum)
        {
            bool isElevatorGoingUp = true;
            Console.WriteLine("Doors are closing..!!");

            destinationFloorList[floorNum] = true;

            switch (status)
            {
                case Common.ElevatorStatus.DOWN:
                    isElevatorGoingUp = false;
                    GoingDown(floorNum);
                    break;

                case Common.ElevatorStatus.STOPPED:
                    if (currentFloor < floorNum)
                    {
                        isElevatorGoingUp = true;
                        GoingUp(floorNum);
                    }
                    else if (currentFloor == floorNum)
                        OnSameFloor();
                    else
                    {
                        isElevatorGoingUp = false;
                        GoingDown(floorNum);
                    }
                    break;

                case Common.ElevatorStatus.UP:
                    isElevatorGoingUp = true;
                    GoingUp(floorNum);
                    break;

                default:
                    break;
            }

            return isElevatorGoingUp;

        }

        public Dictionary<string, int> AddPassenger(int personCount)
        {
            Dictionary<string, int> objPersons = new Dictionary<string, int>();

            for (int i = 1; i <= personCount; i++)
            {
                if (i <= Common.allowedPassenger)
                {
                    int personFloor;

                FloorPress:
                    Console.WriteLine("Please press which floor person {0} would like to go to", i);
                    string tempPrFlr = Console.ReadLine();
                    if (Int32.TryParse(tempPrFlr, out personFloor))
                    {
                        if (Common.ValidateFloorInput(personFloor, topfloor))
                        {
                            if (currentFloor == personFloor)
                            {
                                OnSameFloor();
                                Thread.Sleep(2000);
                                goto FloorPress;
                            }
                            else
                            {
                                string prName = "Person " + i;
                                objPersons.Add(prName, personFloor);
                            }
                        }
                        else goto FloorPress;
                    }
                    else if (tempPrFlr == Common.QUIT)
                    {
                        Common.QuitApplication();
                    }
                    else
                    {
                        Console.WriteLine("You have pressed an incorrect floor, Please try again");
                        goto FloorPress;
                    }
                }
                else
                {
                    Console.WriteLine("\nElevator is full..!!");
                    Console.WriteLine("\n No. of passenger allowed : {0}", Common.allowedPassenger);
                    Console.WriteLine("No of waiting passenger on floor {0} is {1}..!! \n", currentFloor, (personCount - Common.allowedPassenger));
                    //Waiting queue logic

                }
            }

            return objPersons;

        }

    }

}
