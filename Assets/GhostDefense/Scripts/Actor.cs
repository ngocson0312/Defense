using System.Collections;
using System.Collections.Generic;
using UDEV.GhostDefense.Editor;
using UnityEngine;
using UDEV.SPM;
using System;

namespace UDEV.GhostDefense
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Actor : MonoBehaviour
    {
        [Header("Common: ")]
        public GameTag damageTo;
        public ActorStat stat;

        [Header("Layers:")]
        [LayerList]
        public int normalLayer;
        [LayerList]
        public int invincibleLayer;
        [LayerList]
        public int deadLayer;

        [Header("Reference: ")]
        [SerializeField]
        protected Animator m_anim;
        protected Rigidbody2D m_rb;

        [Header("Vfx: ")]
        [PoolerKeys(target = PoolerTarget.NONE)]
        [SerializeField] private string m_healthBarPool;
        [SerializeField] private Vector3 m_hpBarOffset;
        [SerializeField] private Vector3 m_hpBarScale = Vector3.one;
        [SerializeField] private FlashVfx m_flashVfx;
        [PoolerKeys(target = PoolerTarget.NONE)]
        [SerializeField] private string m_deadVfxPool;

        protected Actor m_whoHit;

        protected float m_curHp;
        protected float m_curDmg;
        protected bool m_isKnockBack;
        protected bool m_isInvincible;
        protected float m_startingGrav;
        protected bool m_isFacingLeft;
        protected float m_curSpeed;
        protected float m_dmgTaked;

        protected ImageFilled m_healthBar;

        public float CurHp
        {
            get => m_curHp;
            set
            {
                m_curHp = value;
                m_curHp = Mathf.Clamp(m_curHp, 0, stat.hp);
            }
        }

        public float CurDmg{ get => m_curDmg;}
        public bool IsFacingLeft { get => m_isFacingLeft;}
        public float CurSpeed { get => m_curSpeed;} 
        public Actor WhoHit { get => m_whoHit; set => m_whoHit = value; }

        protected virtual void Awake ()
        {
            m_rb = GetComponent<Rigidbody2D>();
            m_startingGrav = m_rb.gravityScale;
        }

        private void LateUpdate()
        {
            if (m_healthBar)
            {
                FlipHpBarOffset();
                m_healthBar.transform.position = transform.position + m_hpBarOffset;
            }
        }

        public virtual void Init()
        {

        }

        public virtual void TakeDamage(float dmg, Actor whoHit)
        {
            if (m_isInvincible || m_isKnockBack) return;

            if(m_curHp > 0)
            {
                m_dmgTaked = dmg;
                m_whoHit = whoHit;

                m_curHp -= dmg;
                if(m_curHp <= 0)
                {
                    m_curHp = 0;
                    Dead();
                }
                KnockBack();
            }
        }

        protected void KnockBack()
        {
            if (m_isInvincible || m_isKnockBack || !gameObject.activeInHierarchy) return;

            m_isKnockBack = true;

            StartCoroutine(StopKnockBack());

            if (m_flashVfx)
            {
                m_flashVfx.Flash(stat.invincibleTime);
            }
        }

        protected void KnockbackMove(float yRate)
        {
            if (!m_whoHit) return;

            Vector2 dir = m_whoHit.transform.position - transform.position;
            dir.Normalize();
            if(dir.x > 0)
            {
                m_rb.velocity = new Vector2(-stat.knockbackForce, yRate * stat.knockbackForce);
            }else
            {
                m_rb.velocity = new Vector2(stat.knockbackForce, yRate * stat.knockbackForce);
            }
        }

        protected IEnumerator StopKnockBack()
        {
            yield return new WaitForSeconds(stat.knockbackTime);

            m_isKnockBack = false;

            m_isInvincible = true;

            gameObject.layer = invincibleLayer;

            StartCoroutine(StopInvincible(stat.invincibleTime));
        }

        protected IEnumerator StopInvincible(float time)
        {
            yield return new WaitForSeconds(time);

            m_isInvincible = false;

            gameObject.layer = normalLayer;
        }

        public void Invincible(float time)
        {
            m_isInvincible = true;

            gameObject.layer = invincibleLayer;

            if (m_flashVfx)
            {
                m_flashVfx.Flash(time);
            }

            StartCoroutine(StopInvincible(time));
        }

        protected virtual void Dead()
        {
            m_rb.velocity = Vector2.zero;

            if (m_healthBar)
            {
                m_healthBar.Show(false);
            }

            gameObject.layer = deadLayer;

            PoolersManager.Ins.Spawn(PoolerTarget.NONE, m_deadVfxPool, transform.position, Quaternion.identity);
        }

        public virtual void Dash()
        {

        }

        protected void Flip(Direction dir)
        {
            switch (dir)
            {
                case Direction.Left:
                    if(transform.localScale.x > 0)
                    {
                        transform.localScale = new Vector3(
                            transform.localScale.x * -1,
                            transform.localScale.y, transform.localScale.z
                            );
                        m_isFacingLeft = true;
                    }
                    break;
                case Direction.Right:
                    if (transform.localScale.x < 0)
                    {
                        transform.localScale = new Vector3(
                            transform.localScale.x * -1,
                            transform.localScale.y, transform.localScale.z
                            );
                        m_isFacingLeft = false;
                    }
                    break;
                case Direction.Top:
                    if (transform.localScale.y < 0)
                    {
                        transform.localScale = new Vector3(
                            transform.localScale.x,
                            transform.localScale.y * -1, transform.localScale.z
                            );
                    }
                    break;
                case Direction.Bottom:
                    if (transform.localScale.y > 0)
                    {
                        transform.localScale = new Vector3(
                            transform.localScale.x,
                            transform.localScale.y * -1, transform.localScale.z
                            );
                    }
                    break;
            }
        }

        protected void ReduceActionRate(ref bool isActed, ref float curTime, float totalTime)
        {
            if(isActed)
            {
                curTime -= Time.deltaTime;
                if(curTime <= 0)
                {
                    isActed = false;
                    curTime = totalTime;
                }
            }
        }

        protected void CreateHealthBarUI()
        {
            GameObject hpBar = PoolersManager.Ins.Spawn(PoolerTarget.NONE, m_healthBarPool, transform.position, Quaternion.identity);
            if (!hpBar) return;

            hpBar.transform.localScale = m_hpBarScale;
            m_healthBar = hpBar.GetComponent<ImageFilled>();

            if (!m_healthBar) return;

            m_healthBar.Show(true);
            m_healthBar.UpdateValue(m_curHp, stat.hp);
            m_healthBar.Root = transform;
        }

        protected void FlipHpBarOffset()
        {
            if (m_isFacingLeft)
            {
                if(m_hpBarOffset.x < 0)
                {
                    m_hpBarOffset = new Vector3(m_hpBarOffset.x * -1, m_hpBarOffset.y, m_hpBarOffset.z);
                }
            }
            else
            {
                if (m_hpBarOffset.x > 0)
                {
                    m_hpBarOffset = new Vector3(m_hpBarOffset.x * -1, m_hpBarOffset.y, m_hpBarOffset.z);
                }
            }
        }

        protected void LimitHozMoving()
        {
            float minX = CameraFollow.Ins.LeftLimit - 6f;
            float maxX = CameraFollow.Ins.RightLimit + 6f;
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, minX, maxX), transform.position.y,
                transform.position.z);
        }

        protected virtual void OnDrawGizmos()
        {
            if (!string.IsNullOrEmpty(m_healthBarPool))
            {
                Gizmos.DrawIcon(transform.position + m_hpBarOffset, "HPBar_Icon.png", true);
            }
        }
    }
}
