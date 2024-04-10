using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.GhostDefense {
    public class PauseDialog : SettingBaseDialog
    {
        public override void Show(bool isShow)
        {
            base.Show(isShow);
            Time.timeScale = 0f;
        }

        public override void Close()
        {
            base.Close();
            Time.timeScale = 1f;
        }

        public void Replay()
        {
            Close();
            GameManager.Ins.Replay();
        }
    }

}