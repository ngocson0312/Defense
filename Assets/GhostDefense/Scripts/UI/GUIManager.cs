using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Guirao.UltimateTextDamage;
using UnityEngine.UI;

namespace UDEV.GhostDefense {
    public class GUIManager : Singleton<GUIManager>
    {
        [Header("Txt Damage:")]
        public UltimateTextDamageManager dmgTxtMng;

        [Header("Mobile Gamepad:")]
        [SerializeField] private GameObject m_mobileGamepad;
        public ImageFilled atkBtnFilled;
        public ImageFilled dashBtnFilled;
        public ImageFilled ultiBtnFilled;

        [Header("Hero Info:")]
        public ImageFilled hpBar;
        public ImageFilled energyBar;
        [SerializeField] private Image m_heroAvatar;
        [SerializeField] private Text m_lvCountingTxt;
        [SerializeField] private Text m_ptCountingTxt;

        [Header("Wave Info:")]
        public ImageFilled waveBar;
        public ImageFilled bossHpBar;
        [SerializeField] private Text m_waveBarTxt;

        [Header("Other:")]
        [SerializeField] private Text m_coinCountingTxt;
        public Text waveCountingTxt;
        public Text missionCompletedTxt;
        public Text youDieTxt;
        [SerializeField] private RectTransform m_coinMovingDest;

        [Header("Dialog:")]
        public Dialog completedDialog;
        public Dialog gameoverDialog;
        public Dialog reviveDialog;


        protected override void Awake()
        {
            MakeSingleton(false);
        }

        public void ShowMobileGamepad(bool isShow)
        {
            if (m_mobileGamepad)
            {
                m_mobileGamepad.SetActive(isShow);
            }
        }

        private void UpdateTxt(Text txt, string content)
        {
            if (txt)
            {
                txt.text = content;
            }
        }

        public void UpdateHeroLevel(int level)
        {
            UpdateTxt(m_lvCountingTxt, $"Level {level}");
        }

        public void UpdateHeroPoint(int point)
        {
            UpdateTxt(m_ptCountingTxt, $"Point {point}");
        }

        public void UpdateHeroAvatar(Sprite av)
        {
            if (m_heroAvatar)
            {
                m_heroAvatar.sprite = av;
            }
        }

        public void UpdateCoinCounting()
        {
            UpdateTxt(m_coinCountingTxt, GameData.Ins.coin.ToString());
        }

        public void UpdateWaveCounting(int cur, int total)
        {
            string content = string.Empty;
            if(cur == total)
            {
                content = "FINAL WAVE";
            }else
            {
                content = $"WAVE {cur} / {total}";
            }

            UpdateTxt(m_waveBarTxt, content);
            UpdateTxt(waveCountingTxt, content);
        }

        public Vector3 GetCoinIconUICorner()
        {
            if (!m_coinMovingDest) return Vector3.zero;

            var rect = m_coinMovingDest;
            Vector3[] v = new Vector3[4];
            rect.GetWorldCorners(v);
            return v[0];
        }
    }
}
