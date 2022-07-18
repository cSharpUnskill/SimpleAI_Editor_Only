using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ziggurat
{
    public class SideMenuManager : MonoBehaviour
    {
        [HideInInspector]
        private Vector2 _startPosition;

        [HideInInspector]
        private Vector2 _endPosition;

        [HideInInspector]
        private bool _open = false;

        [HideInInspector]
        private Coroutine _sideCoroutine;

        [SerializeField]
        private float _fullTravelTime = 0.3f;

        [HideInInspector]
        private float _sizeDeltaX;

        [SerializeField]
        private GameObject _sidePreventingObstacle;

        [SerializeField]
        private GameManager _gameManager;

        void Start()
        {
            _sizeDeltaX = GetComponent<RectTransform>().sizeDelta.x;

            _startPosition = transform.position;

            _endPosition = transform.position;

            _endPosition.x -= _sizeDeltaX;
        }

        public void ClearStats_EditorEvent()
        {
            _gameManager.RedBots.Clear();           
            _gameManager.GreenBots.Clear();
            _gameManager.BlueBots.Clear();
            _gameManager.KilledRedBots.Clear();
            _gameManager.KilledGreenBots.Clear();
            _gameManager.KilledBlueBots.Clear();    
        }

        public void KillAll_EditorEvent()
        {
            ClearStats_EditorEvent();
            _gameManager.DestroyBots();
        }

        public void OpenSide_EditorEvent()
        {
            if (_sideCoroutine != null) StopCoroutine(_sideCoroutine);

            var modifier = _open ? _startPosition.x - transform.position.x : transform.position.x - _endPosition.x;

            modifier /= _sizeDeltaX;

            if (_open) _sidePreventingObstacle.SetActive(false);
            if (!_open) _sidePreventingObstacle.SetActive(true);

            _sideCoroutine = StartCoroutine(Lerp(transform, _open ? _startPosition : _endPosition, _fullTravelTime * modifier));

            _open = !_open;
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

            _sideCoroutine = null;
        }
    }
}
