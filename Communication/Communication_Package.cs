using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace communication
{
    class Communication_Package
    {
        public String dataString;
        public byte[] data;
        public String packageType;
        public List<String> arguments;

        #region constructors
        public Communication_Package(byte[] data)
        {
            this.data = data;
            this.dataString = BitConverter.ToString(this.data);
            arguments = new List<String>();
        }

        public Communication_Package() { }

        #endregion
        #region noargs
        public void SetTypePING()
        {
            this.dataString = "<type>PING</type>";
        }
 

        public void SetTypeQUIT_SERVER()
        {
            this.dataString = "<type>QUIT_SERVER</type>";
        }

        public void SetTypeQUIT_LOBBY()
        {
            this.dataString = "<type>QUIT_LOBBY</type>";
        }

        public void SetTypeQUIT_GAME()
        {
            this.dataString = "<type>QUIT_GAME</type>";
        }
        #endregion
        #region multipleargs
        public void SetTypeCHOOSE(int id)
        {
            this.dataString = "<type>CHOOSE</type><arg1>" + id.ToString() + "</arg1>";
        }

        public void SetTypeLOGIN(String username, String password)
        {
            this.dataString = "<type>LOGIN</type><arg1>" + username + "</arg1><arg2>" + password + "</arg2>";
        }

        public void SetTypeLOGIN_CONFIRM(String username)
        {
            this.dataString = "<type>LOGIN_CONFIRM</type><arg1>" + username + "</arg1>";
        }

        public void SetTypeLOGIN_REFUSE(String username, String reason)
        {
            this.dataString = "<type>LOGIN_REFUSE</type><arg1>" + username + "</arg1><arg2>" + reason + "</arg2>";
        }

        public void SetTypeSIGNUP(String username, String password, String confirmPassword)
        {
            this.dataString = "<type>SIGNUP</type><arg1>" + username + "</arg1><arg2>" + password + "</arg2><arg3>" + confirmPassword + "</arg3>";
        }

        public void SetTypeSIGNUP_CONFIRM(String username)
        {
            this.dataString = "<type>SIGNUP_CONFIRM</type><arg1>" + username + "</arg1>";
        }
        public void SetTypeSIGNUP_REFUSE(String username, String reason)
        {
            this.dataString = "<type>SIGNUP_REFUSE</type><arg1>" + username + "</arg1><arg2>" + reason + "</arg2>";
        }
        public void SetTypeGLOBAL_MESSAGE(int senderId, String senderUsername, String message)
        {
            this.dataString = "<type>GLOBAL_MESSAGE</type><arg1>" + senderId.ToString() + "</arg1><arg2>" + senderUsername + "</arg2><arg3>" + message + "</arg3>";
        }
        #endregion
        public byte[] ToByteArray()
        {
            byte[] result = Encoding.ASCII.GetBytes(dataString);
            this.data = result;
            return result;
        }

        public String toString()
        {
            String result = BitConverter.ToString(data);
            return result;
        }

        public  void Interpet()
        {
            if (data != null)
            {
                this.dataString = Encoding.UTF8.GetString(data, 0, data.Length);
                List<String> arguments = new List<String>();
                //parse into datatable
                DataTable dataTable = parseXMLIntoDataTable(dataString);
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    foreach (var item in dataRow.ItemArray)
                    {
                        arguments.Add((string)item);
                    }
                }
                this.packageType = arguments[0];
                //delete type from arguments list
                arguments.RemoveAt(0);
                this.arguments = arguments;
            }
        }

        private static DataTable parseXMLIntoDataTable(String dataString)
        {
            StringReader xmlStream = new StringReader(dataString);
            DataSet dataSet = new DataSet();
            dataSet.ReadXml(xmlStream);

            return dataSet.Tables[0];
        }

        public void SetTypeLOGIN_REQUEST()
        {
            this.dataString = "<type>LOGIN_REQUEST</type>";
        }
        public void SetTypeSIGNUP_REQUEST()
        {
            this.dataString = "<type>SIGNUP_REQUEST</type>";
        }

        public void SetTypeERROR(String message)
        {
            this.dataString = "<type>ERROR</type><arg1>"+message+"</arg1>";
        }

        public void SetTypeCHOICE_REQUEST(String whatIsRequested)
        {
            this.dataString = "<type>CHOICE_REQUEST</type><arg1>"+whatIsRequested+"</arg1>";
        }

        public void SetTypeLIST(List<String> list)
        {
            this.dataString = "<type>LIST</type>";
            int i = 1;
            foreach(String s in list)
            {
                this.dataString += "<arg" + i + ">" + s + "</arg" + i + ">";
            }
        }

        public void SetTypeBack()
        {
            this.dataString = "<type>BACK</type>";
        }

        public void refreshByteArray()
        {
            this.data= Encoding.ASCII.GetBytes(this.dataString);
        }
    }
}
