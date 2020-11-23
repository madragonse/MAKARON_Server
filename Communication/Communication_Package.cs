using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace communication
{
    public class Communication_Package
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
            this.dataString = "<PACKAGE>";
            this.dataString += "<type>PING</type>";
            this.dataString += "</PACKAGE>";
        }
 

        public void SetTypeQUIT_SERVER()
        {
            this.dataString = "<PACKAGE>";
            this.dataString += "<type>QUIT_SERVER</type>";
            this.dataString += "</PACKAGE>";
        }

        public void SetTypeQUIT_LOBBY()
        {
            this.dataString = "<PACKAGE>";
            this.dataString += "<type>QUIT_LOBBY</type>";
            this.dataString += "</PACKAGE>";
        }

        public void SetTypeQUIT_GAME()
        {
            this.dataString = "<PACKAGE>";
            this.dataString += "<type>QUIT_GAME</type>";
            this.dataString += "</PACKAGE>";
        }
        #endregion
        #region multipleargs
        public void SetTypeCHOOSE(int id)
        {
            this.dataString = "<PACKAGE>";
            this.dataString += "<type>CHOOSE</type><arg1>" + id.ToString() + "</arg1>";
            this.dataString += "</PACKAGE>";
        }

        public void SetTypeLOGIN(String username, String password)
        {
            this.dataString = "<PACKAGE>";
            this.dataString += "<type>LOGIN</type><arg1>" + username + "</arg1><arg2>" + password + "</arg2>";
            this.dataString += "</PACKAGE>";
        }

        public void SetTypeLOGIN_CONFIRM(String username)
        {
            this.dataString = "<PACKAGE>";
            this.dataString += "<type>LOGIN_CONFIRM</type><arg1>" + username + "</arg1>";
            this.dataString += "</PACKAGE>";
        }

        public void SetTypeLOGIN_REFUSE(String username, String reason)
        {
            this.dataString = "<PACKAGE>";
            this.dataString += "<type>LOGIN_REFUSE</type><arg1>" + username + "</arg1><arg2>" + reason + "</arg2>";
            this.dataString += "</PACKAGE>";
        }

        public void SetTypeSIGNUP(String username, String password, String confirmPassword)
        {
            this.dataString = "<PACKAGE>";
            this.dataString += "<type>SIGNUP</type><arg1>" + username + "</arg1><arg2>" + password + "</arg2><arg3>" + confirmPassword + "</arg3>";
            this.dataString += "</PACKAGE>";
        }

        public void SetTypeSIGNUP_CONFIRM(String username)
        {
            this.dataString = "<PACKAGE>";
            this.dataString += "<type>SIGNUP_CONFIRM</type><arg1>" + username + "</arg1>";
            this.dataString += "</PACKAGE>";
        }
        public void SetTypeSIGNUP_REFUSE(String username, String reason)
        {
            this.dataString = "<PACKAGE>";
            this.dataString += "<type>SIGNUP_REFUSE</type><arg1>" + username + "</arg1><arg2>" + reason + "</arg2>";
            this.dataString += "</PACKAGE>";
        }
        public void SetTypeGLOBAL_MESSAGE(int senderId, String senderUsername, String message)
        {
            this.dataString = "<PACKAGE>";
            this.dataString += "<type>GLOBAL_MESSAGE</type><arg1>" + senderId.ToString() + "</arg1><arg2>" + senderUsername + "</arg2><arg3>" + message + "</arg3>";
            this.dataString += "</PACKAGE>";
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
            this.dataString = "<PACKAGE>";
            this.dataString += "<type>LOGIN_REQUEST</type>";
            this.dataString = "</PACKAGE>";
        }
        public void SetTypeSIGNUP_REQUEST()
        {
            this.dataString = "<PACKAGE>";
            this.dataString += "<type>SIGNUP_REQUEST</type>";
            this.dataString += "</PACKAGE>";
        }

        public void SetTypeERROR(String message)
        {
            this.dataString = "<PACKAGE>";
            this.dataString += "<type>ERROR</type><arg1>"+message+"</arg1>";
            this.dataString += "</PACKAGE>";
        }

        public void SetTypeCHOICE_REQUEST(String whatIsRequested)
        {
            this.dataString = "<PACKAGE>";
            this.dataString += "<type>CHOICE_REQUEST</type><arg1>"+whatIsRequested+"</arg1>";
            this.dataString += "</PACKAGE>";
        }

        public void SetTypeLIST(List<String> list)
        {
            this.dataString = "<PACKAGE>";
            this.dataString += "<type>LIST</type>";
            int i = 1;
            foreach(String s in list)
            {
                this.dataString += "<arg" + i + ">" + s + "</arg" + i + ">";
            }
            this.dataString += "</PACKAGE>";
        }

        public void SetTypeBack()
        {
            this.dataString = "<PACKAGE>";
            this.dataString += "<type>BACK</type>";
            this.dataString += "</PACKAGE>";
        }
      public void SetTypeCREATE_LOBBY(String gameType,String lobbyName)
        {
            this.dataString = "<PACKAGE>";
            this.dataString += "<type>CREATE_GAME</type>";
            this.dataString += "<arg1>"+gameType+"</arg1>";
            this.dataString += "<arg2>" + lobbyName + "</arg2>";
            this.dataString += "</PACKAGE>";
        }
        public void SetTypeJOIN_LOBBY(String lobbyId)
        {
            this.dataString = "<PACKAGE>";
            this.dataString += "<type>JOIN_LOBBY</type>";
            this.dataString += "<arg1>" + lobbyId + "</arg1>";
            this.dataString += "</PACKAGE>";
        }

        public void refreshByteArray()
        {
            this.data= Encoding.ASCII.GetBytes(this.dataString);
        }
    }
}
