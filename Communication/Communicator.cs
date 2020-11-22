using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using game_lib;


namespace communication
{
    public class Communicator
    {
        private Session session;
        private byte[] buffer;
        public Session Session { get => session; set => session = value; }
        public byte[] Buffer { get => buffer; set => buffer = value; }

        public Communicator(Session s)
        {
            this.session = s;
            this.buffer = new byte[1024];
        }


        private void sendToClient(String m, Session client)
        {
            byte[] message = Encoding.ASCII.GetBytes(m);
            client.Stream.Write(message, 0, message.Length);
        }

        private String getClientsResponse(Session client)
        {
            int messageLength = 0;
            String response = "\r\n";
            while (response == "\r\n")
            {
                messageLength = client.Stream.Read(Buffer, 0, Buffer.Length);
                response = Encoding.UTF8.GetString(Buffer, 0, messageLength);
            }

            return response;
        }

        /// <summary>
        /// Greets given user and allows him to choose whether to log in or sign up.
        /// Returns "s" if user chooses to sign up, and other strings for log in.
        /// </summary>
        /// <param  Client structure="client"></param>
        /// <returns>Returns s if user chooses to sign up, and other strings for log in.</returns>
        public String greetAndChooseOption()
        {
            int messageLenght = 0;

            sendToClient("\nWelcome to the server! Log in or Sign up (s-sign up/anything else-log in)", this.session);
           
            //wait for the response    
            messageLenght = session.Stream.Read(Buffer, 0, Buffer.Length);
            return Encoding.UTF8.GetString(Buffer, 0, messageLenght);
        }



        /// <summary>
        /// Puts given Client in echo state, whatever he writes is repeated back to him.
        /// If the client doesn't respond for 10 seconds, he timesout.
        /// </summary>
        /// <param Client structure="client"></param>
        public void LetPlay(Session client)
        {
            client.Stream.ReadTimeout = 10000;
            game_lib.Game.GameName chosenGame = game_lib.Game.GameName.BOMBERMAN;

            //test game
            GameManager.CreateGame(game_lib.Game.GameName.BOMBERMAN,"TEST LOBBY FOR USER id="+client.Id);
          

            //choose game
            sendToClient("\r\n\nChoose a game!", client);
            printCurrentGameTypes(client);
            String response = getClientsResponse(client);
            Type enumType = chosenGame.GetType();
            chosenGame = (game_lib.Game.GameName) Enum.Parse(enumType, response);
            //choose lobby
            printCurrentLobbies(client, chosenGame);

            sendToClient("Choose a lobby (b-go back/j- join lobby/c- create new/r-refresh)!)", client);
            response = getClientsResponse(client);
            switch (response)
            {
                //join game
                case "j":
                    sendToClient("\n\rChoose lobby id: ", client);
                    response = getClientsResponse(client);
                    if (GameManager.JoinGame(response, client))
                    {
                        //do smth
                        sendToClient("\n\rLobby joined!", client);
                    }
                    break;
                //create game
                case "r":
                    printCurrentLobbies(client, chosenGame);
                    break;
                case "b":
                    //do back stuff
                    break;
                default: break;
            }

            //echo loop
            while (true)
            {
                try
                {
                    int messageLenght = client.Stream.Read(Buffer, 0, Buffer.Length);
                    client.Stream.Write(Buffer, 0, messageLenght);
                }
                catch (System.IO.IOException) { Console.Write("\rClient " + client.Id + " has disconected!"); break; }
            }

        }

        private void printCurrentLobbies(Session client, game_lib.Game.GameName name)
        { 
            sendToClient("\n----CURRENT LOBBIES----", client);
            List<Game> currGames = GameManager.GetAllGames();
            foreach (Game g in currGames)
            {
                if (g.getGameType()==name)
                {
                    sendToClient(g.ToString() + "\n", client);
                }
               
            }
        }

        private void printCurrentGameTypes(Session client)
        {
            sendToClient("\n----CURRENT GAMES----", client);
            var values = Enum.GetValues(typeof(game_lib.Game.GameName));
            int i = 0;
            foreach (var v in values)
            {
                sendToClient("\r\n"+i+". "+v.ToString() + "\n", client);
                i++;
            }
        }


