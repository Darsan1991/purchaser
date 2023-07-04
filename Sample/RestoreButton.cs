using UnityEngine;
using UnityEngine.EventSystems;

namespace DGames.Purchasing.Samples
{
    public class RestoreButton : MonoBehaviour, IPointerClickHandler
    {

        #if IN_APP
        public IPurchaser Purchaser => DGames.Purchasing.Purchaser.Instance;
        #endif
        
        private void Awake()
        {
            gameObject.SetActive(Application.platform == RuntimePlatform.IPhonePlayer);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
#if IN_APP
        Purchaser.Restore();
#endif
        }
    }
}