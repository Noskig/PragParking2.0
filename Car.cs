using System;
using System.Collections.Generic;
using System.Text;

namespace Prag_Parking2._0
{
    public class Car : Vehicles, IVehicle
    {
        public Car(string platenumber)
        {
            Identifier = platenumber;
            Size= 4;
            Type = "Car"; 
            VechicleInTime = DateTime.UtcNow;
        }

        public Car(string platenumber, string type, string intime) //Re-create car for list
        {
            Identifier = platenumber;
            Size = 4;
            Type = type;
            VechicleInTime = Convert.ToDateTime(intime);
        }
    }
}
