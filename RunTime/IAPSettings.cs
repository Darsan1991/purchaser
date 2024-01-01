using System;
using System.Collections.Generic;
using System.Linq;
using DGames.Essentials.Attributes;
using DGames.Essentials.Unity;
using UnityEngine;

namespace DGames.Purchasing
{
    [DashboardResourceItem(path:"Settings")]
    [HideScriptField]
    public partial class IAPSettings : ScriptableObject
    {
        [ScriptableSymbolsToggle("","IN_APP",BuildTargetGroup.Android | BuildTargetGroup.iOS)][SerializeField] private bool _iapActive;
        [Inline()][SerializeField] private List<IAPProduct> _products = new();
        
        public bool IAPActive => _iapActive;

        public IEnumerable<IAPProduct> Products => _products.Where(p=>p.active);
    }
    
    public partial class IAPSettings
    {
        public static IAPSettings Default => Resources.Load<IAPSettings>(nameof(IAPSettings));
        
                
#if UNITY_EDITOR
        [UnityEditor.MenuItem("MyGames/Settings/IAPSettings")]
        public static void Open()
        {
            ScriptableEditorUtils.OpenOrCreateDefault<IAPSettings>();
        }
#endif
    }

    public partial class IAPSettings
    {
        public const string ACTIVE_FIELD = nameof(_iapActive);
        public const string PRODUCTS = nameof(_products);
    }

    [Serializable]
    public struct IAPProduct
    {
        public bool active;
        public string name;
        public string productId;
        public bool nonConsumable;
    }
}
