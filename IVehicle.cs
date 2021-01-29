using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Prag_Parking2._0
{

    public interface IVehicle //Interface för att göra det simplare att lägga olika typer av fordon i listan
    {
        string Identifier { get; set; }

        public int Size { get; set; }

        string Type { get; set; }

        DateTime VechicleInTime { get; set; }


    }
}
