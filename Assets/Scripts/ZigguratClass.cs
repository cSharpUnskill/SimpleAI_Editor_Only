using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ziggurat
{
    public class ZigguratClass : MonoBehaviour
    {
        [HideInInspector]
        public ColorType colorType;
        
        [Range(1f, 5f)]
        public float spawnDelay;

        [Range(0.5f, 2f)]
        public float fastAttackRate;

        [Range(1f, 100f)]
        public float critChance;

        [Range(1f, 100f)]
        public float missChance;

        [Range(10f, 50f)]
        public float health;

        [Range(1f, 5f)]
        public float speed;

        [Range(1f, 5f)]
        public float lightDamage;

        [Range(6f, 10f)]
        public float heavyDamage;

        [SerializeField]
        private GameObject _bot;

        [HideInInspector]
        public Transform _spawnPoint;

        [SerializeField]
        private GameManager _gameManager;

        [HideInInspector]
        public bool isSpawn;

        [SerializeField]
        public GameObject spawnWalls;

        [HideInInspector]
        public bool isSpawnWalls;

        void Awake()
        {
            if (transform.tag == "RedGate") colorType = ColorType.RedGate;
            if (transform.tag == "GreenGate") colorType = ColorType.GreenGate;
            if (transform.tag == "BlueGate") colorType = ColorType.BlueGate;

            spawnDelay = 1f;
            health = 50f;
            speed = 5f;
            lightDamage = 1f;
            heavyDamage = 6f;
            fastAttackRate = 1f;
            critChance = 20f;
            missChance = 10f;

            _spawnPoint = this.gameObject.transform.Find("Spawn");

            isSpawn = true;

            isSpawnWalls = false;
        }
    }

    public enum ColorType
    {
        RedGate,

        GreenGate,

        BlueGate,
    }
}
