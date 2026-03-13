using System;
using System.Collections.Generic;

namespace Uwu.Data
{
    /// <summary>Intentory factory; limits garbage collection.</summary>
    public class InventoryFactory<T> where T: new()
    {
        #region Static elements (per-type singleton instances, etc.)
        const int DEFAULT_INVENTORY_SIZE = 64;
        static Dictionary<Type, Object> instances = new Dictionary<Type, Object>();

        public static InventoryFactory<T> Instance
        {
            get
            {
                if (!instances.ContainsKey(typeof(T)))
                    instances[typeof(T)] = new InventoryFactory<T>();

                return (InventoryFactory<T>)instances[typeof(T)];
            }
        }
        #endregion

        #region Field(s) & Constructor(s)
        Queue<T> inventory;
        InventoryFactory() { inventory = new Queue<T>(DEFAULT_INVENTORY_SIZE); }
        #endregion

        #region Methods
        public T Request()
        {
            if (inventory.Count == 0)
                return new T();
            else
                return inventory.Dequeue();
        }

        public void Relinquish(T element)
        {
            if(element != null)
                inventory.Enqueue(element);
        }
        #endregion
    }
}
