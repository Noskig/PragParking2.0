using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Prag_Parking2._0
{
    class Program
    {
        //real
        static void Main(string[] args)
        {
            int GarageSize = 100;
            int ChargeCar = 20;
            int ChargeMc = 10;
            const int CarSize = 4;
            const int McSize = 2;
            string Platenumber;

            double FreeOfChargeTime = 10;

            List<Parkingspots> Garage = new List<Parkingspots>(); //Skapar listan som används för Garaget
            for (int i = 1; i < GarageSize + 1; i++)    //Fyller den med tomma platser
            {
                var spot = new Parkingspots(i);
                Garage.Add(spot);

            }
            Garage = LoadFromFile(Garage); //Laddar från text fil
            


            while (true)
            {
                WriteToFile(Garage);

                Console.Clear();
                int freeslots = NumberOfFreeSlots(Garage, CarSize);


                Console.WriteLine( "Total number of spots: {0} \t\t/Prag Parking Appclication/" +
                                 "\nHourly Charge: {1}$",GarageSize, ChargeCar);

                Console.WriteLine("\n" +
                    "Press 1 to Park Car\n" +
                    "Press 2 to Park MC\n" +
                    "Press 3 to Remove a Vehicle\n" +
                    "Press 4 to Move\n\n" +
                    "Press 5 to Check if platenumber is in Garage\n\n\n");


                FreeSpotsWrite(Garage, CarSize, McSize); //Vill jag att adet ska skriva ut index också? Mer jobb. ändra lite för bara 100rutor

                string selection = Console.ReadLine();
                switch (selection)
                {
                    case "1": //Park Car
                        Console.Clear();
                        Console.WriteLine("Enter Platenumber for the Car to park.\n ");
                        Platenumber = Console.ReadLine();
                        IVehicle car = new Car(Platenumber);

                        var spotForCar = IsGarageFull(Garage, car);
                        if (spotForCar != null)
                        {
                            spotForCar.AddVehicleToSpot(car);
                            Console.WriteLine("\nPark Car at spot #{0} ", spotForCar.GetId());
                            Console.ReadKey();
                        }
                        else
                        {
                            Console.Clear();

                            Console.WriteLine("Sry the Garage is Full.. Press any to continue");
                            Console.ReadLine();
                            break;
                        }
                        // Update list so it saves WriteToTheFile(Garage);
                        break;
                    case "2": //Park Mc
                        Console.Clear();
                        Console.WriteLine("Enter Platenumber for the MC to park.\n");
                        Platenumber = Console.ReadLine();
                        IVehicle mc = new Mc(Platenumber);

                        var spotForMC = IsGarageFull(Garage, mc);
                        if (spotForMC != null)
                        {
                            spotForMC.AddVehicleToSpot(mc);
                            Console.WriteLine("\nPark MC at #{0}", spotForMC.GetId());
                            Console.ReadKey();
                        }
                        else
                        {
                            Console.Clear();
                            Thread.Sleep(500);
                            Console.WriteLine("Sry the Garage is Full");
                        }
                        // Update list so it saves ... WriteToTheFile(Garage);
                        break;
                    case "3": //Remove Vehicle
                        Console.Clear();
                        Console.WriteLine("Enter Platenumber to remove..\n");

                        Platenumber = Console.ReadLine();
                        Parkingspots spot = SearchVehicle(Garage, Platenumber);

                        if (spot != null)
                        {
                            IVehicle vehicleToRemove = spot.RemoveVehicle(Platenumber);
                            if (vehicleToRemove != null)
                            {
                                DateTime currentTime = DateTime.UtcNow;
                                TimeSpan duration = currentTime.Subtract(vehicleToRemove.VechicleInTime);
                                var totaltimeinhours = (((duration.TotalMinutes) - FreeOfChargeTime) / 60);

                                if (vehicleToRemove is Car)
                                {
                                    Console.WriteLine("\n| Drive Car: {0} from #{1} to the customer|\n", Platenumber, spot.GetId());
                                    Console.WriteLine("|The Fee is ${0}|", (double)totaltimeinhours * ChargeCar);
                                }
                                if (vehicleToRemove is Mc)
                                {
                                    Console.WriteLine("\n| Drive Mc: {0} from #{1} to the customer |\n", Platenumber, spot.GetId());
                                    Console.WriteLine("|The Fee is ${0}", (double)totaltimeinhours * ChargeMc);
                                }

                                Console.ReadKey();
                            }
                            else
                            {
                                Console.WriteLine("Vehicle is not found in the Garage");
                            }
                            // Update list so it saves ... WriteToTheFile(Garage);
                        }
                        else
                        {
                            Console.WriteLine("Vehicle is not found in the Garage");
                        }
                        break;


                    case "4"://Move vehicle
                        Console.Clear();
                        Console.WriteLine("Enter platenumber to move..\n");
                        Platenumber = Console.ReadLine();

                        Parkingspots currentspot = SearchVehicle(Garage, Platenumber);
                        //IVehicle vehicleToMove = currentspot.GetVehicles().FirstOrDefault(x => x.Identifier == Platenumber);

                        if (currentspot != null && Platenumber != null)
                        {
                            IVehicle vehicleToMove = currentspot.GetVehicles().FirstOrDefault(x => x.Identifier == Platenumber);
                            Console.Clear();
                            Console.WriteLine("Enter number of the new parkingspot: ");
                            string newSpotNumber = Console.ReadLine();
                            var Spot = Garage.FirstOrDefault(x => x.GetId().ToString() == newSpotNumber);

                            if (Spot != null)
                            {
                                if (Spot.CanAdd(vehicleToMove.Size))
                                {
                                    if (vehicleToMove is Car)
                                    {
                                        Console.WriteLine("Move Car:{2} from spot #{0} to new spot #{1}", currentspot.GetId(), Spot.GetId(), Platenumber);
                                        Thread.Sleep(2500);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Move Mc:{2} from spot #{0} to new spot #{1}", currentspot.GetId(), Spot.GetId(), Platenumber);
                                        Thread.Sleep(2500);
                                    }
                                    currentspot.RemoveVehicle(Platenumber);
                                    Spot.AddVehicleToSpot(vehicleToMove);

                                    // Update list so it saves ... WriteToTheFile(Garage);
                                }
                                else
                                {
                                    Console.WriteLine("There is no space in your choosen parkingspot");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Cant find that parkingspot - might not exist");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Vehicle is not available in Garage");
                        }
                        break;

                            case "5"://Check if 
                        
                        break;
                }



            }


        }

        private static void FreeSpotsWrite(List<Parkingspots> list, int Carsize, int MCsize)
        {
            int counter = 0;
            foreach (var item in list)
            {

                int id = item.GetId();
                int availabelspace = item.GetFreeSpace();
                string extra = "";
                if (counter <9) //för att få kartan jämn
                {
                    extra = "  ";
                }
                else if(counter >= 9 && counter < 99)
                {
                    extra = " ";
                }

                if (availabelspace >= Carsize)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("| {0}{1} |", id,extra);
                }
                else if (availabelspace == MCsize)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write("| {0}{1} |", id,extra);
                }
                else if (availabelspace == 0)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write("| {0}{1} |", id,extra);
                }
            
                counter += 1; // för att hoppa rad
                if (counter % 10 == 0)
                {
                    Console.WriteLine();
                }
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static Parkingspots SearchVehicle(List<Parkingspots> garage, string platenumber)
        {

            foreach (var spot in garage)
            {
                if (spot.GetVehicles().FirstOrDefault(x => x.Identifier == platenumber) != null)
                {
                    return spot;
                }
            }
            Console.WriteLine("Could not find your Platenumber");
            Thread.Sleep(1500);
            return null;
        }
        public static int[] indexesFreeSpace(List<Parkingspots>garage, int GarageSize)
        {

            int[] indexes = new int[GarageSize];
            int count=0;
            foreach (var spot in garage)
            {

                if (spot.GetVehicles().FirstOrDefault(x => x.Identifier == null) == null) //Where it
                {
                    indexes[count] = spot.GetId();
                }
                count += 1;
            }

            return indexes;
        }

        public static Parkingspots IsGarageFull(List<Parkingspots> garage, IVehicle vehiceltoadd)
        {
            if (garage != null)
            {
                bool canAdd = false;
                foreach (var spot in garage)
                {
                    canAdd = spot.CanAdd(vehiceltoadd.Size);
                    if (canAdd)
                    {
                        return spot;
                    }

                }
                return null;
            }
            else
            {
                return null;
            }

        }
        public static int NumberOfFreeSlots(List<Parkingspots> slots, int carsize)
        {
            int counted = 0;

            foreach (var slot in slots)
            {
                int C = slot.GetFreeSpace();
                counted += C;
            }
            counted /= carsize; //size of car
            return counted;
        }
        public static List<Parkingspots> LoadFromFile(List<Parkingspots> mygarage)
        {
            List<Parkingspots> parkinggarage = mygarage;
            try
            {
                using (StreamReader file = new StreamReader(Environment.CurrentDirectory + "Garage.txt"))
                {
                    string ln;

                    while ((ln = file.ReadLine()) != null)
                    {
                        if (!string.IsNullOrEmpty(ln))
                        {
                            var VehicleData = ln.Split(',');
                            IVehicle vehicle;

                            string type = VehicleData.FirstOrDefault(x => x.Contains(Const.type)).Split(Const.seporator)[1];
                            string identifier = VehicleData.FirstOrDefault(x => x.Contains(Const.regnumber)).Split(Const.seporator)[1];
                            string VehicleIntime = VehicleData.FirstOrDefault(x => x.Contains(Const.intime)).Split(Const.seporator)[1];
                            string Id = VehicleData.FirstOrDefault(x => x.Contains(Const.Id)).Split(Const.seporator)[1];
                            int id = Int32.Parse(Id);
                            if (type == "Car")
                            {
                                vehicle = new Car(identifier, type, VehicleIntime);

                                var space = mygarage.FirstOrDefault(x => x.GetId() == Convert.ToInt32(Id));
                                if (space != null) //måste finnas en plats för att parkera
                                {
                                    space.AddVehicleToSpot(vehicle);
                                }
                                else
                                {

                                }
                            }
                            else if (type == "Mc")
                            {
                                vehicle = new Mc(identifier, type, VehicleIntime);

                                var space = mygarage.FirstOrDefault(x => x.GetId() == Convert.ToInt32(Id));
                                if (space != null) //måste finnas en plats för att parkera
                                {
                                    space.AddVehicleToSpot(vehicle);
                                }
                            }
                        }
                    }
                    file.Close();
                }
            }
            catch (Exception)
            {
            }
            return mygarage;
        }
        public static bool WriteToFile(List<Parkingspots> thegarage)
        {
            TextWriter tw = new StreamWriter(Environment.CurrentDirectory + "Garage.txt");
            foreach (var slot in thegarage)
            {
                foreach (IVehicle item in slot.GetVehicles())
                {
                    string line = string.Format("{0}{1}{2},{3}{1}{4},{5}{1}{6},{7}{1}{8}", Const.type, Const.seporator, item.Type,
                                                                                           Const.regnumber, item.Identifier,
                                                                                           Const.intime, item.VechicleInTime,
                                                                                           Const.Id, slot.GetId());

                    tw.WriteLine(line);

                }

            }
            tw.Close();
            return true;

        }
    }
}

