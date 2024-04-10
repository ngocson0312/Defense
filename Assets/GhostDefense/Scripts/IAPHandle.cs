using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace UDEV.GhostDefense
{
    public class IAPHandle : MonoBehaviour
    {
        private string m_buyingMessage;

        private void Start()
        {
            IAPManager.Ins?.OnProccessing.RemoveAllListeners();
            IAPManager.Ins?.OnAdsBuying.RemoveAllListeners();
            IAPManager.Ins?.OnItemBuying.RemoveAllListeners();
            IAPManager.Ins?.OnBuyingFailed.RemoveAllListeners();

            IAPManager.Ins?.OnAdsBuying.AddListener(BuyNoAds);
            IAPManager.Ins?.OnItemBuying.AddListener((IAPItem item) => BuyItem(item));
            IAPManager.Ins?.OnBuyingFailed.AddListener((string reason) => BuyFailed(reason));
            IAPManager.Ins?.OnProccessing.AddListener(Proccessing);
        }

        private void BuyNoAds()
        {
            GameData.Ins.isNoAds = true;
            GameData.Ins.SaveData();
            m_buyingMessage = "Ads remove successful!.Please restart game.";
            OpenMessageDialog(m_buyingMessage);
            //FirebaseController.Ins.AddLogEvent(FBLogEvent.iap_no_ads.ToString());
        }

        private void BuyItem(IAPItem buyingItem)
        {
            if (buyingItem == null) return;
            
            GameData.Ins.coin += buyingItem.amount;
            GameData.Ins.SaveData();

            m_buyingMessage = "You Got x" +
                    buyingItem.amount.ToString("n0") + " Coins";

            OpenMessageDialog(m_buyingMessage);
        }

        private void BuyFailed(string reason)
        {
            OpenMessageDialog(reason);
        }

        private void Proccessing()
        {
            IAPDialog iapDialog = FindObjectOfType<IAPDialog>();
            if (iapDialog)
            {
                iapDialog.Close();
            }

            OpenMessageDialog("Processing...");
        }

        private void OpenMessageDialog(string mess)
        {
            Dialog messDialog = FindObjectOfType<MessageDialog>(true);

            if (messDialog)
            {
                messDialog.contentText.text = mess;
                messDialog.Show(true);
            }
        }
    }
}
