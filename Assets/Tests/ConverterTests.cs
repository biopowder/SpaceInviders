using Homework;
using NUnit.Framework;

namespace Tests
{
    public sealed class ConverterTests
    {
        [Test]
        public void NotEnoughResources_StopsImmediately()
        {
            // Arrange
            ResourceZone loadingZone = new(capacity: 5);
            ResourceZone unloadingZone = new(capacity: 10);
            loadingZone.AddResource(2);

            Converter converter = new(
                loadingZone,
                unloadingZone,
                amountToTake: 3,
                amountToDeliver: 1,
                conversionTime: 1f
            );

            // Act
            converter.ToggleConverter(true);
            converter.Update(1f);
            
            Assert.IsTrue(loadingZone.CurrentAmount == 2,
                "After the first tick of 1s, we have already taken 2 resources.");
            Assert.IsTrue(unloadingZone.CurrentAmount == 0, "Not enough time has elapsed for unloading.");
            
            converter.Update(1f);
            converter.ToggleConverter(false);

            // Assert
            Assert.AreEqual(2, loadingZone.CurrentAmount,
                "Resources should not have been taken as there are fewer than required.");
            Assert.AreEqual(0, unloadingZone.CurrentAmount,
                "No resources should have appeared in the unloading zone.");
        }

        [Test]
        public void EnoughResources_OneCycle_ResourcesTransferred()
        {
            // Arrange
            ResourceZone loadingZone = new(capacity: 5);
            ResourceZone unloadingZone = new(capacity: 5);
            loadingZone.AddResource(5);

            Converter converter = new(
                loadingZone,
                unloadingZone,
                amountToTake: 2,
                amountToDeliver: 2,
                conversionTime: 2f
            );

            // Act
            converter.ToggleConverter(true);
            converter.Update(1f);

            Assert.IsTrue(loadingZone.CurrentAmount == 3,
                "After the first tick of 1s, we have already taken 2 resources.");
            Assert.IsTrue(unloadingZone.CurrentAmount == 0, "Not enough time has elapsed for unloading.");

            converter.Update(1f);
            converter.ToggleConverter(false);

            // Assert
            Assert.AreEqual(3, loadingZone.CurrentAmount,
                "Only 2 resources should have been taken from loadingZone (5 - 2 = 3).");
            Assert.AreEqual(2, unloadingZone.CurrentAmount, "2 resources should have been added to the unloadingZone.");
        }

        [Test]
        public void ToggleOffDuringConversion_ResourcesReturnToLoading()
        {
            // Arrange
            ResourceZone loadingZone = new(capacity: 10);
            ResourceZone unloadingZone = new(capacity: 10);
            loadingZone.AddResource(10);

            Converter converter = new(
                loadingZone,
                unloadingZone,
                amountToTake: 5,
                amountToDeliver: 3,
                conversionTime: 2f
            );

            // Act
            converter.ToggleConverter(true);
            converter.Update(1f);
            converter.ToggleConverter(false);

            // Assert
            Assert.AreEqual(10, loadingZone.CurrentAmount,
                "All 5 resources should have returned to the loadingZone.");
            Assert.AreEqual(0, unloadingZone.CurrentAmount,
                "No resources should have reached the unloadingZone, as the cycle did not complete.");
        }

        [Test]
        public void UnloadingZoneOverflow_LeftoverBurns()
        {
            // Arrange
            ResourceZone loadingZone = new(capacity: 10);
            ResourceZone unloadingZone = new(capacity: 1);

            loadingZone.AddResource(10);

            Converter converter = new(
                loadingZone,
                unloadingZone,
                amountToTake: 2,
                amountToDeliver: 2,
                conversionTime: 1f
            );

            // Act
            converter.ToggleConverter(true);
            converter.Update(1f);
            converter.ToggleConverter(false);

            // Assert
            Assert.AreEqual(8, loadingZone.CurrentAmount,
                "8 resources should remain in the loadingZone (10 - 2, as they were taken).");
            Assert.AreEqual(1, unloadingZone.CurrentAmount,
                "Only 1 resource could fit in the unloadingZone, the other one burned.");
        }
    }
}