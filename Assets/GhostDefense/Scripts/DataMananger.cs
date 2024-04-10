using UnityEngine;
using UnityEngine.Events;

namespace UDEV.GhostDefense
{
    public class DataMananger : Singleton<DataMananger>
    {
        public ShopItemData shopItemData;
        public LevelItemData levelItemData;

        private int m_curLevelId;
        public int CurLevelId { get => m_curLevelId; set => m_curLevelId = value; }

        public LevelItem CurLevel
        {
            get => levelItemData.levels[m_curLevelId];
        }

        public ShopItem CurShopItem
        {
            get => shopItemData.items[GameData.Ins.curPlayerId];
        }

        private void Start()
        {
            Inititalize();
        }

        public void Inititalize()
        {
            GameData.Ins.LoadData(Pref.GameData);
            ShopInitilize();
            LevelsInitialize();
            AudioController.Ins.SetMusicVolume(GameData.Ins.musicVol);
            AudioController.Ins.SetSoundVolume(GameData.Ins.soundVol);
            GameData.Ins.SaveData();
        }

        private void LevelsInitialize()
        {
            if (!levelItemData) return;

            var levels = levelItemData.levels;

            if (levels == null || levels.Length <= 0) return;

            for (int i = 0; i < levels.Length; i++)
            {
                var level = levels[i];

                if (level == null) continue;

                var levelData = GameData.Ins.GetLevelData(i);

                if (levelData != null) continue;

                levelData = new LevelData();

                if (i == 0)
                {
                    levelData.isUnlocked = true;
                    GameData.Ins.curLevelId = i;
                }
                else
                {
                    levelData.isUnlocked = false;
                }

                GameData.Ins.UpdateLevelData(levelData, i);
            }
        }

        public void SelectLevel(LevelItem level, int levelId, UnityAction SelectedLevel = null)
        {
            if (level == null) return;

            bool isUnlocked = GameData.Ins.IsLevelUnlocked(levelId);

            if (isUnlocked)
            {
                GameData.Ins.curLevelId = levelId;
                CurLevelId = levelId;
                GameData.Ins.SaveData();
                SelectedLevel?.Invoke();
                SceneController.Ins.LoadGameplay();
            }
            else
            {
                Debug.Log("Level not unlock yet !!!");
            }
        }

        private void ShopInitilize()
        {
            if(!shopItemData) return;

            var items = shopItemData.items;

            if (items == null || items.Length <= 0) return;

            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];

                if (item == null) continue;

                var playerData = GameData.Ins.GetPlayerData(i);

                if (playerData != null) continue;

                playerData = new PlayerData();

                if (i == 0)
                {
                    playerData.isUnlocked = true;
                    GameData.Ins.curLevelId = i;
                }
                else
                {
                    playerData.isUnlocked = false;
                }
                GameData.Ins.UpdatePlayerData(playerData, i);
            }
        }

        public void UnlockHero(ShopItem item, int playerId, UnityAction BuyingSuccess = null)
        {
            if (GameData.Ins.coin >= item.price)
            {
                GameData.Ins.coin -= item.price;
                GameData.Ins.UpdatePlayerUnlocked(playerId, true);
                GameData.Ins.curPlayerId = playerId;
                GameData.Ins.SaveData();
                GameManager.Ins?.ChangePlayer();
                BuyingSuccess?.Invoke();
            }
        }

        public void UpgradeHero(Stat stat, UnityAction UpgradeSuccess = null)
        {
            if (!stat) return;

            stat.Upgrade(
                () =>
                {
                    GameManager.Ins?.Player?.LoadStat();
                    UpgradeSuccess?.Invoke();
                });
        }
    }
}
