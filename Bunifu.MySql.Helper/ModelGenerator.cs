using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace Bunifu.Data.Helper
{
   public static class ModelGenerator
    {
         
        public static void GenerateModel(DataTable Table,string ModelName,bool appendClipboard=false)
        {
            string str = "/****"+ModelName.ToUpper()+" MODEL****/\n" +
                "public class "+ModelName+"\n" +
                "{\n";
           
            foreach (DataColumn column in Table.Columns)
            {
                str += "  public " + column.DataType.Name + " " + column+" { get; set; }\n";
            }
            str += "}";

            if (appendClipboard)
            {
                string cp = Clipboard.GetText();
                Clipboard.SetText( cp+"\n\n\n"+ str);
            }
            else
            {
                Clipboard.SetText(str);
            }

        }
    }
}
