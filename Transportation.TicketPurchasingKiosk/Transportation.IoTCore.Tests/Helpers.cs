using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Transportation.IoTCore.Tests
{
    [TestClass]
    public class Helpers
    {
        [TestMethod]
        public void GetBytes_ValidInput_ProducesValidOutput()
        {
            // Arrange
            var test = "test string";

            // Act
            var answer = test.GetBytes();

            // Assert
            var reference = new byte[22] { 116, 0, 101, 0, 115, 0, 116, 0, 32, 0, 115, 0, 116, 0, 114, 0, 105, 0, 110, 0, 103, 0 };
            
            Assert.IsTrue(reference.SequenceEqual(answer));
        }
    }
}
