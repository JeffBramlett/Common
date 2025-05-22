using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common.Serialization;
using System;
using Newtonsoft.Json;

namespace Common.Tests
{
    [TestClass]
    public class SerializeNullTest
    {
        [TestMethod]
        public void TestSerializeNull()
        {
            // This test is to determine what JsonHelpers.Serialize actually returns for null
            string result = JsonHelpers.Serialize<object>(null);
            Console.WriteLine($"Result: '{result}'");
            
            // Let's also check what Newtonsoft.Json does directly
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };
            string directResult = JsonConvert.SerializeObject(null, settings);
            Console.WriteLine($"Direct result: '{directResult}'");
        }
    }
}