using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.GhostDefense
{
    public class UIAnimEvent : MonoBehaviour
    {
        public void Deactive()
        {
            gameObject.SetActive(false);
        }

        public void ShowCompletedDialog()
        {
            if (GUIManager.Ins.completedDialog)
            {
                GUIManager.Ins.completedDialog.Show(true);
            }
            //FirebaseController.Ins.AddLogEvent(FBLogEvent.gameover_ads.ToString());

            //Show Interstitial Ads
        }

        public void ShowGameoverDialog()
        {
            //AdmobController.Ins.ShowInterstitial();
            if (GUIManager.Ins.gameoverDialog)
            {
                GUIManager.Ins.gameoverDialog.Show(true);
            }

            //FirebaseController.Ins.AddLogEvent(FBLogEvent.mission_completed_ads.ToString());

            //Show Interstitial Ads
        }
    }
}
