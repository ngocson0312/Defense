using System.Collections;
using UnityEngine;
using MonsterLove.StateMachine;

namespace UDEV.GhostDefense
{
    public class Player : Actor
    {
        [Header("Collider:")]
        [SerializeField] private Collider2D m_headCol;
        [SerializeField] private Collider2D m_bodyCol;
        [SerializeField] private Collider2D m_deadCol;

        private PlayerStat m_curStat;
        private StateMachine<PlayerState> m_fsm;
        private PlayerState m_prevState;
        private int m_hozDir, m_vertDir;
        private bool m_isDashed;
        private bool m_isAttacked;
        private float m_curDashRate;
        private float m_curAttackRate;
        private float m_curEnergy;

        public PlayerStat CurStat { get => m_curStat; set => m_curStat = value; }
        public StateMachine<PlayerState> Fsm { get => m_fsm;}
        public bool IsDead
        {
            get => m_fsm.State == PlayerState.Dead || m_prevState == PlayerState.Dead;
        }

        public bool IsAttacking
        {
            get => m_fsm.State == PlayerState.Attack || m_fsm.State == PlayerState.Ultimate;
        }

        public bool IsUlti
        {
            get => m_fsm.State == PlayerState.Ultimate;
        }

        public bool IsDashing
        {
            get => m_fsm.State == PlayerState.Dash;
        }
        public float CurEnergy { get => m_curEnergy; set => m_curEnergy = value; }

        protected override void Awake()
        {
            base.Awake();
            m_fsm = StateMachine<PlayerState>.Initialize(this);
            if (stat)
            {
                m_curStat = (PlayerStat)stat;
            }
        }

        private void Start()
        {
            Init();
        }

        private void Update()
        {
            LimitHozMoving();
            if(m_isKnockBack)
            {
                float mapSpeed = m_isFacingLeft ? m_curStat.knockbackForce : -m_curStat.knockbackForce;
                GameManager.Ins.SetMapSpeed(mapSpeed);
            }
            ActionHandle();
        }

        public override void Init()
        {
            LoadStat();

            m_fsm.ChangeState(PlayerState.Idle);
            m_prevState = PlayerState.Idle;
        }

        public void LoadStat()
        {
            if (!m_curStat) return;

            m_curStat.Load(GameData.Ins.curPlayerId);
            m_curSpeed = m_curStat.moveSpeed;
            m_curHp = m_curStat.hp;
            m_curDmg = m_curStat.damage;
            m_curDashRate = m_curStat.dashRate;
            m_curAttackRate = m_curStat.atkRate;
            m_curEnergy = m_curStat.ultiEnergy;
        }

        private void ActionHandle()
        {
            if (IsAttacking || IsDashing || m_isKnockBack || IsDead) return;

            if (GamepadController.Ins.IsStatic)
            {
                m_curSpeed = 0f;
                m_rb.velocity = new Vector2(0f, m_rb.velocity.y);
                if (!m_isInvincible)
                {
                    GameManager.Ins.SetMapSpeed(0);
                    ChangeState(PlayerState.Idle);
                }
            }
            else
            {
                if (GamepadController.Ins.CanAttack)
                {
                    if (!m_isAttacked)
                    {
                        m_isAttacked = true;
                        ChangeState(PlayerState.Attack);
                    }
                }else if(GamepadController.Ins.CanUlti && m_curEnergy >= m_curStat.ultiEnergy)
                {
                    ChangeState(PlayerState.Ultimate);
                }
            }

            ReduceActionRate(ref m_isDashed, ref m_curDashRate, m_curStat.dashRate);
            ReduceActionRate(ref m_isAttacked, ref m_curAttackRate, m_curStat.atkRate);

            GUIManager.Ins.atkBtnFilled.UpdateValue(m_curAttackRate, m_curStat.atkRate);
            GUIManager.Ins.dashBtnFilled.UpdateValue(m_curDashRate, m_curStat.dashRate);
            GUIManager.Ins.ultiBtnFilled.UpdateValue(m_curEnergy, m_curStat.ultiEnergy);
        }

