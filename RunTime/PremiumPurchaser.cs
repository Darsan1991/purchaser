using System;
using System.Linq;
using DGames.ObjectEssentials.Scriptable;
using UnityEngine;
#if IN_APP
using UnityEngine.Purchasing;
#endif

namespace DGames.Purchasing
{
    public partial class PremiumPurchaser : MonoBehaviour
    {
        [SerializeField] private string _name = "NO_ADS";
        [SerializeField] private ValueField<bool> _premium = new("PREMIUM");
    }
        
    #if IN_APP
    public partial class PremiumPurchaser: INonConsumableItemPurchaser
    {
        public event Action<INonConsumableItemPurchaser, bool> ItemPurchased;
        
       

        private IPurchaser _purchaser;
        private string _id;
        public string Id => _id ??= IAPSettings.Default.Products.First(p => p.name == _name).productId;
        public bool Initialized => _purchaser.Initialized;

        public IPurchaser Purchaser => _purchaser ??= GetComponent<IPurchaser>();

        public string GetPrice()
        {
            return Purchaser.GetPrice(Id);
        }

        public void Buy(Action<bool> callback)
        {

            if (ItemAlreadyPurchased())
                throw new InvalidOperationException();
            
            Purchaser.BuyProduct(Id, success =>
            {
                if(success && _premium)
                    _premium.Value.Set(true);
                
                ItemPurchased?.Invoke(this, success);
                callback?.Invoke(success);
            });
        }

        public bool ItemAlreadyPurchased()
        {
            return Purchaser.ItemAlreadyPurchased(Id);
        }
        
        public Product Get()
        {
            return Purchaser.GetProduct(Id);
        }
        
    }
#endif
}
