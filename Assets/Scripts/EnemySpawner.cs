using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moving_Tower
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject enemyPrefab;

        [Header("Spawn Controls")]
        [SerializeField] private List<Vector3> spawnPoints;
        [Range(1f, 5f)]
        [SerializeField] private float spawnInterval = 1f;
        [SerializeField] private byte spawnCount = 0, invokeCount = 0;

        // Start is called before the first frame update
        private void Start()
        {
            Invoke("SpawnEnemy", spawnInterval);
        }

        private void SpawnEnemy()
        {
            spawnCount++;
            int randomIndex = Random.Range(0, spawnPoints.Count);

            var enemy = EnemyPoolManager.instance.ReuseEnemy(spawnPoints[randomIndex], Quaternion.identity);
            enemy.GetComponent<EnemyController>().SetWayPoints((byte)randomIndex);

            if (spawnCount < invokeCount)
                Invoke("SpawnEnemy", spawnInterval);            //Invoke Spawn after a set time interval
        }
    }
}