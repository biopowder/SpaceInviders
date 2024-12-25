using System;
using Homework;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public sealed class ResourceZoneTests
    {
        [Test]
        public void Constructor_ValidCapacity_SetsCurrentAmountToZero()
        {
            // Arrange & Act
            ResourceZone zone = new(10);

            // Assert
            Assert.AreEqual(0, zone.CurrentAmount,
                "After creating ResourceZone with a positive capacity, CurrentAmount should be 0.");
        }

        [Test]
        public void Constructor_NegativeCapacity_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new ResourceZone(-5),
                "Creating ResourceZone with a negative capacity should throw an exception.");
        }

        [Test]
        public void AddResource_WhenAmountIsZero_NoChanges()
        {
            // Arrange
            ResourceZone zone = new(5);

            // Act
            int leftover = zone.AddResource(0);

            // Assert
            Assert.AreEqual(0, leftover,
                "Adding 0 resources should return 0 leftover.");
            Assert.AreEqual(0, zone.CurrentAmount,
                "Adding 0 resources should keep CurrentAmount unchanged at 0.");
        }

        [Test]
        public void AddResource_WhenAmountIsNegative_ThrowsArgumentException()
        {
            // Arrange
            ResourceZone zone = new(5);

            // Assert
            Assert.Throws<ArgumentException>(() => zone.AddResource(-10),
                "Adding a negative amount of resources should throw an exception.");
        }

        [Test]
        public void AddResource_LessThanFreeSpace_AddsCorrectly()
        {
            // Arrange
            ResourceZone zone = new(5);

            // Act
            int leftover = zone.AddResource(3);

            // Assert
            Assert.AreEqual(0, leftover,
                "If we add 3 to a capacity of 5, there should be no leftover.");
            Assert.AreEqual(3, zone.CurrentAmount,
                "After adding 3 resources, the current amount should be 3.");
        }

        [Test]
        public void AddResource_MoreThanFreeSpace_ReturnsExcess()
        {
            // Arrange
            ResourceZone zone = new(5);

            // Act
            int leftover = zone.AddResource(7);

            // Assert
            Assert.AreEqual(2, leftover,
                "When adding 7 to a zone with capacity 5, 2 should be leftover (excess).");
            Assert.AreEqual(5, zone.CurrentAmount,
                "CurrentAmount should not exceed capacity, so it should be 5.");
        }

        [Test]
        public void RemoveResource_WhenAmountIsZero_NoChanges()
        {
            // Arrange
            ResourceZone zone = new(5);
            zone.AddResource(3);

            // Act
            zone.RemoveResource(0);

            // Assert
            Assert.AreEqual(3, zone.CurrentAmount,
                "Removing 0 resources should not change the current amount.");
        }

        [Test]
        public void RemoveResource_WhenAmountIsNegative_ThrowsArgumentException()
        {
            // Arrange
            ResourceZone zone = new(5);

            // Act
            zone.AddResource(3);

            Assert.Throws<ArgumentException>(() => zone.RemoveResource(-1),
                "Removing a negative number of resources should throw an exception.");
        }

        [Test]
        public void RemoveResource_MoreThanCurrent_RemovesAll()
        {
            // Arrange
            ResourceZone zone = new(5);
            zone.AddResource(3);

            // Act
            zone.RemoveResource(10);

            // Assert
            Assert.AreEqual(0, zone.CurrentAmount,
                "If asked to remove more than available, the zone should end up empty (0).");
        }
    }
}