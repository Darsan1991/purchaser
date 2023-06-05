#if IN_APP
using System;
using System.Collections.Generic;
using UnityEngine.Purchasing;

namespace DGames.Purchasing
{
    public interface IPurchaser
    {
        /// <summary>
        /// Item Purchase Completed with id and success state
        /// </summary>
        event Action<string, bool> ItemPurchased;

        event Action<bool> RestorePurchased;
        bool Initialized { get; }
        IEnumerable<string> ConsumableItems { get; }
        IEnumerable<string> NonConsumableItems { get; }
        string GetPrice(string productId);

        /// <summary>
        /// Buy the product with item
        /// </summary>
        /// <param name="productId">Product Id</param>
        /// <param name="callback">Buy product call back with success state</param>
        void BuyProduct(string productId, Action<bool> callback);

        /// <summary>
        /// The Item Already Brought for Non consumable Products
        /// </summary>
        /// <param name="productId">product id</param>
        /// <returns></returns>
        bool ItemAlreadyPurchased(string productId);

        void BuyProduct(string productId);
        void Restore();
        Product GetProduct(string id);
    }
}
#endif