using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.GhostDefense {
    public class AnimEvent : MonoBehaviour
    {
        [SerializeField] private Actor m_owner;
        [SerializeField] private GameObject m_weapon;
        [SerializeField] private UltiManager m_ultiMng;

        private void Start()
        {
            if (!m_ultiMng || !m_owner) return;

            m_ultiMng.Owner = m_owner;
        }

        public void Dash()
        {
            if (m_owner)
            {
                m_owner.Dash();
                AudioController.Ins.PlaySound(AudioController.Ins.dash);
            }
        }

        public void WeaponAttack()
        {
            if (!m_weapon) return;

            UltiController ultiCtr = m_weapon.GetComponent<UltiController>();
            if (ultiCtr)
            {
                ultiCtr.Owner = m_owner;
            }

            IDamageCreater dmgCreater = m_weapon.GetComponent<IDamageCreater>();
            if(dmgCreater != null)
            {
                dmgCreater.DealDamage();
            }
        }

        public void UltiTrigger()
        {
            if (!m_ultiMng) return;
            m_ultiMng.UltiTrigger();
        }

        public void Deactive()
        {
            if(!m_owner) return;
            m_owner.gameObject.SetActive(false);
        }

        public void PlayFootstepSound()
        {
            AudioController.Ins.PlaySound(AudioController.Ins.footSteps);
        }
    }
}
