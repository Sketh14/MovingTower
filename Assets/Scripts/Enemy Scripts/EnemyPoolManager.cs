using System.Collections.Generic;
using UnityEngine;

namespace Moving_Tower
{
    public class EnemyPoolManager : MonoBehaviour
    {
        [System.Serializable]
        public class Enemy
        {
            public Enemy_SO enemyStats;
            public GameObject enemyPrefab;
            public EnemyHerdFormations_SO enemyHerdFormations;
        }

        public List<Enemy> enemiesList;
        public Dictionary<string, Queue<GameObject>> enemiesDictionary;
        [SerializeField] private int maxEnemyCount;

        private static EnemyPoolManager _instance;
        public static EnemyPoolManager instance { get { return _instance; } }

        #region Singleton
        void Awake()
        {
            if (instance != null && instance != this)
                Destroy(this);
            else
                _instance = this;

            AllocatePool();
        }
        #endregion Singleton

        private void AllocatePool()
        {
            enemiesDictionary = new Dictionary<string, Queue<GameObject>>();

            
            foreach (Enemy enemy in enemiesList)
            {
                GameObject poolHolder = new GameObject("EnemyPool_" + enemy.enemyStats.tag);
                poolHolder.transform.parent = transform;

                Queue<GameObject> enemyQueue = new Queue<GameObject>();

                //Change this so that the Enemies are instantiated at each level and not the same quantity for every level
                for (int i = 0; i < maxEnemyCount; i++)
                {
                    GameObject tempEnemy = Instantiate(enemy.enemyPrefab, poolHolder.transform);
                    tempEnemy.name = enemy.enemyStats.tag + i;
                    tempEnemy.gameObject.SetActive(false);

                    enemyQueue.Enqueue(tempEnemy);
                }

                enemiesDictionary.Add(enemy.enemyStats.tag, enemyQueue);
            }
        }

        public GameObject ReuseEnemy(string tag, Vector3 position, Quaternion rotation)
        {
            if (!enemiesDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"Pool With Tag {tag} does not exist");
                return null;
            }

            GameObject tempEnemy = enemiesDictionary[tag].Dequeue();

            tempEnemy.SetActive(true);
            tempEnemy.transform.position = position;
            tempEnemy.transform.rotation = rotation;

            enemiesDictionary[tag].Enqueue(tempEnemy);

            return tempEnemy;
        }
    }
}
