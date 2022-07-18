using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ziggurat
{
    public class TopMenuManager : MonoBehaviour
    {
        [HideInInspector]
        private Vector2 _startPosition;

        [HideInInspector]
        private Vector2 _endPosition;

        [HideInInspector]
        private bool _open = false;

        [HideInInspector]
        private Coroutine _topCoroutine;

        [SerializeField]
        private float _fullTravelTime = 0.5f;

        [HideInInspector]
        private float _sizeDeltaY;

        [SerializeField]
        private Slider _spawn;

        [SerializeField]
        private Transform _spawnDisplay;

        [SerializeField]
        private Slider _fastAttackRate;

        [SerializeField]
        private Transform _fastAttackRateDisplay;

        [SerializeField]
        private Slider _critChance;

        [SerializeField]
        private Transform _critChanceDisplay;

        [SerializeField]
        private Slider _missChance;

        [SerializeField]
        private Transform _missChanceDisplay;

        [SerializeField]
        private InputField _health;

        [SerializeField]
        private InputField _speed;

        [SerializeField]
        private InputField _lightDamage;

        [SerializeField]
        private InputField _heavyDamage;

        [SerializeField]
        public ZigguratClass _ziggurat;

        [SerializeField]
        private Transform _currentZig;

        [SerializeField]
        private GameObject _preventingObstacle;

        [SerializeField]
        private Toggle _isSpawnToggle;

        [SerializeField]
        private Toggle _isSpawnWallsToggle;

        [SerializeField]
        private GameManager _gameManager;

        void Start()
        {
            _sizeDeltaY = GetComponent<RectTransform>().sizeDelta.y;

            _startPosition = transform.position;

            _endPosition = transform.position;

            _endPosition.y -= _sizeDeltaY;
        }

        public void ZigguratClick(ZigguratClass zig)
        {
            if(_ziggurat == null) 
            {
                ChangeParameters(zig);

                OpenTop_EditorEvent(false);

                _preventingObstacle.SetActive(true);
            } 

            else if (_ziggurat.colorType != zig.colorType)
            {
                ChangeParameters(zig);
            }

            else
            {
                OpenTop_EditorEvent(true);

                _preventingObstacle.SetActive(false);
            }
        }
            
        public void OpenTop_EditorEvent(bool open)
        {
            if (open) _ziggurat = null;
            
            if (_topCoroutine != null) StopCoroutine(_topCoroutine);
            
            var modifier = open ? _startPosition.y - transform.position.y : _endPosition.y - transform.position.y;

            modifier /= _sizeDeltaY;

            if (!_open) _preventingObstacle.SetActive(false);

            _topCoroutine = StartCoroutine(Lerp(transform, open ? _startPosition : _endPosition, _fullTravelTime * modifier));

            _open = open;
        }

        void ChangeParameters(ZigguratClass ziggurat)
        {
            _ziggurat = ziggurat;

            _spawn.value = ziggurat.spawnDelay;

            _spawnDisplay.GetComponent<Text>().text = ziggurat.spawnDelay.ToString();

            _fastAttackRate.value = ziggurat.fastAttackRate;

            _fastAttackRateDisplay.GetComponent<Text>().text = ziggurat.fastAttackRate.ToString("F1");

            _critChance.value = ziggurat.critChance;

            _critChanceDisplay.GetComponent<Text>().text = ziggurat.critChance.ToString("F0");

            _missChance.value = ziggurat.missChance;

            _missChanceDisplay.GetComponent<Text>().text = ziggurat.missChance.ToString("F0");

            _health.text = _ziggurat.health.ToString();

            _speed.text = _ziggurat.speed.ToString();

            _lightDamage.text = _ziggurat.lightDamage.ToString();

            _heavyDamage.text = _ziggurat.heavyDamage.ToString();

            _isSpawnToggle.isOn = _ziggurat.isSpawn;

            if (_isSpawnToggle.isOn == true) _isSpawnToggle.GetComponentInChildren<Text>().text = "включен";

            if (_isSpawnToggle.isOn == false) _isSpawnToggle.GetComponentInChildren<Text>().text = " выключен";

            _isSpawnWallsToggle.isOn = _ziggurat.isSpawnWalls;

            if (_isSpawnWallsToggle.isOn == true) _isSpawnWallsToggle.GetComponentInChildren<Text>().text = "включены";

            if (_isSpawnWallsToggle.isOn == false) _isSpawnWallsToggle.GetComponentInChildren<Text>().text = " выключены";

            if (_ziggurat.colorType == ColorType.RedGate)
            {
                _currentZig.GetComponent<Text>().color = Color.red;

                _currentZig.GetComponent<Text>().text = "Красный";
            }

            if (_ziggurat.colorType == ColorType.GreenGate)
            {
                _currentZig.GetComponent<Text>().color = Color.green;

                _currentZig.GetComponent<Text>().text = "Зеленый";
            }

            if (_ziggurat.colorType == ColorType.BlueGate)
            {
                _currentZig.GetComponent<Text>().color = Color.blue;

                _currentZig.GetComponent<Text>().text = "Синий";
            }
        }

        public void SpawnMode_EditorEvent(bool b)
        {
            _ziggurat.isSpawn = b;

            if (b == true) _isSpawnToggle.GetComponentInChildren<Text>().text = "включен";

            if (b == false) _isSpawnToggle.GetComponentInChildren<Text>().text = " выключен";
        }

        public void WallsMode_EditorEvent(bool b)
        {
            if (b == true)
            {
                _isSpawnWallsToggle.GetComponentInChildren<Text>().text = "включены";
                _ziggurat.spawnWalls.SetActive(true);
                _ziggurat.isSpawnWalls = true;
            }

            if (b == false)
            {
                _isSpawnWallsToggle.GetComponentInChildren<Text>().text = " выключены";
                _ziggurat.spawnWalls.SetActive(false);
                _ziggurat.isSpawnWalls = false;
            }
        }

        public void SpawnDelay_EditorEvent(float t)
        {
            _ziggurat.spawnDelay = t;

            _spawnDisplay.GetComponent<Text>().text = _ziggurat.spawnDelay.ToString();
        }

        public void FastAttackRate_EditorEvent(float t)
        {
            _ziggurat.fastAttackRate = t;

            _fastAttackRateDisplay.GetComponent<Text>().text = _ziggurat.fastAttackRate.ToString("F1");

            _gameManager.ChangeBotsFastAttackRate(t, _ziggurat);
        }

        public void CritChance_EditorEvent(float t)
        {
            _ziggurat.critChance = t;

            _critChanceDisplay.GetComponent<Text>().text = _ziggurat.critChance.ToString("F0");

            _gameManager.ChangeBotsCritChance(t, _ziggurat);
        }

        public void MissChance_EditorEvent(float t)
        {
            _ziggurat.missChance = t;

            _missChanceDisplay.GetComponent<Text>().text = _ziggurat.missChance.ToString("F0");

            _gameManager.ChangeBotsMissChance(t, _ziggurat);
        }

        public void Health_EditorEvent(string t)
        {
            var r = int.Parse(t);

            if(r > 50) r = 50;

            if(r < 10) r = 10;

            _ziggurat.health = r;

            _health.text = r.ToString();

            _gameManager.ChangeBotsHealth(r, _ziggurat);
        }

        public void Speed_EditorEvent(string t)
        {
            var r = int.Parse(t);

            if(r > 5) r = 5;

            if(r < 1) r = 1;

            _ziggurat.speed = r;

            _speed.text = r.ToString();

            _gameManager.ChangeBotsSpeed(r, _ziggurat);
        }

        public void LightAttack_EditorEvent(string t)
        {
            var r = int.Parse(t);

            if (r > 5) r = 5;
            
            if (r < 1) r = 1;

            _ziggurat.lightDamage = r;

            _lightDamage.text = r.ToString();

            _gameManager.ChangeBotsLDamage(r, _ziggurat);
        }

        public void HeavyAttack_EditorEvent(string t)
        {
            var r = int.Parse(t);

            if (r > 10) r = 10;

            if (r < 6) r = 6;

            _ziggurat.heavyDamage = r;

            _heavyDamage.text = r.ToString();

            _gameManager.ChangeBotsHDamage(r, _ziggurat);
        }

        private IEnumerator Lerp(Transform obj, Vector2 target, float TravelTime)
        {
            Vector2 startPosition = obj.position;

            float t = 0;

            while (t < 1)
            {
                obj.position = Vector2.Lerp(startPosition, target, t * t);

                t += Time.deltaTime / TravelTime;

                yield return null;
            }

            obj.position = target;

            _topCoroutine = null;
        }
    }
}
