
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if IN_APP

using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
#endif


namespace DGames.Purchasing
{
    public partial class Purchaser : Singleton<Purchaser>
    {
        
    }

    #if IN_APP
    public partial class Purchaser:IDetailedStoreListener, IPurchaser
    {
        private static IStoreController _mStoreController; // The Unity Purchasing system.
        private static IExtensionProvider _mStoreExtensionProvider; // The store-specific Purchasing subsystems.
        
        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.Log("OnInilized Failed");
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Debug.Log("OnInilized Failed");
        }


        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            Debug.Log("Purchased Product:" + args.purchasedProduct.definition.id);
            ItemPurchased?.Invoke(args.purchasedProduct.definition.id, true);
            return PurchaseProcessingResult.Complete;
        }


        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) =>
            ItemPurchased?.Invoke(product.definition.id, false);

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            ItemPurchased?.Invoke(product.definition.id, false);
        }
        
        

        public Product GetProduct(string id)
        {
            return _mStoreController?.products.WithID(id);
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _mStoreController = controller;
            _mStoreExtensionProvider = extensions;
        }


        public void Restore()
        {
            _mStoreExtensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions((success, _) =>
            {
                RestorePurchased?.Invoke(success);
            });
        }
        
        /// <summary>
        /// The Item Already Brought for Non consumable Products
        /// </summary>
        /// <param name="productId">product id</param>
        /// <returns></returns>
        public bool ItemAlreadyPurchased(string productId)
        {
            var product = GetProduct(productId);
            if (product != null && product.definition.type != ProductType.Consumable && product.hasReceipt)
            {
                return true;
            }

            return false;
        }
        
        
        private void Init()
        {
            //TODO:UnComment After InApp Plugin added
            if (Initialized)
            {
                return;
            }

            if (_mStoreController == null)
            {
                var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
                foreach (var premiumItem in ConsumableItems)
                {
                    builder.AddProduct(premiumItem, ProductType.Consumable);
                }

                foreach (var nonConsumableItem in NonConsumableItems)
                {
                    builder.AddProduct(nonConsumableItem, ProductType.NonConsumable);
                }

                UnityPurchasing.Initialize(this, builder);

            }
        }

        /// <summary>
        /// Item Purchase Completed with id and success state
        /// </summary>
        public event Action<string, bool> ItemPurchased;

        public event Action<bool> RestorePurchased;

        public bool Initialized => _mStoreController != null && _mStoreExtensionProvider != null;

        public IEnumerable<string> ConsumableItems => _consumableItems;

        public IEnumerable<string> NonConsumableItems => _nonConsumableItems;


       
        private readonly List<string> _consumableItems = new();
        private readonly List<string> _nonConsumableItems = new();


        protected override void OnAwake()
        {
            base.OnAwake();
            _nonConsumableItems.AddRange(IAPSettings.Default.Products.Where(p=>p.nonConsumable).Select(p=>p.productId));
            _consumableItems.AddRange(IAPSettings.Default.Products.Where(p=>!p.nonConsumable).Select(p=>p.productId));
            Init();
        }


        public string GetPrice(string productId) => GetProduct(productId)?.metadata.localizedPriceString;

        /// <summary>
        /// Buy the product with item
        /// </summary>
        /// <param name="productId">Product Id</param>
        /// <param name="callback">Buy product call back with success state</param>
        public void BuyProduct(string productId, Action<bool> callback)
        {
            void OnPurchase(string id, bool success)
            {
                if (id != productId) return;
                ItemPurchased -= OnPurchase;
                callback?.Invoke(success);
            }

            ItemPurchased += OnPurchase;

            BuyProduct(productId);
        }




        public void BuyProduct(string productId)
        {
            // If Purchasing has been initialized ...
            if (Initialized)
            {
                var product = _mStoreController.products.WithID(productId);

                if (product != null && product.availableToPurchase)
                {
                    _mStoreController.InitiatePurchase(product);
                }
                else
                {
                    ItemPurchased?.Invoke(productId, false);
                }
            }
            else
            {
                ItemPurchased?.Invoke(productId, false);
                Init();
            }
        }



    }
    #endif
}
