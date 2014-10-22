using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SprocToPOCO.Logic
{
    public class SQLWriter
    {

        const string CREATE_PROC = "CREATE PROC [dbo].[{0}]";
        /// <summary>
        /// Abandoned: miało budować szkielet procedur
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public static string ToInsertSproc(string tableName, List<SprocParam> pars)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(string.Format(CREATE_PROC, tableName + "Add"));
            sb.AppendLine("(");


            sb.AppendLine(")");
            sb.AppendLine("AS");
            sb.AppendLine("BEGIN");

            foreach (var item in pars)
            {
                if (item.Datatype.Contains("char"))
                {
                    sb.AppendLine(string.Format("@{0} {1}({2})", item.Name, item.Datatype, item.MaxLen));
                }
                else
                {
                    sb.AppendLine(string.Format("@{0} {1}", item.Name, item.Datatype));
                }
            }

            sb.AppendLine("END");
            sb.AppendLine("GO");

            return sb.ToString();
        }
    }
}
