using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Common.Tests
{
    [TestClass]
    public class ListExtensionsTests
    {
        [TestMethod]
        public async Task ApplyAction_ProcessesAllItems()
        {
            // Arrange
            var list = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var processedItems = new List<int>();
            
            // Act
            bool result = await list.ApplyAction(item => 
            {
                processedItems.Add(item);
            }, 3);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(list.Count, processedItems.Count);
            CollectionAssert.AreEquivalent(list, processedItems);
        }

        [TestMethod]
        public async Task ApplyFunction_ReturnsProcessedItems()
        {
            // Arrange
            var list = new List<int> { 1, 2, 3, 4, 5 };
            
            // Act
            var result = await list.ApplyFunction(item => item * 2, 2);

            // Assert
            Assert.AreEqual(list.Count, result.Count);
            for (int i = 0; i < list.Count; i++)
            {
                Assert.AreEqual(list[i] * 2, result[i]);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ApplyAction_NullCollection_ThrowsException()
        {
            // Act
            await ((IList<int>)null).ApplyAction(item => { }, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ApplyAction_NullAction_ThrowsException()
        {
            // Arrange
            var list = new List<int> { 1, 2, 3 };
            
            // Act
            await list.ApplyAction<int>(null, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ApplyAction_ZeroThrottleSize_ThrowsException()
        {
            // Arrange
            var list = new List<int> { 1, 2, 3 };
            
            // Act
            await list.ApplyAction(item => { }, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ApplyFunction_NullCollection_ThrowsException()
        {
            // Act
            await ((IList<int>)null).ApplyFunction<int, int>(item => item, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ApplyFunction_NullFunction_ThrowsException()
        {
            // Arrange
            var list = new List<int> { 1, 2, 3 };
            
            // Act
            await list.ApplyFunction<int, int>(null, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ApplyFunction_ZeroThrottleSize_ThrowsException()
        {
            // Arrange
            var list = new List<int> { 1, 2, 3 };
            
            // Act
            await list.ApplyFunction(item => item, 0);
        }
    }
}