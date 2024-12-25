using System;

namespace Homework
{
    public class ResourceZone
    {
        public int CurrentAmount { get; private set; }
        private int Capacity { get; }

        public ResourceZone(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentException("Capacity cannot be negative.", nameof(capacity));
            
            Capacity = capacity;
            CurrentAmount = 0;
        }

        public int AddResource(int amount)
        {
            if (amount < 0)
                throw new ArgumentException("Cannot add negative amount.", nameof(amount));
            
            int freeSpace = Capacity - CurrentAmount;
            int amountToAdd = Math.Min(freeSpace, amount);
            CurrentAmount += amountToAdd;
            return amount - amountToAdd;
        }

        public void RemoveResource(int amount)
        {
            if (amount < 0)
                throw new ArgumentException("Cannot remove negative amount.", nameof(amount));
            
            int amountToRemove = Math.Min(CurrentAmount, amount);
            CurrentAmount -= amountToRemove;
        }
    }
}