using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Prag_Parking2._0
{//Real
    public class Parkingspots
    {
        private List<IVehicle> VehiclesInSpot =
            new List<IVehicle>();
        private int takenSpace { get; set; } = 0 ;
        private int FreeSpace = 4;
        private int ID = 0;


       

        public Parkingspots(int id_in)
        {
            ID = id_in;
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
