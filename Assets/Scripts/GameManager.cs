using System;
using System.Collections.Generic;
using UnityEngine;

namespace Moving_Tower
{
    public class GameManager : MonoBehaviour
    {
        public List<WayPoints_SO> wayPointsList;
        public List<Transform> activeEnemies;          //Can be made to array for quicker access

        public float currentBulletDamage;
        public bool gameStarted, totalHerdsSpawned, enemyReachedCastle;
        public int currentWave = 1, tokenCollected;
        public int[] upgradePrices;
        //[Header("Test Variable")]
        //public Transform activeTarget;

        [Header ("Reference Scripts")]
        public GameLogic gameLogicReference;
        public EnemySpawner enemySpawnerReference;

        private static GameManager _instance;
        public static GameManager instance
        {
            get { return _instance; }
        }

        private void OnEnable()
        {
            gameLogicReference.OnEnemyKilled += RemoveEnemies;
            gameLogicReference.OnCastleReached += UpdateEnemyStats;
        }

        private void OnDisable()
        {
            gameLogicReference.OnEnemyKilled -= RemoveEnemies;
            gameLogicReference.OnCastleReached -= UpdateEnemyStats;
        }

        // Start is called before the first frame update
        void Start()
        {
            if (_instance != null && _instance != this)
                Destroy(this);
            else
                _instance = this;

            enemySpawnerReference.enabled = true;
            GameManager.instance.currentWave = 1; 
        }

        // So that the rest enemy units just rush towards the castle
        private void UpdateEnemyStats()
        {
            foreach (Transform t in activeEnemies)
            {
                EnemyController controller = t.GetComponent<EnemyController>();
                controller.speedMult = 8;
                controller.waypointDiff = 1f;
            }
        }

        private void RemoveEnemies(Transform enemy)
        {
            foreach(Transform t in activeEnemies)
            {
                if (t.Equals(enemy))
                {
                    activeEnemies.Remove(t);

                    if (totalHerdsSpawned && activeEnemies.Count == 0)
                    {
                        gameLogicReference.OnWaveCompletion?.Invoke();
                        totalHerdsSpawned = false;
                    }
                    return;
                }
            }
        }
    }
}