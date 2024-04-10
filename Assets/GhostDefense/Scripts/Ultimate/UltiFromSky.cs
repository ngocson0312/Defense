using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UDEV.SPM;
using System;

namespace UDEV.GhostDefense {
    public class UltiFromSky : UltiController
    {
        [PoolerKeys(target = PoolerTarget.NONE)]
        [SerializeField] protected string m_weaponPool;
        [SerializeField] private int m_targetNum;
        [SerializeField] private LayerMask m_targetLayer;
        [SerializeField] private float m_atkRadius;
        [SerializeField] protected float m_weaponSpeed;

        protected List<AI> m_targets = new List<AI>();

        public override void DealDamage()
        {
            FindTargets();
        }

        protected void FindTargets()
        {
            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, m_atkRadius, m_targetLayer);
            if (cols == null || cols.Length <= 0) return;
            for (int i = 0; i < cols.Length; i++)
            {
                var col = cols[i];
                if (col == null) continue;
                if (col.CompareTag(m_owner.damageTo.ToString()))
                {
                    AI aiComp = col.GetComponent<AI>();
                    if(!aiComp) continue;
                    m_targets.Add(aiComp);
                }
            }
        }

        protected int GetCurTargetNum()
        {
            int curentTargetNum = 0;
            curentTargetNum = m_targets.Count > m_targetNum ? m_targetNum : m_targets.Count;
            return curentTargetNum;
        }

        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = Helper.ChangAlpha(Color.green, 0.4f);
            Gizmos.DrawSphere(transform.position, m_atkRadius);
        }
    }
}
