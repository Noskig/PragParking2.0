using System;
using System.Collections.Generic;
using System.Text;

namespace Prag_Parking2._0
{
    public class Vehicles : IVehicle
    {
        //Basen för forden. Att skapa en klass för bussar är inte svårt. Mycket ligger inne redan. Typ pris-config för buss osv. Bara att skapa en ny klass och peta in den lite här och vart
        public string Identifier { get; set; }
        public int Size { get; set; } = 2;
        public DateTime VechicleInTime { get; set; }
        public string Type { get; set; } 


    }
}
