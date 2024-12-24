using System;

namespace Homework
{
    public class ResourceZone
    {
        public int CurrentAmount { get; private set; }
        private int Capacity { get; }

        public ResourceZone(int capacity)
        {
            Capacity = capacity;
            CurrentAmount = 0;
        }

        public int AddResource(int amount)
        {
            int freeSpace = Capacity - CurrentAmount;
            int amountToAdd = Math.Min(freeSpace, amount);
            CurrentAmount += amountToAdd;
            return amount - amountToAdd;
        }

        public void RemoveResource(int amount)
        {
            int amountToRemove = Math.Min(CurrentAmount, amount);
            CurrentAmount -= amountToRemove;
        }
    }
}