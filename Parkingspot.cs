using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;



namespace Prag_Parking2._0
{
    public class Parkingspots //Basen för fordonen på listan. Här hamnar funtioner för att lägga till och ta bort.
    {


        private List<IVehicle> VehiclesInSpot =
            new List<IVehicle>();   // I parkeringsplatsen finns denna lista av fordon som kan stå där. 
                                    // Varje fordon som läggs till minskar uttrymmet på platsen och vice versa.


        private int takenSpace { get; set; } = 0 ;
        private int FreeSpace;
        private int ID = 0;
        private int size = int.Parse(ConfigurationManager.AppSettings["Parkingsize"]);


        public Parkingspots(int id_in)
        {
            ID = id_in;
            FreeSpace = size; 
        }

        public bool AddVehicleToSpot(IVehicle vehicle)
        {
            if (CanAdd(vehicle.Size))
            {
                VehiclesInSpot.Add(vehicle);
                FreeSpace -= vehicle.Size;
                return true;
            }
            return false;
        }
        public IVehicle RemoveVehicle(string platenumberIN)
        {
            var vehicle = VehiclesInSpot.FirstOrDefault(x => x.Identifier == platenumberIN);
            if (vehicle != null)
            {
                VehiclesInSpot.Remove(vehicle);
                FreeSpace += vehicle.Size;
                return vehicle;
            }
            return null;

        }
        public bool CanAdd(int space)
        {
            if (takenSpace + space > FreeSpace)
            {
                return false;
            }
            return true;
        }
        public int GetId()
        {
            return ID;
        }
        public int GetFreeSpace()
        {
            return FreeSpace;
        }
        public List<IVehicle> GetVehicles()
        {
            return VehiclesInSpot;
        }
    }
}
