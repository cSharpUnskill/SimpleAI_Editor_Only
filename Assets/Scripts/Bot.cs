using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using System;

namespace Ziggurat
{
    public class Bot : MonoBehaviour
    {
        [HideInInspector]
        public ColorType botColor;

        [HideInInspector]
        public ZigguratClass botParent;

        [SerializeField]
        public float botAttackDelay;

        [SerializeField]
        public float botCritChance;

        [SerializeField]
        public float botMissChance;

        [SerializeField]
        public float botHealth;

        [SerializeField]
        public float botSpeed;

        [SerializeField]
        public float botLightDamage;

        [SerializeField]
        public float botHeavyDamage;

        [HideInInspector]
        public NavMeshAgent agent;

        [HideInInspector]
        public Transform pool;

        public float sightRange;

        public float attackRange;

        public MeshRenderer meshShield;

        public MeshRenderer meshSword;

        [SerializeField]
        private GameObject _blockShield;

        [SerializeField]
        private GameObject _shield;

        [HideInInspector]
        public Bot enemyTarget;

        private Dictionary<Action, float> _actions;

        private bool _alreadyAttacked;

        private bool _immortal = false;

        private bool _firstAttack = true;

        [SerializeField]
        private Action CurrentAction;

        [SerializeField]
        private float LightAttack_Value;

        [SerializeField]
        private float HeavyAttack_Value;

        [SerializeField]
        private float Block_Value;

        [SerializeField]
        private Animator _animator;

        [SerializeField]
        private Collider _collider;

        [HideInInspector]
        public bool dead = false;

        [HideInInspector]
        public float attack;

        private bool _allowAttack = true;

        private List<Bot> _affectedEnemies = new List<Bot>();

        public event EventHandler OnEndAnimation;

        [SerializeField]
        public SkinnedMeshRenderer _skinnedMeshRenderer;

        public void Moving(float direction)
        {
            _animator.SetFloat("Movement", direction);
        }

        public void StartAnimation(string key)
        {
            _animator.SetFloat("Movement", 0f);
            _animator.SetTrigger(key);
        }

        private void AnimationEventCollider_UnityEditor(int isActivity)
        {
            _collider.enabled = isActivity != 0;
        }

