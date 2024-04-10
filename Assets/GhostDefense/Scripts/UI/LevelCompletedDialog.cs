using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UDEV.GhostDefense
{
    public class LevelCompletedDialog : Dialog
    {
        [SerializeField] private Image[] m_stars;
        [SerializeField] private Text m_gameplayTimeTxt;
        [SerializeField] private Text m_bonusTxt;
        [SerializeField] private Sprite m_activeStar;
        [SerializeField] private Sprite m_deactiveStar;

        public override void Show(bool isShow)
        {
            base.Show(isShow);

            if (m_stars == null || m_stars.Length <= 0) return;

            for (int i = 0; i < m_stars.Length; i++)
            {
                var star = m_stars[i];
                if(!star) continue;
                star.sprite = m_deactiveStar;
            }

            for (int i = 0; i < GameManager.Ins.Stars; i++)
            {
                var star = m_stars[i];
                if (!star) continue;
                star.sprite = m_activeStar;
            }

            if (m_gameplayTimeTxt)
            {
                m_gameplayTimeTxt.text = Helper.TimeConvert(GameManager.Ins.GplayTimeCounting);
            }

            if (m_bonusTxt)
            {
                m_bonusTxt.text = GameManager.Ins.MissionCoinBonus.ToString();
            }
        }

        public void Replay()
        {
            Close();
            GameManager.Ins.Replay();
        }

        public void NextLevel()
        {
            LevelItem[] levels = DataMananger.Ins.levelItemData.levels;

            if(levels == null || levels.Length <= 0) return;

            if(GameData.Ins.curLevelId >= levels.Length - 1)
            {
                SceneController.Ins.LoadScene(GameScene.MainMenu.ToString());
            }else
            {
                SceneController.Ins.LoadGameplay();
            }
        }
    }
}