        private void Move(Direction dir)
        {
            if (m_isKnockBack) return;

            if(dir == Direction.Left || dir == Direction.Right)
            {
                Flip(dir);

                m_hozDir = dir == Direction.Left ? -1 : 1;

                if (GameManager.Ins.setting.isOnMobile)
                {
                    m_rb.velocity = new Vector2(GamepadController.Ins.joystick.xValue * m_curSpeed, m_rb.velocity.y);
                }else
                {
                    m_rb.velocity = new Vector2(m_hozDir * m_curSpeed, m_rb.velocity.y);
                }

                if (CameraFollow.Ins.IsHozStuck)
                {
                    GameManager.Ins.SetMapSpeed(0);
                }else
                {
                    GameManager.Ins.SetMapSpeed(-m_hozDir * m_curSpeed);
                }
            }

        }

        public override void Dash()
        {
            if (IsFacingLeft)
            {
                transform.position = new Vector3(transform.position.x - m_curStat.dashDist,
                    transform.position.y, transform.position.z
                    );
            }else
            {
                transform.position = new Vector3(transform.position.x + m_curStat.dashDist,
                    transform.position.y, transform.position.z
                    );
            }
        }

        public void ChangeState(PlayerState state)
        {
            m_prevState = m_fsm.State;
            m_fsm.ChangeState(state);
        }

        private IEnumerator ChangeStateDelayCo(PlayerState newState, float timeExtra = 0)
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

        private void ChangeStateDalay(PlayerState newState, float timeExtra = 0)
        {
            StartCoroutine(ChangeStateDelayCo(newState, timeExtra));
        }

        private void ActiveCol(PlayerCollider collider)
        {
            if (m_headCol)
            {
                m_headCol.enabled = collider == PlayerCollider.Normal;
            }

            if (m_bodyCol)
            {
                m_bodyCol.enabled = collider == PlayerCollider.Normal;
            }

            if (m_deadCol)
            {
                m_deadCol.enabled = collider == PlayerCollider.Dead;
            }
        }

        protected override void Dead()
        {
            base.Dead();
            ChangeState(PlayerState.Dead);
        }

        public override void TakeDamage(float dmg, Actor whoHit)
        {
            if (IsDead || IsUlti) return;
            base.TakeDamage(dmg - m_curStat.defense, whoHit);
            if(m_curHp > 0 && !m_isInvincible)
            {
                ChangeState(PlayerState.GotHit);
            }

            GUIManager.Ins.hpBar.UpdateValue(m_curHp, m_curStat.hp);
        }

        public void AddXp(float xp)
        {
            m_curStat.xp += xp;

            StartCoroutine(m_curStat.LevelUpCo(
                () =>
                {
                    m_curHp = m_curStat.hp;

                    GUIManager.Ins.dmgTxtMng.Add("Level Up", transform, "level_up");
                    GUIManager.Ins.UpdateHeroLevel(m_curStat.playerLevel);
                    GUIManager.Ins.UpdateHeroPoint(m_curStat.point);
                    GUIManager.Ins.hpBar.UpdateValue(m_curHp, m_curStat.hp);

                    AudioController.Ins.PlaySound(AudioController.Ins.levelUp);
                }
                ));
        }

        public void AddEnergy(float energyBonus)
        {
            m_curEnergy += energyBonus;
            GUIManager.Ins.ultiBtnFilled.UpdateValue(m_curEnergy, m_curStat.ultiEnergy);
            GUIManager.Ins.energyBar.UpdateValue(m_curEnergy, m_curStat.ultiEnergy);
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag(GameTag.Collectable.ToString()))
            {
                Collectable collectable = col.gameObject.GetComponent<Collectable>();
                if (!collectable) return;
                collectable.Trigger();
            }
        }

