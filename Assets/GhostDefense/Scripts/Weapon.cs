using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.GhostDefense
{
    public class Weapon : MonoBehaviour, IDamageCreater
    {
        [Header("Common:")]
        public GameTag damageTo;
        [SerializeField] private LayerMask m_targetLayer;
        [SerializeField] private float m_atkRadius;
        [SerializeField] private Vector3 offset;
        public float damage;

        [SerializeField] private Actor m_owner;

        [Header("Cam Shake:")]
        [SerializeField] private bool m_useCameraShake;
        [SerializeField] private float m_shakeDur;
        [SerializeField] private float m_shakeFreq;
        [SerializeField] private float m_shakeAmpli;

        public Actor Owner { get => m_owner; set => m_owner = value; }

        private void Start()
        {
            if (m_owner == null) return;

            damageTo = m_owner.damageTo;

            damage = m_owner.CurDmg;
        }

        public void DealDamage()
        {
            if(m_owner == null) return;

            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position + offset, m_atkRadius, m_targetLayer);

            if (cols == null || cols.Length <= 0) return;

            for (int i = 0; i < cols.Length; i++)
            {
                var col = cols[i];
                if(col == null) continue;
                if (col.gameObject.CompareTag(damageTo.ToString()))
                {
                    Actor actor = col.gameObject.GetComponent<Actor>();

                    if (actor)
                    {
                        actor.TakeDamage(damage, m_owner);
                    }
                }
            }

            if (m_useCameraShake)
            {
                CamShake.Ins.ShakeTrigger(m_shakeDur, m_shakeFreq, m_shakeAmpli);
            }

            damage = m_owner.CurDmg;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Helper.ChangAlpha(Color.yellow, 0.4f);
            Gizmos.DrawSphere(transform.position + offset, m_atkRadius);
        }
    }
}
