using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.GhostDefense
{
    [CreateAssetMenu(menuName = "Game Setting", fileName = "GameSetting")]
    public class GameplaySetting : ScriptableObject
    {
        public bool isOnMobile;
        public int revivePrice;
        public int maxRevive;
    }
}
