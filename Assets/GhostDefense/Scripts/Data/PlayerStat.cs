using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UDEV.GhostDefense
{
    [CreateAssetMenu(fileName = "Player Stat", menuName = "Stats/Player")]
    public class PlayerStat : ActorStat
    {
        [Header("Ability:")]
        public float runSpeed;
        public float atkRate;
        public float dashRate;
        public float dashDist;
        public float ultiEnergy;
        public float defense;
        public float luck;

        [Header("Level Up Base:")]
        public int maxLevel;
        public int level;
        public int playerLevel;
        public float xp;
        public float lvUpXpRequired;
        public int point;

        [Header("Level Up:")]
        public float xpUpRate;
        public float hpUpRate;
        public float dmgUpRate;
        public float defUpRate;
        public float luckUpRate;
        public float pointUpRate;
        public int pointRequired;
        public int pointRequiredUp;

        [Header("Level Up Factor:")]
        public float xpRequireUpFactor = 2;
        public float pointReceiveUpFactor = 2;
        public float pointRequireUpFactor = 2;
        public float hpUpFactor = 2;
        public float dmgUpFactor = 6;
        public float defUpFactor = 10;
        public float luckUpFactor = 2;
        public float LvUpXpRequireUp
        {
            get => Helper.UpgradeForm(playerLevel, xpRequireUpFactor) * xpUpRate;
        }

        public int PointReceiveUp
        {
            get => Mathf.RoundToInt(Helper.UpgradeForm(playerLevel, pointReceiveUpFactor) * pointUpRate);
        }

        public float HpUp
        {
            get => Helper.UpgradeForm(level, hpUpFactor) * hpUpRate;
        }

        public float DmgUp
        {
            get => Helper.UpgradeForm(level, dmgUpFactor) * dmgUpRate;
        }

        public float DefUp
        {
            get => Helper.UpgradeForm(level, defUpFactor) * defUpRate;
        }

        public override bool IsMaxLevel()
        {
            return level >= maxLevel;
        }

        private int PointRequiredUp
        {
            get => Mathf.RoundToInt(Helper.UpgradeForm(level, pointRequireUpFactor) * pointRequiredUp);
        }

        public float LuckUp
        {
            get => (Helper.UpgradeForm(level, luckUpFactor) * luckUpRate) / 100;
        }

        public float MaxHp
        {
            get => MaxUpgradeValue(hpUpFactor, hp, hpUpRate);
        }

        public float MaxDmg
        {
            get => MaxUpgradeValue(dmgUpFactor, damage, dmgUpRate);
        }

        public float MaxDef
        {
            get => MaxUpgradeValue(dmgUpFactor, defense, defUpRate);
        }

        public float MaxLuck
        {
            get => MaxUpgradeValue(2, luck, luckUpRate, true);
        }

        public override void Load(int id)
        {
            string data = GameData.Ins.GetPlayerStat(id);
            if(!string.IsNullOrEmpty(data))
            {
                JsonUtility.FromJsonOverwrite(data, this);
            }
        }

        public override void Save(int id)
        {
            string data = JsonUtility.ToJson(this);
            GameData.Ins.UpdatePlayerStat(id, data);
            GameData.Ins.SaveData();
        }

        private float MaxUpgradeValue(float factor, float oldValue, float upValue, bool isPercent = false)
        {
            float maxValue = 0;

            if(isPercent)
            {
                for (int i = level; i < maxLevel; i++)
                {
                    maxValue += (Helper.UpgradeForm(i, factor) * upValue) / 100;
                }
            }else
            {
                for (int i = level; i < maxLevel; i++)
                {
                    maxValue += Helper.UpgradeForm(i, factor) * upValue;
                }
            }

            maxValue += oldValue;
            return maxValue;
        }

        public override void Upgrade(UnityAction Success = null, UnityAction Failed = null)
        {
            if(!IsMaxLevel() && point >= pointRequired)
            {
                UpgradeCore();
                Save(GameData.Ins.curPlayerId);

                if (Success != null)
                {
                    Success.Invoke();
                }
            }else
            {
                if(Failed != null)
                {
                    Failed.Invoke();
                }
            }
        }

        public override void UpgradeCore()
        {
            point -= pointRequired;
            pointRequired += PointRequiredUp;
            hp += HpUp;
            damage += DmgUp;
            defense += DefUp;
            luck += LuckUp;
            level++;

            level = Mathf.Clamp(level, 1, maxLevel);
            hp = Mathf.Clamp(hp, 0, MaxHp);
            damage = Mathf.Clamp(damage, 0, MaxDmg);
            defense = Mathf.Clamp(defense, 0, MaxDef);
            luck = Mathf.Clamp(luck, 0f, 1f);
        }

        public override void UpgradeToMax()
        {
            while(level < maxLevel)
            {
                level++;
                UpgradeCore();
            }
        }

        public override void LevelUpCore()
        {
            xp -= lvUpXpRequired;
            lvUpXpRequired += LvUpXpRequireUp;
            point += PointReceiveUp;
            playerLevel++;
        }

        public IEnumerator LevelUpCo(UnityAction OnLevelUp = null)
        {
            while(xp >= lvUpXpRequired)
            {
                LevelUpCore();

                if(OnLevelUp != null)
                {
                    OnLevelUp.Invoke();
                }

                Save(GameData.Ins.curPlayerId);

                yield return new WaitForSeconds(0.5f);
            }

            yield return null;
        }
    }
}
