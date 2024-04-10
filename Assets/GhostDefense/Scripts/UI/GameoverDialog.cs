using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UDEV.GhostDefense
{
    public class GameoverDialog : Dialog
    {
        [SerializeField] private Text m_gameplayTimeTxt;
        [SerializeField] private Text m_bestTimeTxt;

        public override void Show(bool isShow)
        {
            base.Show(isShow);
            if (m_gameplayTimeTxt)
            {
                m_gameplayTimeTxt.text = Helper.TimeConvert(GameManager.Ins.GplayTimeCounting);
            }

            if (m_bestTimeTxt)
            {
                float bestTime = GameData.Ins.GetLevelScore(GameData.Ins.curLevelId);
                m_bestTimeTxt.text = Helper.TimeConvert(bestTime);
            }
        }

        public void Replay()
        {
            Close();
            GameManager.Ins.Replay();
        }
    }
}
