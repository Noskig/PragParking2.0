using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Prag_Parking2._0
{

    public interface IVehicle 
    {
        string Identifier { get; set; }

        public int Size { get; set; }

        string Type { get; set; }

        DateTime VechicleInTime { get; set; }


    }
}
