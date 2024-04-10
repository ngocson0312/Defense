using System.Collections;
using System.Collections.Generic;
using UDEV.GhostDefense;
using UnityEngine;
using UnityEngine.UI;

namespace UDEV.GhostDefense
{
    public class NoAdsBtn : MonoBehaviour
    {
        [SerializeField] private Text m_priceTxt;

        private void Start()
        {
            if (m_priceTxt && IAPManager.Ins)
            {
                IAPItem iapItem = new IAPItem();
                iapItem.id = IAPManager.Ins.noadsId;
                iapItem.localPrice = IAPManager.Ins.noadsLocalPrice;

                m_priceTxt.text = IAPManager.Ins.GetPriceString(iapItem);
            }
        }

        public void Purchase()
        {
            IAPManager.Ins?.MakeItemBuying(IAPManager.Ins.noadsId);
        }
    }
}
