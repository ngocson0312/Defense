using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UDEV.GhostDefense {
    public class ShopDialog : Dialog
    {
        [SerializeField] private Text m_totalCoinTxt;
        [SerializeField] private Image m_heroPreview;
        [SerializeField] private Text m_heroNameTxt;
        [SerializeField] private Image m_heroAvatar;
        [SerializeField] private Image m_levelFilled;
        [SerializeField] private Text m_lvProgTxt;
        [SerializeField] private Text m_lvCountingTxt;
        [SerializeField] private Text m_pointTxt;
        [SerializeField] private Image m_hpFilled;
        [SerializeField] private Image m_atkFilled;
        [SerializeField] private Image m_defFilled;
        [SerializeField] private Image m_luckFilled;
        [SerializeField] private Button m_unlockBtn;
        [SerializeField] private Button m_upgradeBtn;
        [SerializeField] private Text m_unlockBtnTxt;
        [SerializeField] private Text m_upgradeBtnTxt;
        [SerializeField] private Image m_nextBtnImg;
        [SerializeField] private Image m_prevBtnImg;
        [SerializeField] private Sprite m_navBtnNormal;
        [SerializeField] private Sprite m_navBtnActive;

        private ShopItem[] m_items;
        private int m_curPlayerId;
        private PlayerStat m_curStat;

        public override void Show(bool isShow)
        {
            base.Show(isShow);

            m_items = DataMananger.Ins.shopItemData.items;
            m_curPlayerId = GameData.Ins.curPlayerId;

            UpdateUI();
            SwitchNavigatorSprite(true);

            Time.timeScale = 0;
        }

        public override void Close()
        {
            base.Close();
            Time.timeScale = 1f;
        }

        private void UpdateUI()
        {
            if (m_totalCoinTxt)
            {
                m_totalCoinTxt.text = GameData.Ins.coin.ToString();
            }

            bool isUnlocked = GameData.Ins.IsPlayerUnlocked(m_curPlayerId);

            if (m_items == null || m_items.Length <= 0) return;

            var item = m_items[m_curPlayerId];

            if (item == null) return;

            if (item.heroPb && item.heroPb.stat)
            {
                m_curStat = (PlayerStat)item.heroPb.stat;
                m_curStat.Load(m_curPlayerId);
            }

            if (m_heroPreview)
            {
                m_heroPreview.sprite = item.preview;
            }

            if (m_heroNameTxt)
            {
                m_heroNameTxt.text = item.heroName;
            }

            if (m_heroAvatar)
            {
                m_heroAvatar.sprite = item.avatar;
            }

            if (m_levelFilled)
            {
                m_levelFilled.fillAmount = m_curStat.xp / m_curStat.lvUpXpRequired;
            }

            if (m_lvProgTxt)
            {
                m_lvProgTxt.text = (Mathf.RoundToInt(m_curStat.xp / m_curStat.lvUpXpRequired * 100)) + "%";
            }

            if (m_lvCountingTxt)
            {
                m_lvCountingTxt.text = $"Level {m_curStat.level}";
            }

            if (m_pointTxt)
            {
                m_pointTxt.text = $"{m_curStat.point} Point";
            }

            if (m_hpFilled)
            {
                m_hpFilled.fillAmount = m_curStat.hp / m_curStat.MaxHp;
            }

            if (m_atkFilled)
            {
                m_atkFilled.fillAmount = m_curStat.damage / m_curStat.MaxDmg;
            }

            if (m_defFilled)
            {
                m_defFilled.fillAmount = m_curStat.defense / m_curStat.MaxDef;
            }

            if (m_luckFilled)
            {
                m_luckFilled.fillAmount = m_curStat.luck / m_curStat.MaxLuck;
            }

            if (m_unlockBtn)
            {
                m_unlockBtn.gameObject.SetActive(!isUnlocked);
                m_unlockBtn.onClick.RemoveAllListeners();
                m_unlockBtn.onClick.AddListener(() => UnlockHeroBtnEvent(item));
            }

            if (m_unlockBtnTxt)
            {
                m_unlockBtnTxt.text = item.price.ToString();
            }

            if (m_upgradeBtn)
            {
                m_upgradeBtn.gameObject.SetActive(isUnlocked);
            }

            if (m_upgradeBtnTxt)
            {
                m_upgradeBtnTxt.text = $"{m_curStat.pointRequired} PT";
            }

            if (GUIManager.Ins && GameManager.Ins)
            {
                GUIManager.Ins.UpdateCoinCounting();
                GUIManager.Ins.UpdateHeroPoint(m_curStat.point);
                GUIManager.Ins.hpBar.UpdateValue(
                    GameManager.Ins.Player.CurHp,
                    GameManager.Ins.Player.CurStat.hp
                    );
                GUIManager.Ins.energyBar.UpdateValue(
                    GameManager.Ins.Player.CurEnergy,
                    GameManager.Ins.Player.CurStat.ultiEnergy
                    );
            }
        }

        private void UnlockHeroBtnEvent(ShopItem shopItem)
        {
            DataMananger.Ins?.UnlockHero(shopItem, m_curPlayerId, () =>
            {
                UpdateUI();
                AudioController.Ins.PlaySound(AudioController.Ins.unlock);
            });
        }

        public void UpgradeHeroBtnEvent()
        {
            DataMananger.Ins?.UpgradeHero(m_curStat, () =>
            {
                UpdateUI();
                AudioController.Ins.PlaySound(AudioController.Ins.upgrade);
            });
        }

        private void SwitchNavigatorSprite(bool isNext)
        {
            if (m_nextBtnImg)
            {
                m_nextBtnImg.sprite = isNext ? m_navBtnActive : m_navBtnNormal;
            }

            if (m_prevBtnImg)
            {
                m_prevBtnImg.sprite = isNext ? m_navBtnNormal : m_navBtnActive;
            }
        }

        private void SelectHero()
        {
            bool isUnlocked = GameData.Ins.IsPlayerUnlocked(m_curPlayerId);

            if (isUnlocked)
            {
                GameData.Ins.curPlayerId = m_curPlayerId;
                GameData.Ins.SaveData();

                if (GameManager.Ins)
                {
                    GameManager.Ins.ChangePlayer();
                }
            }
        }

        public void NextHero()
        {
            m_curPlayerId++;
            if(m_curPlayerId >= m_items.Length)
            {
                m_curPlayerId = 0;
            }

            SelectHero();
            UpdateUI();
            SwitchNavigatorSprite(true);
        }

        public void PrevHero()
        {
            m_curPlayerId--;
            if (m_curPlayerId < 0)
            {
                m_curPlayerId = m_items.Length - 1;
            }

            SelectHero();
            UpdateUI();
            SwitchNavigatorSprite(false);
        }
    }
}
