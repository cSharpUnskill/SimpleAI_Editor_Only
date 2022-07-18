using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ziggurat
{
    public class GameManager : MonoBehaviour
    {
        public List<Bot> Bots = new List<Bot>();

        public List<ZigguratClass> _zigguratList = new List<ZigguratClass>();

        [HideInInspector]
        public List<Bot> RedBots = new List<Bot>();

        [HideInInspector]
        public List<Bot> GreenBots = new List<Bot>();

        [HideInInspector]
        public List<Bot> BlueBots = new List<Bot>();

        [HideInInspector]
        public List<Bot> KilledRedBots = new List<Bot>();

        [HideInInspector]
        public List<Bot> KilledGreenBots = new List<Bot>();

        [HideInInspector]
        public List<Bot> KilledBlueBots = new List<Bot>();

        [SerializeField]
        private Bot _botPrefab;

        [SerializeField]
        private Text _aliveRed;

        [SerializeField]
        private Text _aliveGreen;

        [SerializeField]
        private Text _aliveBlue;

        [SerializeField]
        private Text _deadRed;

        [SerializeField]
        private Text _deadGreen;

        [SerializeField]
        private Text _deadBlue;

        void Start()
        {
            foreach (ZigguratClass zig in _zigguratList)
            {
                StartCoroutine(Spawn(zig));
            }
        }

        void Update()
        {
            var listToKill = new List<Bot>();
            var killedRed = new List<Bot>();
            var killedGreen = new List<Bot>();
            var killedBlue = new List<Bot>();

            foreach (var bot in Bots)
            {
                if (bot.dead)
                {
                    if (bot.botColor == ColorType.RedGate) killedRed.Add(bot);
                    if (bot.botColor == ColorType.GreenGate) killedGreen.Add(bot);
                    if (bot.botColor == ColorType.BlueGate) killedBlue.Add(bot);

                    listToKill.Add(bot);
                    continue;
                }

                _aliveRed.text = RedBots.Count.ToString();
                _deadRed.text = KilledRedBots.Count.ToString();

                _aliveGreen.text = GreenBots.Count.ToString();
                _deadGreen.text = KilledGreenBots.Count.ToString();

                _aliveBlue.text = BlueBots.Count.ToString();
                _deadBlue.text = KilledBlueBots.Count.ToString();

                if (bot.enemyTarget != null)
                {
                    var attackPossible = Vector3.Distance(bot.transform.position, bot.enemyTarget.transform.position) < bot.attackRange;

                    var enemyInSight = Vector3.Distance(bot.transform.position, bot.enemyTarget.transform.position) < bot.sightRange;

                    if (!attackPossible && !enemyInSight) bot.Move(bot.pool.position);

                    if (!attackPossible && enemyInSight) bot.Move(bot.enemyTarget.transform.position);

                    if (attackPossible && enemyInSight) bot.Attack();
                }

                if (bot.enemyTarget == null || bot.enemyTarget.dead)
                {
                    bot.Move(bot.pool.position);

                    Bot potentialEnemy;

                    float sqrDistance = float.MaxValue;

                    foreach (Bot potentialTarget in Bots)
                    {
                        if (potentialTarget.botColor == bot.botColor) continue;

                        var distance = Vector3.Magnitude(potentialTarget.transform.position - bot.transform.position);
                        
                        if (distance < sqrDistance)
                        {
                            sqrDistance = distance;

                            potentialEnemy = potentialTarget;

                            if (sqrDistance < bot.sightRange)
                            {
                                bot.enemyTarget = potentialEnemy;
                            }

                            if (sqrDistance > bot.sightRange || bot.enemyTarget == null)
                            {
                                bot.Move(bot.pool.position);
                            }
                        }
                    }
                }
            }

            foreach (var botToKill in listToKill)
            {
                Bots.Remove(botToKill);
            }

            foreach (var redBot in killedRed)
            {
                RedBots.Remove(redBot);
                KilledRedBots.Add(redBot);
            }

            foreach (var greenBot in killedGreen)
            {
                GreenBots.Remove(greenBot);
                KilledGreenBots.Add(greenBot);
            }

            foreach (var blueBot in killedBlue)
            {
                BlueBots.Remove(blueBot);
                KilledBlueBots.Add(blueBot);
            }
        }

        public void ChangeBotsHealth(float health, ZigguratClass zigType)
        {
            foreach (var bot in Bots)
            {
                if (bot.botParent.colorType != zigType.colorType) continue;
                bot.SetHealth(health);
            }
        }

        public void ChangeBotsSpeed(float speed, ZigguratClass zigType)
        {
            foreach (var bot in Bots)
            {
                if (bot.botParent.colorType != zigType.colorType) continue;
                bot.SetSpeed(speed);
            }
        }

        public void ChangeBotsLDamage(float damage, ZigguratClass zigType)
        {
            foreach (var bot in Bots)
            {
                if (bot.botParent.colorType != zigType.colorType) continue;
                bot.SetLDamage(damage);
            }
        }

        public void ChangeBotsHDamage(float damage, ZigguratClass zigType)
        {
            foreach (var bot in Bots)
            {
                if (bot.botParent.colorType != zigType.colorType) continue;
                bot.SetHDamage(damage);
            }
        }

        public void ChangeBotsFastAttackRate(float rate, ZigguratClass zigType)
        {
            foreach (var bot in Bots)
            {
                if (bot.botParent.colorType != zigType.colorType) continue;
                bot.SetFastAttackRate(rate);
            }
        }

        public void ChangeBotsCritChance(float chance, ZigguratClass zigType)
        {
            foreach (var bot in Bots)
            {
                if (bot.botParent.colorType != zigType.colorType) continue;
                bot.SetCritChance(chance);
            }
        }

        public void ChangeBotsMissChance(float chance, ZigguratClass zigType)
        {
            foreach (var bot in Bots)
            {
                if (bot.botParent.colorType != zigType.colorType) continue;
                bot.SetMissChance(chance);
            }
        }

        public void DestroyBots()
        {
            foreach (var bot in Bots)
            {
                Destroy(bot.gameObject);
            }
            Bots.Clear();

            _aliveRed.text = RedBots.Count.ToString();
            _aliveGreen.text = GreenBots.Count.ToString();
            _aliveBlue.text = BlueBots.Count.ToString();
        }

        private IEnumerator Spawn(ZigguratClass zig)
        {
            while (zig.isSpawn == true)
            {
                var bot = Instantiate(_botPrefab, zig._spawnPoint.position, zig._spawnPoint.rotation, zig.transform);

                bot.botParent = zig;

                bot.botColor = zig.colorType;

                bot.botHealth = bot.botParent.health;

                bot.botSpeed = bot.botParent.speed;

                bot.botLightDamage = bot.botParent.lightDamage;

                bot.botHeavyDamage = bot.botParent.heavyDamage;

                bot.botAttackDelay = bot.botParent.fastAttackRate;

                bot.botCritChance = bot.botParent.critChance;

                bot.botMissChance = bot.botParent.missChance;

                Bots.Add(bot);

                if (zig.colorType == ColorType.RedGate)
                {
                    bot.meshShield.material.color = Color.red;

                    bot.meshSword.material.color = Color.red;

                    bot._skinnedMeshRenderer.material.color = Color.red;

                    RedBots.Add(bot);
                }

                if (zig.colorType == ColorType.GreenGate)
                {
                    bot.meshShield.material.color = Color.green;

                    bot.meshSword.material.color = Color.green;

                    bot._skinnedMeshRenderer.material.color = Color.green;

                    GreenBots.Add(bot);
                }

                if (zig.colorType == ColorType.BlueGate)
                {
                    bot.meshShield.material.color = Color.blue;

                    bot.meshSword.material.color = Color.blue; 

                    bot._skinnedMeshRenderer.material.color = Color.blue;

                    BlueBots.Add(bot);
                }

                yield return new WaitForSeconds(zig.spawnDelay);
            }

            if (zig.isSpawn == false)
            {
                yield return new WaitForSeconds(zig.spawnDelay);
                StartCoroutine(Spawn(zig));
            }
        }
    }
}
    