        /// <summary>
        /// Allows user to log into an existing account.
        /// </summary>
        /// <param  Client structure="client"></param>
        public void LogIn(Session client)
        {
            Authentication auth = new Authentication();
            int messageLenght = 0;
            byte[] message;
            String username = "\r\n";
            String password = "\r\n";


            message = Encoding.ASCII.GetBytes("\r\nUsername: ");
            client.Stream.Write(message, 0, message.Length);
            //in order to avoid \r\n randomly sent by Putty being taken as an input
            while (username == "\r\n")
            {
                messageLenght = client.Stream.Read(Buffer, 0, Buffer.Length);
                username = Encoding.UTF8.GetString(Buffer, 0, messageLenght);
            }

            message = Encoding.ASCII.GetBytes("\rPassword: ");
            client.Stream.Write(message, 0, message.Length);
            //in order to avoid \r\n randomly sent by Putty being taken as an input
            while (password == "\r\n")
            {
                messageLenght = client.Stream.Read(Buffer, 0, Buffer.Length);
                password = Encoding.UTF8.GetString(Buffer, 0, messageLenght); 
            }

            try
            {
                auth.AuthorizeUser(username, password);
                Console.WriteLine("\nA user loged in [username:" + username + "]");
                message = Encoding.ASCII.GetBytes("\n\rLog in succesfull!");
                client.Stream.Write(message, 0, message.Length);
            }
            catch (AuthenticationException e)
            {
                if (e.ErrorCategory == -1)
                {
                    message = Encoding.ASCII.GetBytes("Server malfunction: " + e);
                    client.Stream.Write(message, 0, message.Length);
                    return;
                }
                if (e.ErrorCategory == 1)
                {
                    message = Encoding.ASCII.GetBytes("Error: " + e);
                    client.Stream.Write(message, 0, message.Length);
                    message = Encoding.ASCII.GetBytes("\n\r--Try again!--\n\r");
                    client.Stream.Write(message, 0, message.Length);
                    LogIn(client);
                }
            }
        }

        /// <summary>
        /// Allows user to create a new account.
        /// </summary>
        /// <param  Client structure="client"></param>
        public void SignUp(Session client)
        {
            Authentication auth = new Authentication();
            int messageLenght = 0;
            String password = "\r\n";
            String confpassword = "\r\n";
            String username = "\r\n";

            byte[] message = Encoding.ASCII.GetBytes("\rUsername (max 10 chars): ");
            client.Stream.Write(message, 0, message.Length);
            //in order to avoid \r\n randomly sent by Putty being taken as an input
            while (username == "\r\n")
            {
                messageLenght = client.Stream.Read(Buffer, 0, Buffer.Length);
                username = Encoding.UTF8.GetString(Buffer, 0, messageLenght);
            }


            message = Encoding.ASCII.GetBytes("\rPassword (one upercase Letter and number required): ");
            client.Stream.Write(message, 0, message.Length);
            //in order to avoid \r\n randomly sent by Putty being taken as an input
            while (password == "\r\n")
            {
                messageLenght = client.Stream.Read(Buffer, 0, Buffer.Length);
                password = Encoding.UTF8.GetString(Buffer, 0, messageLenght); ;
            }

            message = Encoding.ASCII.GetBytes("\rConfirm Password: ");
            client.Stream.Write(message, 0, message.Length);
            //in order to avoid \r\n randomly sent by Putty being taken as an input
            while (confpassword == "\r\n")
            {
                messageLenght = client.Stream.Read(Buffer, 0, Buffer.Length);
                confpassword = Encoding.UTF8.GetString(Buffer, 0, messageLenght); ;
            }

            //Check if the two passwords are the same
            if (password != confpassword)
            {
                message = Encoding.ASCII.GetBytes("\r\nError: Passwords don't match!");
                client.Stream.Write(message, 0, message.Length);
                SignUp(client);
            }


            try
            {
                auth.CreateUser(username, password);
                Console.WriteLine("\nNew account created [user:" + username + " password: " + password + "]");
                message = Encoding.ASCII.GetBytes("\n\rAccount created succesfully!");
                client.Stream.Write(message, 0, message.Length);
            }
            catch (AuthenticationException e)
            {
                if (e.ErrorCategory == -1)
                {
                    message = Encoding.ASCII.GetBytes("Server malfunction: " + e);
                    client.Stream.Write(message, 0, message.Length);
                    return;
                }
                if (e.ErrorCategory == 1)
                {
                    message = Encoding.ASCII.GetBytes("Error: " + e);
                    client.Stream.Write(message, 0, message.Length);
                    message = Encoding.ASCII.GetBytes("\n\r--Try again!--\n\r");
                    client.Stream.Write(message, 0, message.Length);
                    SignUp(client);
                }
            }
        }
    

    }
}

