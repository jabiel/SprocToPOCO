using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SprocToPOCO.Logic
{
    public class TableColumn
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int Length { get; set; }
        public bool IsNullable { get; set; }
        public bool IsIdentity { get; set; }
    }
}
