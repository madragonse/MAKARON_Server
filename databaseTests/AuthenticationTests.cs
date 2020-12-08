using Microsoft.VisualStudio.TestTools.UnitTesting;
using database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace database.Tests
{
    [TestClass()]
    public class AuthenticationTests
    {
        [TestMethod()]
        public void AuthorizeUserTest()
        {
            Authentication auth = new Authentication();
            //user and password exist so no exception should be thrown
            auth.AuthorizeUser("Kret", "e2f0c2aafca651a80fe70ca7159ad93a2915e9a99cf34b1eebd0412aec2e3dac");
            //user and password don't exist- throws exception
            Assert.ThrowsException<AuthenticationException>(() => auth.AuthorizeUser("test", "niema"));
        }

        [TestMethod()]
        public void ConnectToDatabaseTest()
        {
            DatabaseConector d = new DatabaseConector();
            d.ConnectToDatabase();
            d.DisconectFromDatabase();
        }
    }
}