        #region
        private void Idle_Enter() { 
            ActiveCol(PlayerCollider.Normal);
        }
        private void Idle_Update() {
            if(GamepadController.Ins.CanMoveLeft || GamepadController.Ins.CanMoveRight)
            {
                ChangeState(PlayerState.Walk);
            }

            if (IsDead)
            {
                ChangeState(PlayerState.Dead);
            }

            Helper.PlayAnim(m_anim, PlayerState.Idle.ToString());
        }
        private void Idle_Exit() { }
        private void Walk_Enter() {
            m_curSpeed = m_curStat.moveSpeed;
        }
        private void Walk_Update() {
            if (GamepadController.Ins.CanDash)
            {
                if (m_isDashed) return;
                m_isDashed = true;
                ChangeState(PlayerState.Dash);
            }
            else
            {
                m_curSpeed += Time.deltaTime * 1.5f;
                m_curSpeed = Mathf.Clamp(m_curSpeed, m_curStat.moveSpeed, m_curStat.runSpeed);
                if(m_curSpeed >= m_curStat.runSpeed)
                {
                    Helper.PlayAnim(m_anim, PlayerState.Run.ToString());
                }
                else
                {
                    Helper.PlayAnim(m_anim, PlayerState.Walk.ToString());
                }
            }

            if(GamepadController.Ins.CanMoveLeft)
            {
                Move(Direction.Left);
            }else if (GamepadController.Ins.CanMoveRight)
            {
                Move(Direction.Right);
            }
        }
        private void Walk_Exit() { }
        private void Run_Enter() { }
        private void Run_Update() {
            Helper.PlayAnim(m_anim, PlayerState.Run.ToString());
        }
        private void Run_Exit() { }
        private void Dash_Enter() {
            gameObject.layer = invincibleLayer;
            ChangeStateDalay(PlayerState.Idle);
        }
        private void Dash_Update() {
            Helper.PlayAnim(m_anim, PlayerState.Dash.ToString());
        }
        private void Dash_Exit() { 
            gameObject.layer = normalLayer;
        }
        private void Attack_Enter() {
            ChangeStateDalay(PlayerState.Idle);

            AudioController.Ins.PlaySound(AudioController.Ins.attack);
        }
        private void Attack_Update() {
            Helper.PlayAnim(m_anim, PlayerState.Attack.ToString());
        }
        private void Attack_Exit() { }
        private void Ultimate_Enter() {
            m_curEnergy -= m_curStat.ultiEnergy;
            m_curDmg = m_curStat.damage + m_curStat.damage * 0.3f;
            ChangeStateDalay(PlayerState.Idle);

            GUIManager.Ins.energyBar.UpdateValue(m_curEnergy, m_curStat.ultiEnergy);
        }
        private void Ultimate_Update() {
            Helper.PlayAnim(m_anim, PlayerState.Ultimate.ToString());
        }
        private void Ultimate_Exit() {
            m_curDmg = m_curStat.damage;
        }
        private void GotHit_Enter() {
            AIStat aiStat = (AIStat)m_whoHit.stat;
            AddEnergy(aiStat.EnergyBonus / 5);

            AudioController.Ins.PlaySound(AudioController.Ins.gotHit);
        }
        private void GotHit_Update() {
            KnockbackMove(0.2f);
            if (!m_isKnockBack)
            {
                ChangeState(PlayerState.Idle);
            }
            Helper.PlayAnim(m_anim, PlayerState.GotHit.ToString());
        }
        private void GotHit_Exit() {
            
        }
        private void Dead_Enter() {
            ActiveCol(PlayerCollider.Dead);
            GameManager.Ins.ReviveChecking();
            CamShake.Ins.ShakeTrigger(0.2f, 0.2f);

            AudioController.Ins.PlaySound(AudioController.Ins.dead);
        }
        private void Dead_Update() {
            gameObject.layer = deadLayer;
            GameManager.Ins.SetMapSpeed(0);
            Helper.PlayAnim(m_anim, PlayerState.Dead.ToString());
        }
        private void Dead_Exit() { }
        #endregion
    }
}
