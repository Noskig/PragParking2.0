using System;
using System.Collections.Generic;
using System.Text;

namespace Prag_Parking2._0
{
    public class Mc : Vehicles, IVehicle
    {
        public Mc(string platenumber)
        {
            Identifier = platenumber;
            Size = 2;
            Type = "Mc"; 
            VechicleInTime = DateTime.UtcNow;
        }

        public Mc(string platenumber, string type, string intime) //Re-create mc for load
        {
            Identifier = platenumber;
            Size = 2;
            Type = type;
            VechicleInTime = Convert.ToDateTime(intime);
        }
    }
}
