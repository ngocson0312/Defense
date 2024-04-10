using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UDEV.GhostDefense {
    public class LevelItemUI : MonoBehaviour
    {
        [SerializeField] private Text m_levelTxt;
        [SerializeField] private Image m_lockIcon;
        [SerializeField] private Image[] m_starImgs;
        public Button btnComp;
        [SerializeField] private Sprite m_activeStar;
        [SerializeField] private Sprite m_deactiveStar;

        public void UpdateUI(LevelItem levelItem, int levelId)
        {
            if (levelItem == null) return;

            bool isUnlocked = GameData.Ins.IsLevelUnlocked(levelId);
            int stars = GameData.Ins.GetLevelStars(levelId);

            if(m_starImgs != null && m_starImgs.Length > 0)
            {
                for (int i = 0; i < m_starImgs.Length; i++)
                {
                    var starImg = m_starImgs[i];
                    if (starImg == null) continue;
                    starImg.sprite = m_deactiveStar;
                }

                for (int i = 0; i < stars; i++)
                {
                    var starImg = m_starImgs[i];
                    if (starImg == null) continue;
                    starImg.sprite = m_activeStar;
                }
            }

            if (m_levelTxt)
            {
                m_levelTxt.gameObject.SetActive(isUnlocked);
                m_levelTxt.text = (levelId + 1).ToString();
            }

            if (m_lockIcon)
            {
                m_lockIcon.gameObject.SetActive(!isUnlocked);
            }
        }
    }
}
