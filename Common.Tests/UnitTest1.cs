using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common.Serialization;
using System;
using System.Collections.Generic;

namespace Common.Tests
{
    [TestClass]
    public class JsonHelpersTests
    {
        [TestMethod]
        public void Serialize_ValidObject_ReturnsJsonString()
        {
            // Arrange
            var testObject = new TestClass { Id = 1, Name = "Test" };

            // Act
            string json = JsonHelpers.Serialize(testObject);

            // Assert
            Assert.IsNotNull(json);
            Assert.IsTrue(json.Contains("\"Id\": 1"));
            Assert.IsTrue(json.Contains("\"Name\": \"Test\""));
        }

        [TestMethod]
        public void Deserialize_ValidJson_ReturnsObject()
        {
            // Arrange
            string json = "{\"Id\": 1,\"Name\": \"Test\"}";
            TestClass result;

            // Act
            bool success = JsonHelpers.Deserialize(json, out result);

            // Assert
            Assert.IsTrue(success);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Test", result.Name);
        }

        [TestMethod]
        public void Serialize_NullObject_ReturnsNullString()
        {
            // Act
            string json = JsonHelpers.Serialize<TestClass>(null);

            // Assert
            Assert.IsNotNull(json);
            Assert.AreEqual("null", json);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Deserialize_NullJson_ThrowsException()
        {
            // Arrange
            TestClass result;

            // Act
            bool success = JsonHelpers.Deserialize(null, out result);
        }

        private class TestClass
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}