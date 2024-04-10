using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UDEV.GhostDefense
{
    public class IAPDialog : Dialog
    {
        [SerializeField] private Transform m_layoutRoot;
        [SerializeField] private IAPItemUI m_itemPrefab;
        List<IAPItem> m_items;

        public override void Show(bool isShow)
        {
            base.Show(isShow);

            m_items = IAPManager.Ins.items;

            UpdateUI();

            //FirebaseController.Ins.AddLogEvent(FBLogEvent.iap_shop_open.ToString());
        }

        public void UpdateUI()
        {
            //Helper.ClearChilds(layoutRoot);

            if (m_items != null && m_items.Count > 0)
            {
                for (int i = 0; i < m_items.Count; i++)
                {
                    int idx = i;

                    IAPItem item = m_items[idx];

                    if (item != null && m_layoutRoot && m_itemPrefab)
                    {
                        IAPItemUI itemUIClone = Instantiate(m_itemPrefab, Vector3.zero, Quaternion.identity);

                        itemUIClone.transform.SetParent(m_layoutRoot);
                        itemUIClone.transform.localPosition = Vector3.zero;
                        itemUIClone.transform.localScale = Vector3.one;

                        itemUIClone.UpdateUI(item);

                        if (itemUIClone.buyBtn)
                        {
                            itemUIClone.buyBtn.onClick.RemoveAllListeners();
                            itemUIClone.buyBtn.onClick.AddListener(() => Purchase(item));
                        }
                    }
                }
            }
        }

        void Purchase(IAPItem item)
        {
            if (IAPManager.Ins.controller != null)
            {
                Close();

                Dialog messDialog = FindObjectOfType<MessageDialog>(true);

                if (messDialog)
                {
                    messDialog.contentText.text = "Processing...";
                    messDialog.Show(true);
                }

                IAPManager.Ins.controller.InitiatePurchase(item.id);
            }
        }
    }
}
