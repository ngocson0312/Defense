using System.Collections;
using UnityEngine;
using MonsterLove.StateMachine;

namespace UDEV.GhostDefense
{
    public class AI : Actor
    {
        [HideInInspector]
        public bool isBoss;
        [SerializeField] private AIStat m_bossStat;
        [SerializeField] private float m_actionDist;
        [SerializeField] private float m_ultiDist;
        [SerializeField] private float m_dashDist;

        protected AIStat m_curStat;
        protected Vector3 m_targetDir;
        protected Player m_player;
        protected AIState m_prevState;
        protected float m_actionRate;
        protected StateMachine<AIState> m_fsm;
        protected float m_curAtkTime;
        protected float m_curDashTime;
        protected float m_curUltiTime;
        private bool m_isAtked;
        private bool m_isDashed;
        private bool m_isUltied;

        public StateMachine<AIState> Fsm { get => m_fsm; }

        protected bool IsDead
        {
            get => m_fsm.State == AIState.Dead || m_fsm.State == AIState.Dead;
        }

        protected bool IsAtking
        {
            get => m_fsm.State == AIState.Attack || m_fsm.State == AIState.Ultimate;
        }

        protected bool CanAction
        {
            get => Vector2.Distance(m_player.transform.position, transform.position) <= m_actionDist;
        }

        protected bool CanUlti
        {
            get => Vector2.Distance(m_player.transform.position, transform.position) <= m_ultiDist;
        }

        protected bool IsDashing
        {
            get => m_fsm.State == AIState.Dash1 || m_fsm.State == AIState.Dash2;
        }

        protected override void Awake()
        {
            base.Awake();
            FSMInit(this);
        }

        private void Start()
        {
            Init();
        }

        private void Update()
        {
            LimitHozMoving();

            if (!m_player)
            {
                m_player = GameManager.Ins.Player;
                return;
            }

            ActionHandle();

            if (m_healthBar)
            {
                m_healthBar.UpdateValue(m_curHp, m_curStat.CurHp);
            }
        }

        private void ActionHandle()
        {
            ReduceActionRate(ref m_isAtked, ref m_curAtkTime, m_curStat.atkTime);
            ReduceActionRate(ref m_isDashed, ref m_curDashTime, m_curStat.dashTime);
            ReduceActionRate(ref m_isUltied, ref m_curUltiTime, m_curStat.ultiTime);

            if (IsAtking || IsDashing || m_isKnockBack || IsDead || m_player.IsDead) return;

            GetTargetDir();

            if(CanUlti && m_actionRate <= m_curStat.CurUltiRate && !m_isUltied)
            {
                m_isUltied = true;
                ChangeState(AIState.Ultimate);
            }

            if (CanAction)
            {
                ActionSwitch();
            }

            if(m_targetDir.x > 0)
            {
                Flip(Direction.Right);
            }
            else
            {
                Flip(Direction.Left);
            }

            if (isBoss)
            {
                GUIManager.Ins.bossHpBar.UpdateValue(m_curHp, m_curStat.CurHp);
            }
        }

        protected void FSMInit(MonoBehaviour behaviour)
        {
            m_fsm = StateMachine<AIState>.Initialize(behaviour);
        }

        public override void Init()
        {
            m_player = GameManager.Ins.Player;
            m_isInvincible = false;
            m_isKnockBack = false;
            gameObject.layer = normalLayer;

            m_fsm.ChangeState(AIState.Walk);
            m_prevState = m_fsm.State;

            LoadStat();
            GetActionRate();

            CreateHealthBarUI();

            if (isBoss)
            {
                GUIManager.Ins.bossHpBar.Show(true);
                GUIManager.Ins.bossHpBar.UpdateValue(m_curHp, m_curStat.CurHp);
            }
        }

        private void LoadStat()
        {
            if (!stat) return;

            if (!isBoss)
            {
                m_curStat = (AIStat)stat;
            }
            else if (m_bossStat)
            {
                m_curStat = m_bossStat;
            }

            m_curSpeed = m_curStat.moveSpeed;
            m_curAtkTime = m_curStat.atkTime;
            m_curDashTime = m_curStat.dashTime;
            m_curUltiTime = m_curStat.ultiTime;
            m_curHp = m_curStat.CurHp;
            m_curDmg = m_curStat.CurDmg;
        }

        private void GetActionRate()
        {
            m_actionRate = UnityEngine.Random.Range(0f, 1f);
        }

        protected void ActionSwitch()
        {
            if(m_actionRate <= m_curStat.dashRate && m_actionRate > m_curStat.CurUltiRate)
            {
                if (m_isDashed) return;

                float randCheck = UnityEngine.Random.Range(0f, 1f);
                if(randCheck <= 0.5f)
                {
                    ChangeState(AIState.Dash1);
                }
                else
                {
                    ChangeState(AIState.Dash2);
                }
                m_isDashed = true;
            }else if(m_actionRate <= m_curStat.atkRate)
            {
                if (m_isAtked) return;
                m_isAtked = true;
                ChangeState(AIState.Attack);
            }
        }

