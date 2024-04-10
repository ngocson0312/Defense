using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.GhostDefense
{
    public class ReviveDialog : Dialog
    {
        public void ReviveUseCoin()
        {
            if(GameData.Ins.coin >= GameManager.Ins.setting.revivePrice)
            {
                Close();
                GameManager.Ins.Revive(() =>
                {
                    GameData.Ins.coin -= GameManager.Ins.setting.revivePrice;
                    GameData.Ins.SaveData();
                    GUIManager.Ins.UpdateCoinCounting();
                });
            }
        }

        public void ReviveUseAds()
        {
            if(GameManager.Ins.IsCanRevive())
            {
                //Show Rewarded Ads

                //FirebaseController.Ins.AddLogEvent(FBLogEvent.revive_ads.ToString());
            }
        }

        public void CloseBtn()
        {
            Close();
            GameManager.Ins.Gameover();
        }
    }
}
