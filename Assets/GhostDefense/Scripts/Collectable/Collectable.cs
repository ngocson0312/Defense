using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UDEV.SPM;
using UDEV.GhostDefense.Editor;

namespace UDEV.GhostDefense {
    [RequireComponent(typeof(Rigidbody2D))]
    public class Collectable : MonoBehaviour
    {
        [SerializeField] private int m_minBonus;
        [SerializeField] private int m_maxBonus;
        [SerializeField] private int m_lifeTime;
        [SerializeField] private float m_spawnForce;
        [LayerList]
        [SerializeField] private int m_collectedLayer;
        [LayerList]
        [SerializeField] private int m_normalLayer;
        [SerializeField] private AudioClip m_hitSound;
        [SerializeField] private bool m_deactiveWhenHitted;

        protected int m_bonus;
        protected Player m_player;
        protected bool m_isNotMoving;

        private int m_timeCounting;
        private Rigidbody2D m_rb;
        private FlashVfx m_flashVfx;

        private void Awake()
        {
            m_rb = GetComponent<Rigidbody2D>();
            m_flashVfx = GetComponent<FlashVfx>();
        }

        public virtual void Init()
        {
            gameObject.layer = m_normalLayer;

            m_isNotMoving = false;
            m_player = GameManager.Ins.Player;
            m_timeCounting = m_lifeTime;

            if (!m_player || !m_rb || !m_flashVfx) return;

            m_bonus = Random.Range(m_minBonus, m_maxBonus) * (GameData.Ins.curLevelId + 1);

            float randForce = Random.Range(-m_spawnForce, m_spawnForce);

            m_rb.velocity = new Vector2(randForce, randForce);

            StartCoroutine(StopMove());

            m_flashVfx.OnCompleted.RemoveAllListeners();
            m_flashVfx.OnCompleted.AddListener(() =>
            {
                gameObject.SetActive(false);
            });
            
            StartCoroutine(CountingDown());
        }

        public void Trigger()
        {
            TriggerCore();

            if (m_deactiveWhenHitted)
            {
                gameObject.SetActive(false);
            }
            gameObject.layer = m_collectedLayer;

            AudioController.Ins.PlaySound(m_hitSound);
        }

        protected virtual void TriggerCore()
        {
            
        }

        private IEnumerator CountingDown()
        {
            while (m_timeCounting > 0)
            {
                yield return new WaitForSeconds(1f);

                m_timeCounting--;

                float timeRate = Mathf.Round((float)m_timeCounting / (float)m_lifeTime);

                if (timeRate <= 0.3f)
                {
                    m_flashVfx.Flash(m_timeCounting);
                }
            }
        }

        private IEnumerator StopMove()
        {
            yield return new WaitForSeconds(1f);
            m_rb.velocity = Vector2.zero;
            m_isNotMoving = true;
        }
    }
}