        private void AnimationEventEnd_UnityEditor(string result)
        {
            if (result == "die") Destroy(gameObject);
            OnEndAnimation?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// ///////////////////////////////////////////////
        /// </summary>
        void Start()
        {
            pool = GameObject.Find("Pool").transform;

            agent = GetComponent<NavMeshAgent>();

            agent.speed = botSpeed;

            Move(pool.position);

            _actions = new Dictionary<Action, float>() { { Action.lightAttack, 1 }, { Action.heavyAttack, 1 }, { Action.block, 1 } };

            LightAttack_Value = _actions[Action.lightAttack];

            HeavyAttack_Value = _actions[Action.heavyAttack];

            Block_Value = _actions[Action.block];
        }

        void OnTriggerEnter(Collider col)
        {
            var enemy = col.GetComponent<Bot>();

            if (enemy && enemy.botColor != botColor)
            {
                if (_affectedEnemies.Contains(enemy)) return;

                _affectedEnemies.Add(enemy);

                DoDamage();
            }
        }

        public void Move(Vector3 target)
        {
            Moving(1);

            _animator.speed = 1;

            agent.SetDestination(target);

            if (Vector3.Distance(transform.position, target) < attackRange)
            {
                agent.SetDestination(transform.position);
            }

            if (Vector3.Distance(transform.position, target) < 4f)
            {
                Moving(0);
                StartAnimation("Walk");
            }
        }

        public void Attack()
        {
            if (!_allowAttack) return;

            _affectedEnemies.Clear();

            agent.SetDestination(transform.position);

            transform.LookAt(enemyTarget.transform);

            if (!_alreadyAttacked)
            {
                if (!_firstAttack)
                {
                    CurrentAction = _actions.OrderByDescending(x => x.Value).First().Key;

                    if (CurrentAction == Action.lightAttack) LightAttack();
                    if (CurrentAction == Action.heavyAttack) HeavyAttack();
                    if (CurrentAction == Action.block) Block();

                    _alreadyAttacked = true;
                    Invoke(nameof(ResetAttack), botAttackDelay);
                }

                if (_firstAttack)
                {
                    var firstAttack = (UnityEngine.Random.value > 0.5f);

                    if (firstAttack)
                    {
                        CurrentAction = Action.lightAttack;
                        LightAttack();
                    }

                    if (!firstAttack)
                    {
                        CurrentAction = Action.heavyAttack;
                        HeavyAttack();
                    }

                    _alreadyAttacked = true;
                    Invoke(nameof(ResetAttack), botAttackDelay);
                    _firstAttack = false;
                }
            }
        }

        public void ResetAttack()
        {
            _animator.speed = 1f;
            _alreadyAttacked = false;
        }

        public void DoDamage()
        {
            if (enemyTarget.dead) return;
            enemyTarget.TakeDamage(attack);
        }

        public void TakeDamage(float damage)
        {
            if (_immortal) return;

            _animator.speed = 1f;

            botHealth -= damage;

            if (botHealth > 0) StartAnimation("Impact");

            if (botHealth <= 0)
            {
                StartAnimation("Die");
                dead = true;
            }
        }

        void LightAttack()
        {
            _immortal = false;

            _animator.speed = (1f / botAttackDelay);

            if (_animator.speed <= 0) _animator.speed = 1f;

            StartAnimation("Fast");

            var critChance = UnityEngine.Random.Range(0, 100) <= botCritChance ? true : false;
            var missChance = UnityEngine.Random.Range(0, 100) <= botMissChance ? true : false;

            if (!missChance)
            {
                if (!enemyTarget._immortal)
                {
                    if (!critChance) attack = botLightDamage;

                    if (critChance) attack = botLightDamage * 2f;

                    UpdateDictionary(1.5f, 3f, 3f);
                }
            }
        }

        void HeavyAttack()
        {
            _immortal = false;

            _animator.speed = (1f / botAttackDelay);

            if (_animator.speed <= 0) _animator.speed = 1f;

            StartAnimation("Strong");
            
            var critChance = UnityEngine.Random.Range(0, 100) <= botCritChance ? true : false;
            var missChance = UnityEngine.Random.Range(0, 100) <= botMissChance ? true : false;

            if (!missChance)
            {
                if (!enemyTarget._immortal)
                {
                    if (!critChance) attack = botHeavyDamage;

                    if (critChance) attack = botHeavyDamage * 2f;

                    UpdateDictionary(3f, 1.5f, 3f);
                }
            }
        }

        void Block()
        {
            _immortal = true;

            _animator.speed = (1f / botAttackDelay);

            if (_animator.speed <= 0) _animator.speed = 1;

            StopAllCoroutines();
            StartCoroutine(BlockAnimation());

            UpdateDictionary(3f, 3f, 1.5f);
        }

        void UpdateDictionary(float light, float heavy, float block)
        {
            _actions[Action.lightAttack] += UnityEngine.Random.Range(1f, light);
            LightAttack_Value = _actions[Action.lightAttack];

            _actions[Action.heavyAttack] += UnityEngine.Random.Range(1f, heavy);
            HeavyAttack_Value = _actions[Action.heavyAttack];

            _actions[Action.block] += UnityEngine.Random.Range(1f, block);
            Block_Value = _actions[Action.block];
        }

        IEnumerator BlockAnimation()
        {
            _allowAttack = false;
            _blockShield.SetActive(true);
            _shield.SetActive(false);
            yield return new WaitForSeconds(botAttackDelay);
            _blockShield.SetActive(false);
            _shield.SetActive(true);
            _allowAttack = true;
        }

        void ResetActions()
        {
            _actions = new Dictionary<Action, float>() { { Action.lightAttack, 1 }, { Action.heavyAttack, 1 }, { Action.block, 1 } };
        }

        public void SetHealth(float health)
        {
            botHealth = health;
        }

        public void SetSpeed(float speed)
        {
            agent.speed = speed;
        }

        public void SetLDamage(float damage)
        {
            botLightDamage = damage;
        }

        public void SetHDamage(float damage)
        {
            botHeavyDamage = damage;
        }

        public void SetFastAttackRate(float rate)
        {
            botAttackDelay = rate;
        }

        public void SetCritChance(float chance)
        {
            botCritChance = chance;
        }

        public void SetMissChance(float chance)
        {
            botMissChance = chance;
        }
    }

    public enum Action
    {
        lightAttack,

        heavyAttack,

        block
    }
}

