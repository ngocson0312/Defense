using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UDEV.GhostDefense
{
    public class IAPItemUI : MonoBehaviour
    {
        [SerializeField] private Text m_priceText;
        [SerializeField] private Text m_amountText;
        public Button buyBtn;

        public void UpdateUI(IAPItem item)
        {
            if (m_priceText)
                m_priceText.text = IAPManager.Ins.GetPriceString(item);

            if (m_amountText)
                m_amountText.text = item.amount.ToString("n0");
        }
    }
}
