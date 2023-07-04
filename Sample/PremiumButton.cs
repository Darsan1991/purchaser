using DGames.ObjectEssentials.Scriptable;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DGames.Purchasing.Samples
{
    public class PremiumButton : MonoBehaviour, IPointerClickHandler
    {

        [SerializeField] private ValueField<bool> _premiumValueField = new("PREMIUM");
#if IN_APP
        public PremiumPurchaser Purchaser => DGames.Purchasing.Purchaser.Instance.Get<PremiumPurchaser>();
#endif
        
        private void Awake()
        {
#if IN_APP
            gameObject.SetActive(!_premiumValueField);
#else
        gameObject.SetActive(false);
#endif
        }
#if IN_APP
        private void OnEnable()
        {
            if (WarningIfNoPurchaser())
            {
                return;
            }

            Purchaser.ItemPurchased += PurchaserOnItemPurchased;
        }

        private bool WarningIfNoPurchaser()
        {
            if(Purchaser==null)
            {
                Debug.LogWarning("No Premium Purchaser Found!");
                return true;
            }

            return false;
        }



        private void OnDisable()
        {
            if (WarningIfNoPurchaser())
            {
                return;
            }

            Purchaser.ItemPurchased -= PurchaserOnItemPurchased;
        }

        private void PurchaserOnItemPurchased(INonConsumableItemPurchaser nonConsumableItemPurchaser, bool b)
        {
            gameObject.SetActive(_premiumValueField);
        }
#endif
        
        public void OnPointerClick(PointerEventData eventData)
        {
#if IN_APP
            if (WarningIfNoPurchaser())
            {
                return;
            }

            Purchaser.Buy(_ => { });
#endif
        }

    }
}