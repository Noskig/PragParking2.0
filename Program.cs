using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;

namespace Prag_Parking2._0
{
    class Program
    {
        //Prag parking 2.0
        //Jag har lämnat kommentarer i koden på hur saker fungerar. En större summering av projektet och kursen och kommer vid inlämning
        //Dessa kommentarer är mitt försök att förklara simpelt vad som händer där de behövs.




        static void Main(string[] args)
        {
            //function för att kontrollera config
            if (ConfigParseCheck() == false)
            {
                Console.WriteLine("something seems to be wrong with you config. make sure you use numbers");
                Console.ReadKey();
            }

            var GarageSize = int.Parse(ConfigurationManager.AppSettings["GARAGEsize"]);
            List<Parkingspots> Garage = new List<Parkingspots>(); //Skapar listan som används för Garaget
            for (int i = 1; i < GarageSize + 1; i++)    //Fyller den med tomma platser baserat på hur stort garage du vill ha. Ändra i config
            {
                var spot = new Parkingspots(i);
                Garage.Add(spot);

            }
            Garage = LoadFromFile(Garage); //Laddar sparade fordon
            


            while (true) //Körningen av application
            {
                WriteToFile(Garage); //I början sparas alla ändringar som gjorts till sjävla listan som är garaget

                if (ConfigParseCheck() == false) //Självförklarande
                {
                    Console.WriteLine("something seems to be wrong with you config. make sure you use numbers");
                    Console.ReadKey();
                }

                //Eftersom variablerna har kontrollerats kan vi parsa dom. Felmeddelande kommer om upp ändringen orsaka fel.
                var Parkingsize = int.Parse(ConfigurationManager.AppSettings["Parkingsize"]);
                var ChargeCar = int.Parse(ConfigurationManager.AppSettings["PricePerHourCAR"]);
                var ChargeMC = int.Parse(ConfigurationManager.AppSettings["PricePerHourMC"]);
                var CarSize = int.Parse(ConfigurationManager.AppSettings["CARsize"]);
                var McSize = int.Parse(ConfigurationManager.AppSettings["MCsize"]);
                var FreeParkingTimeMinutes = int.Parse(ConfigurationManager.AppSettings["FreeParkingTimeMinutes"]);

                string Platenumber;
                int freeslots = NumberOfFreeSlots(Garage, CarSize);

                Console.Clear();

                Console.WriteLine( "Total number of spots: {0} \t\t//Prag Parking Application//" +
                                 "\nHourly Charge cars: {1}$",GarageSize, ChargeCar);

                Console.WriteLine("\n" +
                    "Press 1 to Park Car\n" +
                    "Press 2 to Park MC\n" +
                    "Press 3 to Remove a Vehicle\n" +
                    "Press 4 to Move\n\n" +
                    "Press 6 to change Settings\n\n\n");


                FreeSpotsWrite(Garage, CarSize, McSize); //Kartan 
                Console.WriteLine();

                Console.Write("Press number and press enter :");
                string selection = Console.ReadLine();

                switch (selection)
                {
                    case "1": //Park Car
                        Console.Clear();
                        Console.WriteLine("Enter Platenumber for the Car to park.\n ");
                        Platenumber = Console.ReadLine();
                        IVehicle car = new Car(Platenumber); //Använder Ivehicle för att det ska vara lättare att lägga till i den nestade listan.

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

                            Console.WriteLine("Sry the Garage is Full");
                            Thread.Sleep(2000);
                        }
                        break;
                    case "3": //Remove/checkout Vehicle
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
                                var totaltimeinhours = (((duration.TotalMinutes) - FreeParkingTimeMinutes) / 60);

                                if (vehicleToRemove is Car)
                                {
                                    Console.WriteLine("\n| Drive Car: {0} from #{1} to the customer|\n", Platenumber, spot.GetId());
                                    Console.WriteLine("|The Fee is ${0}|", ((double)totaltimeinhours * ChargeCar).ToString("0.00"));
                                }
                                if (vehicleToRemove is Mc)
                                {
                                    Console.WriteLine("\n| Drive Mc: {0} from #{1} to the customer |\n", Platenumber, spot.GetId());
                                    Console.WriteLine("|The Fee is ${0}", ((double)totaltimeinhours * ChargeMC).ToString("0.00"));
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

                        if (currentspot != null && Platenumber != null)
                        {
                            IVehicle vehicleToMove = currentspot.GetVehicles().FirstOrDefault(x => x.Identifier == Platenumber);  //Kopierar det första forden med det regnummret
                            Console.Clear();
                            Console.WriteLine("Enter number of the new parkingspot: ");
                            string newSpotNumber = Console.ReadLine();
                            var Spot = Garage.FirstOrDefault(x => x.GetId().ToString() == newSpotNumber); //hittar Id för nya platsen

                            if (Spot != null)
                            {
                                if (Spot.CanAdd(vehicleToMove.Size)) //kollar om det finns plats 
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

                    case "6":
                        Console.Clear();
                        Console.WriteLine("What do you want to change?");
                        Console.WriteLine("1. Change Priceing\t\t Currently: {0} for cars and {6} for MC's\n" +
                            "2. Change Free Parking time\t Currently: {1}\n" +
                            "3. How big Car's are\t\t Current Size: {2}\n" +
                            "4. How big MC's are\t\t Current Size: {3}\n" +
                            "5. How big Parkingspots are\t Current Size: {4}\n" +
                            "6. How many Parkingspot is in the Garage.\t Currently: {5}",ChargeCar, FreeParkingTimeMinutes
                            , CarSize, McSize, Parkingsize, GarageSize, ChargeMC);


                        string choice;
                        choice = Console.ReadLine();

                        string x;
                        string setting;

                        switch (choice) //Det tog mig sån tid att hitta hur jag skulle göra detta. Var ute på många äventyr för att pricka rätt... Detta tog längst tid av allt i projektet av någon anledning. 
                                        //Sen kom jag på hur jag skulle göra och det blev lätt hehe.
                                        //Först var jag inne på att göra likadant som jag gjorde med Garage listan. Att typ variablerna skulle läsas in rad för rad från en textfil.
                                        //Men det kändes som det skulle finnas ett smidigare sätt och det fanns det med hjälp av 'using System.Configuration' 
                                        //Skrev min egna enkla funktion. åhh swoosh! de funka! Så glad! -Jonatan
                        {

                            case "1":
                                Console.Clear();
                                Console.WriteLine("1. change hourly rate of cars\n" +
                                    "2. Change hourly rate of Mc's");
                                choice = Console.ReadLine();
                                switch (choice)
                                {
                                    case "1":
                                        Console.WriteLine("How much you wanna charge an hour for Cars?");
                                         x = Console.ReadLine();
                                         setting = "PricePerHourCAR"; 

                                        ChangeSetting(setting, x);//
                                        break;
                                    case "2":
                                        Console.WriteLine("How much you wanna charge an hour for MC's?");
                                         x = Console.ReadLine();
                                         setting = "PricePerHourMC";

                                        ChangeSetting(setting, x);
                                        break;
                                }
                                break;
                            case "2":
                                Console.Clear();
                                Console.WriteLine("How many min of free parking do you want?");
                                 x = Console.ReadLine();
                                 setting = "FreeParkingTimeMinutes";

                                ChangeSetting(setting, x);
                                break;
                            case "3":
                                Console.Clear();
                                Console.WriteLine("Size of car in program.\t\t #Relates to logic of calculating how many vehicles can fit in one spot");
                                x = Console.ReadLine();
                                setting = "CARsize";

                                ChangeSetting(setting, x);
                                break;
                            case "4":
                                Console.Clear();
                                Console.WriteLine("Size of MC in program.\t\t #Relates to logic of calculating how many vehicles can fit in one spot");
                                x = Console.ReadLine();
                                setting = "MCsize";

                                ChangeSetting(setting, x);

                                break;
                            case "5":
                                Console.Clear();
                                Console.WriteLine("Size of Parkingspace in program.\t\t #Relates to logic of calculating how many vehicles can fit in one spot");
                                x = Console.ReadLine();
                                setting = "Parkingsize";

                                ChangeSetting(setting, x);

                                break;
                            case "6":
                                Console.Clear();
                                Console.WriteLine("So you are doing a remodeling of the entire garage?!?! Wow! Good! busy times!\n\n" +
                                    "How many parkingspots will your new garage have?\n" +
                                    "PS. Make sure no cars are hurt in your remodeling. Dont make the garage(list) smallar than the car parked in the highest index parkingslot.\n\n" +
                                    "new spots:");
                                x = Console.ReadLine();
                                setting = "GARAGEsize";

                                ChangeSetting(setting, x);
                                break;

                        }
                        Console.ReadKey();
                        
                        break;
                }



            }


        }


        private static bool ConfigParseCheck() //parsar alla config-värden
        {
            var Parkingsize = ConfigurationManager.AppSettings["Parkingsize"];
            var GarageSize = ConfigurationManager.AppSettings["GARAGEsize"];
            var ChargeCar = ConfigurationManager.AppSettings["PricePerHourCAR"];
            var ChargeMC = ConfigurationManager.AppSettings["PricePerHourMC"];
            var CarSize = ConfigurationManager.AppSettings["CARsize"];
            var McSize = ConfigurationManager.AppSettings["MCsize"];
            var FreeParkingTimeMinutes = ConfigurationManager.AppSettings["FreeParkingTimeMinutes"];

            int check = 0;

            if (int.TryParse(Parkingsize, out check) && int.TryParse(FreeParkingTimeMinutes, out check) && int.TryParse(GarageSize, out check) && int.TryParse(ChargeCar, out check) && int.TryParse(ChargeMC, out check) && int.TryParse(CarSize, out check) && int.TryParse(McSize, out check))
            {
                return true;
            }
            else
            {

                Console.WriteLine("Something wrong with the configfile. Make sure you use numbers");
                return false;
            }



        }


        private static void FreeSpotsWrite(List<Parkingspots> list, int Carsize, int MCsize) //kartan
        {
            int counter = 0;
            foreach (var item in list)
            {

                int id = item.GetId();
                int availabelspace = item.GetFreeSpace();
                string extra = "";//för att få kartan jämn
                if (counter <9) 
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
            Console.WriteLine("Could not find your Platenumber"); //retunerar den inte nått så hittas de inte
            Thread.Sleep(1500);
            return null;
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
        public static int NumberOfFreeSlots(List<Parkingspots> slots, int carsize) //based on carsize
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
        public static List<Parkingspots> LoadFromFile(List<Parkingspots> mygarage) //Denna var också gräslig innan jag kom på hur jag skulle tänka och lägga upp det. Men tillslut så! :D
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
        public static bool WriteToFile(List<Parkingspots> thegarage) // 
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
        public static void ChangeSetting(string settingname, string newnumber)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            config.AppSettings.Settings[settingname].Value = newnumber;
            config.Save(ConfigurationSaveMode.Modified);
            Console.Clear();
            Console.WriteLine("Restart the application to apply");
            return;
        }
    }
}

