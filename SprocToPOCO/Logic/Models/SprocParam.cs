using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SprocToPOCO.Logic
{
    public class SprocParam
    {
        public bool OutParam { get; set; }
        public string Name { get; set; }
        public string Datatype { get; set; }
        public int? MaxLen { get; set; }        
    }
}
