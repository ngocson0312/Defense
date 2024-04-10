using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace UDEV.GhostDefense {
    [System.Serializable]
    public class LevelData
    {
        public bool isUnlocked;
        public bool isPassed;
        public int stars;
        public float completeTime;
    }

    [System.Serializable]
    public class PlayerData
    {
        public bool isUnlocked;
        public string stat;
    }

    public class GameData : Singleton<GameData>
    {
        public int coin;
        public int curLevelId;
        public int curPlayerId;
        public float musicVol;
        public float soundVol;
        public bool isNoAds;
        public List<LevelData> levelDatas;
        public List<PlayerData> playerDatas;
        private string m_data;

        public UnityEvent OnInit;
        public UnityEvent OnLocalLoaded;
        public UnityEvent OnCloudLoaded;
        public UnityEvent OnDataLoaded;

        protected override void Awake()
        {
            base.Awake();
            musicVol = 0.65f;
            soundVol = 1f;
            levelDatas = new List<LevelData>();
            playerDatas = new List<PlayerData>();

            m_data = string.Empty;
        }

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            if (OnInit != null)
            {
                OnInit.Invoke();
            }

            SaveData();
        }

        private void HandleData(string data)
        {
            if(string.IsNullOrEmpty(data)) return;

            JsonUtility.FromJsonOverwrite(data, this);
        }

        public void SaveData()
        {
            Pref.GameData = JsonUtility.ToJson(this);
        }

        public void LoadData(string data)
        {
            HandleData(data);

            if(OnDataLoaded != null)
            {
                OnDataLoaded.Invoke();
            }
        }

        public void LoadLocal()
        {
            if (Pref.IsFirstTime)
            {
                Init();
            }
            else
            {
                LoadData(Pref.GameData);
                SaveData();

                if (OnLocalLoaded != null)
                {
                    OnLocalLoaded.Invoke();
                }
            }
        }

        private T GetValue<T>(List<T> dataList, int idx)
        {
            if(dataList == null || dataList.Count <= 0 || idx < 0 || idx >= dataList.Count) return default;

            return dataList[idx];
        }

        private void UpdateValue<T>(ref List<T> dataList, int idx, T value)
        {
            if(dataList == null) return;

            if(dataList.Count <= 0 || (dataList.Count > 0 && idx >= dataList.Count))
            {
                dataList.Add(value);
            }else
            {
                dataList[idx] = value;
            }
        }

        #region LEVEL_METHODS
        public LevelData GetLevelData(int levelId)
        {
            return GetValue<LevelData>(levelDatas, levelId);
        }

        public void UpdateLevelData(LevelData levelData, int levelId)
        {
            UpdateValue<LevelData>(ref levelDatas, levelId, levelData);
        }

        public void UpdateLevelUnlocked(int levelId, bool isUnlocked)
        {
            LevelData level = GetLevelData(levelId);
            level.isUnlocked = isUnlocked;
            UpdateValue<LevelData>(ref levelDatas, levelId, level);
        }

        public void UpdateLevelPassed(int levelId, bool isPassed)
        {
            LevelData level = GetLevelData(levelId);
            level.isPassed = isPassed;
            UpdateValue<LevelData>(ref levelDatas, levelId, level);
        }

        public int GetLevelStars(int levelId)
        {
            LevelData level = GetLevelData(levelId);
            return level.stars;
        }

        public void UpdateLevelStars(int levelId, int stars)
        {
            LevelData level = GetLevelData(levelId);
            level.stars = stars;
            UpdateValue<LevelData>(ref levelDatas, levelId, level);
        }

        public float GetLevelScore(int levelId)
        {
            LevelData level = GetLevelData(levelId);
            return level.completeTime;
        }

        public void UpdateLevelScoreNoneCheck(int levelId, float completeTime)
        {
            LevelData level = GetLevelData(levelId);
            level.completeTime = completeTime;
            UpdateValue<LevelData>(ref levelDatas, levelId, level);
        }

        public void UpdateLevelScore(int levelId, float completeTime)
        {
            LevelData level = GetLevelData(levelId);
            float oldCompleteTime = level.completeTime;

            if (completeTime < oldCompleteTime)
            {
                level.completeTime = completeTime;
                UpdateValue<LevelData>(ref levelDatas, levelId, level);
            }
        }

        public bool IsLevelUnlocked(int levelId)
        {
            LevelData level = GetLevelData(levelId);
            return level.isUnlocked;
        }

        public bool IsLevelPassed(int levelId)
        {
            LevelData level = GetLevelData(levelId);
            return level.isUnlocked;
        }
        #endregion

        #region PLAYER_METHODS
        public PlayerData GetPlayerData(int playerId)
        {
            return GetValue<PlayerData>(playerDatas, playerId);
        }

        public void UpdatePlayerData(PlayerData playerData, int playerId)
        {
            UpdateValue<PlayerData>(ref playerDatas, playerId, playerData);
        }

        public bool GetPlayerUnlocked(int playerId)
        {
            PlayerData player = GetPlayerData(playerId);
            return player.isUnlocked;
        }

        public void UpdatePlayerUnlocked(int playerId, bool isUnlocked)
        {
            PlayerData player = GetPlayerData(playerId);
            player.isUnlocked = isUnlocked;
            UpdateValue<PlayerData>(ref playerDatas, playerId, player);
        }

        public string GetPlayerStat(int playerId)
        {
            PlayerData player = GetPlayerData(playerId);
            return player.stat;
        }

        public void UpdatePlayerStat(int playerId, string stat)
        {
            PlayerData player = GetPlayerData(playerId);
            player.stat = stat;
            UpdateValue<PlayerData>(ref playerDatas, playerId, player);
        }

        public bool IsPlayerUnlocked(int playerId)
        {
            PlayerData level = GetPlayerData(playerId);
            return level.isUnlocked;
        }
        #endregion
    }
}
