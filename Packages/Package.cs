using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace packages
{
    public class Package
    {
        public String XML = null;

        #region ctors

        public Package(String s)
        {
            this.XML = s;
        }

        public Package(byte[] data)
        {
            this.XML = Encoding.ASCII.GetString(data, 0, data.Length);
        }

        public Package() { }
        #endregion
        public byte[] ToByteArray()
        {
            byte[] result = Encoding.ASCII.GetBytes(XML);
            return result;
        }

        public String toString()
        {
            return XML;
        }

        public List<String> getArguments()
        {
            if (XML != null)
            {
                try
                {
                    List<String> arguments = new List<String>();
                    //parse into datatable
                    DataTable dataTable = parseXMLIntoDataTable(XML);
                    foreach (DataRow dataRow in dataTable.Rows)
                    {
                        foreach (var item in dataRow.ItemArray)
                        {
                            arguments.Add((string)item);
                        }
                    }
                    return arguments;
                }
                catch(Exception e) { Console.WriteLine(e.GetType());}
                //delete type from arguments list
                return new List<String>();
            }
            else return new List<String>();
        }
        private static DataTable parseXMLIntoDataTable(String dataString)
        {
            StringReader xmlStream = new StringReader(dataString);
            DataSet dataSet = new DataSet();
            dataSet.ReadXml(xmlStream);
            return dataSet.Tables[0];
        }
    }
}

