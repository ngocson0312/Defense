using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.GhostDefense
{

    [CreateAssetMenu(fileName = "New Level Item Data", menuName = "Data/Level Item Data")]
    public class LevelItemData : ScriptableObject
    {
        public LevelItem[] levels;
    }

    [System.Serializable]
    public class LevelItem
    {
        public int minBonus;
        public int maxBonus;
        public int minXpBonus;
        public int maxXpBonus;
        public Goal goal;
        public WavePlayer waveCtrFb;
        public FreeParallax mapFb;
    }

    [System.Serializable]
    public class Goal
    {
        public int timeOneStar;
        public int timeTwoStar;
        public int timeThreeStar;

        public int GetStar(int time)
        {
            if (time < timeOneStar)
            {
                return 3;
            }
            else if (time < timeTwoStar)
            {
                return 2;
            }
            else
            {
                return 1;
            }
        }
    }
}
