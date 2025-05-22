using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common.Serialization;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Common.Tests
{
    [TestClass]
    public class JsonUtilsTests
    {
        [TestMethod]
        public void GetRootToken_ValidJson_ReturnsJToken()
        {
            // Arrange
            string json = "{\"Person\":{\"Name\":\"John\",\"Age\":30}}";

            // Act
            JToken token = JsonUtils.GetRootToken(json);

            // Assert
            Assert.IsNotNull(token);
            Assert.AreEqual(JTokenType.Object, token.Type);
        }

        [TestMethod]
        public void GetTokenValue_StringValue_ReturnsString()
        {
            // Arrange
            string json = "{\"Person\":{\"Name\":\"John\",\"Age\":30}}";
            JToken token = JsonUtils.GetRootToken(json);

            // Act
            string name = JsonUtils.GetTokenValue(token, "Person.Name");

            // Assert
            Assert.AreEqual("John", name);
        }

        [TestMethod]
        public void GetTokenValue_InvalidPath_ReturnsNull()
        {
            // Arrange
            string json = "{\"Person\":{\"Name\":\"John\",\"Age\":30}}";
            JToken token = JsonUtils.GetRootToken(json);

            // Act
            string value = JsonUtils.GetTokenValue(token, "Person.InvalidProperty");

            // Assert
            Assert.IsNull(value);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void GetTokenValue_NullToken_ThrowsException()
        {
            // Act
            string value = JsonUtils.GetTokenValue(null, "Person.Name");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetTokenValue_NullPath_ThrowsException()
        {
            // Arrange
            string json = "{\"Person\":{\"Name\":\"John\",\"Age\":30}}";
            JToken token = JsonUtils.GetRootToken(json);

            // Act
            string value = JsonUtils.GetTokenValue(token, null);
        }

        [TestMethod]
        public void GetTokenValue_GenericType_ReturnsTypedValue()
        {
            // Arrange
            string json = "{\"Person\":{\"Name\":\"John\",\"Age\":30}}";
            JToken token = JsonUtils.GetRootToken(json);

            // Act
            int age = JsonUtils.GetTokenValue<int>(token, "Person.Age");

            // Assert
            Assert.AreEqual(30, age);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void GetTokenValue_Generic_NullToken_ThrowsException()
        {
            // Act
            int value = JsonUtils.GetTokenValue<int>(null, "Person.Age");
        }

        [TestMethod]
        public void GetTokenJson_ValidPath_ReturnsJsonString()
        {
            // Arrange
            string json = "{\"Person\":{\"Name\":\"John\",\"Age\":30}}";
            JToken token = JsonUtils.GetRootToken(json);

            // Act
            string personJson = JsonUtils.GetTokenJson(token, "Person");

            // Assert
            Assert.IsNotNull(personJson);
            Assert.IsTrue(personJson.Contains("\"Name\":\"John\""));
            Assert.IsTrue(personJson.Contains("\"Age\":30"));
        }

        [TestMethod]
        public void GetTokenJson_InvalidPath_ReturnsNull()
        {
            // Arrange
            string json = "{\"Person\":{\"Name\":\"John\",\"Age\":30}}";
            JToken token = JsonUtils.GetRootToken(json);

            // Act
            string value = JsonUtils.GetTokenJson(token, "InvalidPath");

            // Assert
            Assert.IsNull(value);
        }
    }
}