﻿using game_lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server_lib
{
    public class Communicator
    {
        private Server server;
        public Server Server { get => server; set => server = value; }

        public Communicator(Server s)
        {
            this.server = s;
        }
        private void sendToClient(String m, Session client)
        {
            byte[] message = Encoding.ASCII.GetBytes(m);
            client.Stream.Write(message, 0, message.Length);
        }

        /// <summary>
        /// Greets given user and allows him to choose whether to log in or sign up.
        /// Returns "s" if user chooses to sign up, and other strings for log in.
        /// </summary>
        /// <param  Client structure="client"></param>
        /// <returns>Returns s if user chooses to sign up, and other strings for log in.</returns>
        public String greetAndChooseOption(Session client)
        {
            int messageLenght = 0;

            sendToClient("\nWelcome to the server! Log in or Sign up (s-sign up/anything else-log in)", client);
           
            //wait for the response    
            messageLenght = client.Stream.Read(server.Buffers[client.Id], 0, server.Buffer_size);
            return Encoding.UTF8.GetString(server.Buffers[client.Id], 0, messageLenght);
        }



        /// <summary>
        /// Puts given Client in echo state, whatever he writes is repeated back to him.
        /// If the client doesn't respond for 10 seconds, he timesout.
        /// </summary>
        /// <param Client structure="client"></param>
        public void LetPlay(Session client)
        {
            client.Stream.ReadTimeout = 10000;

            //test games
            GameManager.CreateGame(GameManager.GameName.BOMBERMAN, "TESTOWY POKOJ 1");
            GameManager.CreateGame(GameManager.GameName.BOMBERMAN, "TESTOWY POKOJ 2");

            //list all currgames
            printCurrentGames(client);

            sendToClient("\n\rCreate a new game or join an existing one! (j- join/c-create)", client);
            String response = getClientsResponse(client);

   
            switch (response)
            {

                //join game
                case "j":

                    break;
                //create game
                case "c":

                    break;
                default: break;
            }

            //echo loop
            while (true)
            {
                try
                {
                    int messageLenght = client.Stream.Read(server.Buffers[client.Id], 0, server.Buffer_size);
                    client.Stream.Write(server.Buffers[client.Id], 0, messageLenght);
                }
                catch (System.IO.IOException) { Console.Write("\rClient " + client.Id + " has disconected!"); break; }
            }

        }

        private void printCurrentGames(Session client)
        { 
            sendToClient("\n----CURRENT GAMES----", client);
            List<Game> currGames = GameManager.GetAllGames();
            foreach (Game g in currGames)
            {
                sendToClient(g.ToString()+"\n", client);
            }
        }

        private String getClientsResponse(Session client)
        {
            int messageLength = client.Stream.Read(server.Buffers[client.Id], 0, server.Buffer_size);
            return Encoding.UTF8.GetString(server.Buffers[client.Id], 0, messageLength);
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
                messageLenght = client.Stream.Read(server.Buffers[client.Id], 0, server.Buffer_size);
                username = Encoding.UTF8.GetString(server.Buffers[client.Id], 0, messageLenght);
            }

            message = Encoding.ASCII.GetBytes("\rPassword: ");
            client.Stream.Write(message, 0, message.Length);
            //in order to avoid \r\n randomly sent by Putty being taken as an input
            while (password == "\r\n")
            {
                messageLenght = client.Stream.Read(server.Buffers[client.Id], 0, server.Buffer_size);
                password = Encoding.UTF8.GetString(server.Buffers[client.Id], 0, messageLenght); 
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
                messageLenght = client.Stream.Read(server.Buffers[client.Id], 0, server.Buffer_size);
                username = Encoding.UTF8.GetString(server.Buffers[client.Id], 0, messageLenght);
            }


            message = Encoding.ASCII.GetBytes("\rPassword (one upercase Letter and number required): ");
            client.Stream.Write(message, 0, message.Length);
            //in order to avoid \r\n randomly sent by Putty being taken as an input
            while (password == "\r\n")
            {
                messageLenght = client.Stream.Read(server.Buffers[client.Id], 0, server.Buffer_size);
                password = Encoding.UTF8.GetString(server.Buffers[client.Id], 0, messageLenght); ;
            }

            message = Encoding.ASCII.GetBytes("\rConfirm Password: ");
            client.Stream.Write(message, 0, message.Length);
            //in order to avoid \r\n randomly sent by Putty being taken as an input
            while (confpassword == "\r\n")
            {
                messageLenght = client.Stream.Read(server.Buffers[client.Id], 0, server.Buffer_size);
                confpassword = Encoding.UTF8.GetString(server.Buffers[client.Id], 0, messageLenght); ;
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

