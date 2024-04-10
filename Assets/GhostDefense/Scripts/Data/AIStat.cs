using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UDEV.GhostDefense
{
    [CreateAssetMenu(fileName = "AI Stat", menuName = "Stats/Enemy")]
    public class AIStat : ActorStat
    {
        [Header("Common:")]
        [Range(0f, 1f)]
        public float atkRate;
        [Range(0f, 1f)]
        public float dashRate;
        [Range(0f, 1f)]
        public float ultiRate;
        public float atkTime;
        public float ultiTime;
        public float dashTime;

        [Header("Collect:")]
        public float minXpBonus;
        public float maxXpBonus;
        public float minEnergyBonus;
        public float maxEnergyBonus;

        [Header("Level Up:")]
        public float hpUpRate;
        public float dmgUpRate;
        public float ultiUpRate;

        [Header("Level Up Factor:")]
        public float hpUpFactor = 2;
        public float dmgUpFactor = 5;
        public float ultiRateUpFactor = 4;

        public float CurHp
        {
            get => MaxUpgradeValue(hpUpFactor, hp, hpUpRate);
        }

        public float CurDmg
        {
            get => MaxUpgradeValue(dmgUpFactor, damage, dmgUpRate);
        }

        public float CurUltiRate
        {
            get => MaxUpgradeValue(ultiRateUpFactor, ultiRate, ultiUpRate, true);
        }

        public float XpBonus
        {
            get => Random.Range(minXpBonus, maxXpBonus) * (GameData.Ins.curLevelId + 1);
        }

        public float EnergyBonus
        {
            get => Random.Range(minEnergyBonus, maxEnergyBonus);
        }

        private float MaxUpgradeValue(float factor, float oldValue, float upValueRate, bool isPercent = false)
        {
            float maxValue = 0;

            if (isPercent)
            {
                for (int i = 0; i < GameData.Ins.curLevelId + 1; i++)
                {
                    maxValue += (Helper.UpgradeForm(i, factor) * upValueRate) / 100;
                }
            }else
            {
                for (int i = 0; i < GameData.Ins.curLevelId + 1; i++)
                {
                    maxValue += (Helper.UpgradeForm(i, factor) * upValueRate);
                }
            }

            maxValue += oldValue;

            return maxValue;
        }

        public override void UpgradeCore()
        {
            hp = CurHp;
            damage = CurDmg;
            ultiRate = CurUltiRate;
        }

        public override void Upgrade(UnityAction Success = null, UnityAction Failed = null)
        {
            GameData.Ins.curLevelId++;
            UpgradeCore();
        }
    }
}
