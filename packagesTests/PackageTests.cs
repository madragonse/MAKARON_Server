using Microsoft.VisualStudio.TestTools.UnitTesting;
using packages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace packages.Tests
{
    [TestClass()]
    public class PackageTests
    {
        [TestMethod()]
        public void getArgumentsTest()
        {
            String data = "<PACKAGE>" +
                "         <TYPE>LOGIN_CONFIRM</TYPE>" +
                "         <arg1>example</arg1>" +
                "         </PACKAGE>";
            Package p = new Communication_Package(Encoding.ASCII.GetBytes(data));

            List<String> arguments = p.getArguments();
            List<String> expectedResult = new List<String>() { "LOGIN_CONFIRM", "example" };
            CollectionAssert.AreEqual(arguments, expectedResult);
        }

    }
}