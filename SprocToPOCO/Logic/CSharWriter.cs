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

            sb.AppendLine("public class "+className+"Model {");
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


        private static string SqlTypeToCSarp(string typ)
        {
            string s = typ;
            s = s.Replace("varchar", "string");
            s = s.Replace("bit", "bool");

            
            return s;
        }

        private static string FixParameterName(string name, bool isEntity)
        {
            name = name.Replace("@", "");
            if (isEntity)
            {
                name = "entity." + name;
            }
            else
            {
                name = Char.ToLowerInvariant(name[0]) + name.Substring(1);
            }
            return name;
        }

        public static string ToDataProvider(string sprocName, List<SprocParam> pars, bool isSprocReturnsRowset, bool paramsAsEntity = false)
        {
            StringBuilder sb = new StringBuilder();

            string methodParam = "T entity  ";
            if (!paramsAsEntity)
            {
                methodParam = "";
                foreach (var p in pars)
                {
                    methodParam += SqlTypeToCSarp(p.Datatype) + " " + FixParameterName(p.Name, false) + ", ";
                }
            }
            methodParam = methodParam.Substring(0, methodParam.Length - 2); // ostatni przecienk

            if (isSprocReturnsRowset)
            {
                sb.AppendLine(string.Format("public IEnumerable<{0}Model> {0}({1})", sprocName, methodParam));
                
            } else 
            {
                sb.AppendLine(string.Format("public void {0}({1})", sprocName, methodParam));
            }

            sb.AppendLine("{");

            sb.AppendLine(string.Format("{1}provider.StoredProc(\"{0}\")", sprocName, isSprocReturnsRowset ? "return " : ""));

            foreach (var p in pars)
            {
                if (p.Datatype.Contains("char"))
                {
                    sb.AppendLine(string.Format(".Param{0}(\"{1}\", {2}, {3})", p.OutParam ? "Out" : "", p.Name, FixParameterName(p.Name, paramsAsEntity), p.MaxLen));
                }
                else
                {
                    sb.AppendLine(string.Format(".Param{0}(\"{1}\", {2})", p.OutParam ? "Out" : "", p.Name, FixParameterName(p.Name, paramsAsEntity)));
                }
                
            }

            if (isSprocReturnsRowset)
            {
                sb.AppendLine(string.Format(".ExecuteList<{0}>();", sprocName+"Model"));

            }
            else
            {
                sb.AppendLine(string.Format(".ExecuteNonQuery();"));
            }

            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
