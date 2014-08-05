using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SprocToPOCO.Logic
{
    public class CSharWriter
    {
        const string propertyGetSet = "{ get; set; }";


        public static string FixType(string typ, bool nullable)
        {
            string s = typ;
            s = s.Replace("System.Int32", "int");
            s = s.Replace("System.Int16", "int");
            s = s.Replace("System.String", "string");
            s = s.Replace("System.Decimal", "decimal");
            s = s.Replace("System.DateTime", "DateTime");
            s = s.Replace("System.Boolean", "bool");

            if (nullable && typ != "System.String")
                s = "Nullable<" + s + ">";

            return s;
        }

        public static string ToPOCO(string className, List<SprocColumn> cols, bool addAnnotations = true, string resourceName = "Resources.Strings")
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("public class "+className+" {");
            String row = "";
            String t = "";
            String comment = "";
            foreach (var c in cols)
            {
                comment = "";

                if (addAnnotations)
                {
                    if (c.Type == "System.String")
                    {
                        sb.AppendLine("");
                        sb.AppendLine(string.Format("    [StringLength({0}, ErrorMessageResourceName = \"FIELDTOOLONG\", ErrorMessageResourceType = typeof({1}))]", c.Length, resourceName));
                    }

                    if (c.Type == "System.DateTime")
                    {
                        sb.AppendLine("");
                        sb.AppendLine("    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = \"{0:yyyy-MM-dd}\")]");
                    }

                }

                if (c.Type == "System.Int16")
                {
                    comment = " // tinyint";
                }


                sb.AppendLine("    " + string.Format("public {1} {0}", c.Name, FixType(c.Type, c.IsNullable)) + " " + propertyGetSet + comment);
            
            }

            sb.AppendLine("}");

            return sb.ToString();
        }


        public static string ToDataProvider(string sprocName, List<SprocParam> pars)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(string.Format("dataprovider.StoredProc(\"{0}\")", sprocName));

            foreach (var p in pars)
            {
                if (p.Datatype.Contains("char"))
                {
                    sb.AppendLine(string.Format(".Param{0}(\"{1}\", entity.{2}, {3})", p.OutParam ? "Out" : "", p.Name, p.Name.Replace("@", ""), p.MaxLen));
                }
                else
                {
                    sb.AppendLine(string.Format(".Param{0}(\"{1}\", entity.{2})", p.OutParam ? "Out" : "", p.Name, p.Name.Replace("@", "")));
                }
                
            }

            return sb.ToString();
        }
    }
}
