using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UDEV.GhostDefense
{
    public class LevelDialog : Dialog
    {
        [SerializeField] private Transform m_gridRoot;
        [SerializeField] private LevelItemUI m_itemUIPrefab;

        public override void Show(bool isShow)
        {
            base.Show(isShow);
            Time.timeScale = 0f;
            UpdateUI();
        }

        private void UpdateUI()
        {
            var levels = DataMananger.Ins.levelItemData.levels;

            if (levels == null || levels.Length <= 0 || !m_gridRoot || !m_itemUIPrefab) return;

            Helper.ClearChilds(m_gridRoot);

            for (int i = 0; i < levels.Length; i++)
            {
                int levelId = i;
                var level = levels[i];
                if (level == null) continue;

                var itemUIClone = Instantiate(m_itemUIPrefab, Vector3.zero, Quaternion.identity);
                itemUIClone.transform.SetParent(m_gridRoot);
                itemUIClone.transform.localScale = Vector3.one;
                itemUIClone.transform.localPosition = Vector3.zero;
                itemUIClone.UpdateUI(level, levelId);

                if (itemUIClone.btnComp)
                {
                    itemUIClone.btnComp.onClick.RemoveAllListeners();
                    itemUIClone.btnComp.onClick.AddListener(() => LevelSelectBtnEvent(level, levelId));
                }
            }
        }

        private void LevelSelectBtnEvent(LevelItem level, int levelId)
        {
            DataMananger.Ins.SelectLevel(level, levelId, Close);
        }

        public override void Close()
        {
            base.Close();
            Time.timeScale = 1f;
        }
    }
}
