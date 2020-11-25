
using System;
using System.Collections.Generic;

namespace communication
{

    public class Communication_Package :Package
{
    #region ctors
    public Communication_Package(byte[] data) : base(data)
    { }

    public Communication_Package() { }
    #endregion

    #region noargs
    public void SetTypePING()
    {
        this.XML = "<PACKAGE>";
        this.XML += "<type>PING</type>";
        this.XML += "</PACKAGE>";
    }


    public void SetTypeQUIT_SERVER()
    {
        this.XML = "<PACKAGE>";
        this.XML += "<type>QUIT_SERVER</type>";
        this.XML += "</PACKAGE>";
    }

    public void SetTypeQUIT_LOBBY()
    {
        this.XML = "<PACKAGE>";
        this.XML += "<type>QUIT_LOBBY</type>";
        this.XML += "</PACKAGE>";
    }

    public void SetTypeQUIT_GAME()
    {
        this.XML = "<PACKAGE>";
        this.XML += "<type>QUIT_GAME</type>";
        this.XML += "</PACKAGE>";
    }
    public void SetTypeREQUEST_GAME_LIST()
    {
        this.XML = "<PACKAGE>";
        this.XML += "<type>REQUEST_GAME_LIST</type>";
        this.XML += "</PACKAGE>";
    }
    #endregion

    #region multipleargs
    public void SetTypeCHOOSE(int id)
    {
        this.XML = "<PACKAGE>";
        this.XML += "<type>CHOOSE</type><arg1>" + id.ToString() + "</arg1>";
        this.XML += "</PACKAGE>";
    }

    public void SetTypeLOGIN(String username, String password)
    {
        this.XML = "<PACKAGE>";
        this.XML += "<type>LOGIN</type><arg1>" + username + "</arg1><arg2>" + password + "</arg2>";
        this.XML += "</PACKAGE>";
    }

    public void SetTypeLOGIN_CONFIRM(String username)
    {
        this.XML = "<PACKAGE>";
        this.XML += "<type>LOGIN_CONFIRM</type><arg1>" + username + "</arg1>";
        this.XML += "</PACKAGE>";
    }

    public void SetTypeLOGIN_REFUSE(String username, String reason)
    {
        this.XML = "<PACKAGE>";
        this.XML += "<type>LOGIN_REFUSE</type><arg1>" + username + "</arg1><arg2>" + reason + "</arg2>";
        this.XML += "</PACKAGE>";
    }

    public void SetTypeSIGNUP(String username, String password)
    {
        this.XML = "<PACKAGE>";
        this.XML += "<type>SIGNUP</type><arg1>" + username + "</arg1><arg2>" + password + "</arg2>";
        this.XML += "</PACKAGE>";
    }

    public void SetTypeSIGNUP_CONFIRM(String username)
    {
        this.XML = "<PACKAGE>";
        this.XML += "<type>SIGNUP_CONFIRM</type><arg1>" + username + "</arg1>";
        this.XML += "</PACKAGE>";
    }
    public void SetTypeSIGNUP_REFUSE(String username, String reason)
    {
        this.XML = "<PACKAGE>";
        this.XML += "<type>SIGNUP_REFUSE</type><arg1>" + username + "</arg1><arg2>" + reason + "</arg2>";
        this.XML += "</PACKAGE>";
    }
    public void SetTypeGLOBAL_MESSAGE(int senderId, String senderUsername, String message)
    {
        this.XML = "<PACKAGE>";
        this.XML += "<type>GLOBAL_MESSAGE</type><arg1>" + senderId.ToString() + "</arg1><arg2>" + senderUsername + "</arg2><arg3>" + message + "</arg3>";
        this.XML += "</PACKAGE>";
    }
    #endregion




    public void SetTypeERROR(String message)
    {
        this.XML = "<PACKAGE>";
        this.XML += "<type>ERROR</type><arg1>" + message + "</arg1>";
        this.XML += "</PACKAGE>";
    }

    public void SetTypeCHOICE_REQUEST(String whatIsRequested)
    {
        this.XML = "<PACKAGE>";
        this.XML += "<type>CHOICE_REQUEST</type><arg1>" + whatIsRequested + "</arg1>";
        this.XML += "</PACKAGE>";
    }

    public void SetTypeLIST(List<String> list)
    {
        this.XML = "<PACKAGE>";
        this.XML += "<type>LIST</type>";
        int i = 1;
        foreach (String s in list)
        {
            this.XML += "<arg" + i + ">" + s + "</arg" + i + ">";
        }
        this.XML += "</PACKAGE>";
    }

    public void SetTypeBack()
    {
        this.XML = "<PACKAGE>";
        this.XML += "<type>BACK</type>";
        this.XML += "</PACKAGE>";
    }
    public void SetTypeCREATE_LOBBY(String gameType, String lobbyName)
    {
        this.XML = "<PACKAGE>";
        this.XML += "<type>CREATE_GAME</type>";
        this.XML += "<arg1>" + gameType + "</arg1>";
        this.XML += "<arg2>" + lobbyName + "</arg2>";
        this.XML += "</PACKAGE>";
    }
    public void SetTypeJOIN_LOBBY(String lobbyId)
    {
        this.XML = "<PACKAGE>";
        this.XML += "<type>JOIN_LOBBY</type>";
        this.XML += "<arg1>" + lobbyId + "</arg1>";
        this.XML += "</PACKAGE>";
    }
}
}