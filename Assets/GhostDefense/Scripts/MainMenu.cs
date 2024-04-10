using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.GhostDefense
{
    public class MainMenu : MonoBehaviour
    {
        public void Init()
        {
            PlayMainMenuMusic();

            Pref.IsFirstTime = false;
        }

        public void PlayMainMenuMusic()
        {
            AudioController.Ins.PlayMusic(AudioController.Ins.menus);
        }
    }
}