        public override void Dash()
        {
            if (IsFacingLeft)
            {
                transform.position = new Vector3(transform.position.x - m_dashDist,
                    transform.position.y, transform.position.z
                    );
            }
            else
            {
                transform.position = new Vector3(transform.position.x + m_dashDist,
                    transform.position.y, transform.position.z
                    );
            }
        }

        public void ChangeState(AIState state)
        {
            m_prevState = m_fsm.State;
            m_fsm.ChangeState(state);
        }

        private IEnumerator ChangeStateDelayCo(AIState newState, float timeExtra = 0)
        {
            var animClip = Helper.GetClip(m_anim, m_fsm.State.ToString());
            if (animClip)
            {
                yield return new WaitForSeconds(animClip.length + timeExtra);
                if (!IsDead)
                {
                    ChangeState(newState);
                }
            }

            yield return null;
        }

        private void ChangeStateDalay(AIState newState, float timeExtra = 0)
        {
            StartCoroutine(ChangeStateDelayCo(newState, timeExtra));
        }

        public override void TakeDamage(float dmg, Actor whoHit)
        {
            if (IsDead) return;
            base.TakeDamage(dmg, whoHit);
            if(m_curHp > 0 && !m_isInvincible)
            {
                ChangeState(AIState.GotHit);
            }
        }

        protected override void Dead()
        {
            base.Dead();
            ChangeState(AIState.Dead);
        }

        protected void GetTargetDir()
        {
            m_targetDir = m_player.transform.position - transform.position;
            m_targetDir.Normalize();
        }

        #region FSM
        private void Walk_Enter() { }
        private void Walk_Update() {
            if (m_isAtked)
            {
                m_rb.velocity = Vector2.zero;
            }else
            {
                m_rb.velocity = new Vector2(m_targetDir.x * m_curSpeed, m_rb.velocity.y);
            }
            Helper.PlayAnim(m_anim, AIState.Walk.ToString());
        }
        private void Walk_Exit() { }
        private void Dash1_Enter() {
            gameObject.layer = invincibleLayer;
            ChangeStateDalay(AIState.Walk);
        }
        private void Dash1_Update() {
            Helper.PlayAnim(m_anim, AIState.Dash1.ToString());
        }
        private void Dash1_Exit() {
            gameObject.layer = normalLayer;
            GetActionRate();
        }
        private void Dash2_Enter() {
            gameObject.layer = invincibleLayer;
            ChangeStateDalay(AIState.Walk);
        }
        private void Dash2_Update() {
            Helper.PlayAnim(m_anim, AIState.Dash2.ToString());
        }
        private void Dash2_Exit() {
            gameObject.layer = normalLayer;
            GetActionRate();
        }
        private void Attack_Enter() { 
            ChangeStateDalay(AIState.Walk);
        }
        private void Attack_Update() {
            m_rb.velocity = Vector3.zero;
            Helper.PlayAnim(m_anim, AIState.Attack.ToString());
        }
        private void Attack_Exit() {
            GetActionRate();
        }
        private void GotHit_Enter() {
            GUIManager.Ins.dmgTxtMng.Add($"-{m_dmgTaked.ToString("f2")}", transform, "ai_damage");
        }
        private void GotHit_Update() {
            m_rb.velocity = Vector3.zero;
            KnockbackMove(0.15f);
            if (!m_isKnockBack)
            {
                ChangeState(AIState.Walk);
            }
            Helper.PlayAnim(m_anim, AIState.GotHit.ToString());
        }
        private void GotHit_Exit() { }
        private void Ultimate_Enter() {
            m_curDmg = m_curStat.CurDmg + m_curStat.CurDmg * 0.3f;
            ChangeStateDalay(AIState.Walk);
        }
        private void Ultimate_Update() {
            m_rb.velocity = Vector3.zero;
            Helper.PlayAnim(m_anim, AIState.Ultimate.ToString());
        }
        private void Ultimate_Exit() {
            GetActionRate();
            m_curDmg = m_curStat.CurDmg;
        }
        private void Dead_Enter() {
            m_player.AddEnergy(m_curStat.EnergyBonus);
            m_player.AddXp(m_curStat.XpBonus);

            GameManager.Ins?.AddEnemyKilled();

            GameManager.Ins?.SpawnCollectable(transform.position);

            AudioController.Ins.PlaySound(AudioController.Ins.enemyDead);
        }
        private void Dead_Update() {
            gameObject.layer = deadLayer;
            if (m_healthBar)
            {
                m_healthBar.Show(false);
            }
            Helper.PlayAnim(m_anim, AIState.Dead.ToString());
        }
        private void Dead_Exit() { }
        #endregion

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, 
                new Vector3(
                    transform.position.x + m_ultiDist,
                    transform.position.y, transform.position.z
                    )
                );

            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position,
                new Vector3(
                    transform.position.x + m_dashDist,
                    transform.position.y, transform.position.z
                    )
                );

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position,
                new Vector3(
                    transform.position.x + m_actionDist,
                    transform.position.y, transform.position.z
                    )
                );
        }
    }
}
