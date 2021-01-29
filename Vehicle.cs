using System;
using System.Collections.Generic;
using System.Text;

namespace Prag_Parking2._0
{
    public class Vehicles : IVehicle
    {
        public string Identifier { get; set; }
        public int Size { get; set; } = 2;
        public DateTime VechicleInTime { get; set; }
        public string Type { get; set; } 


    }
}
