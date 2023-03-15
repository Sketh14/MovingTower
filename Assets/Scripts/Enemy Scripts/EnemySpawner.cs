using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moving_Tower
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private string[] enemyTags;
        [SerializeField] private EnemyHerdFormations_SO[] enemyHerdDetails;

        [Header("Spawn Controls")]
        [SerializeField] private List<Vector3> spawnPoints;
        [Range(1f, 5f)]
        [SerializeField] private float spawnInterval = 1f;
        private bool stopSpawn;

        [Header("Testing Variables")]
        [SerializeField] private byte spawnCount = 0, invokeCount = 0;

        private void OnEnable()
        {
            GameManager.instance.gameLogicReference.OnCastleReached += StopEnemySpawn;
        }

        private void OnDisable()
        {
            GameManager.instance.gameLogicReference.OnCastleReached -= StopEnemySpawn;
        }

        // Start is called before the first frame update
        private void Start()
        {
            Invoke("SpawnEnemy", spawnInterval);
        }

        private void SpawnEnemy()
        {
            int randomIndex = Random.Range(0, spawnPoints.Count);

            int spawnUnitIndex = Random.Range(0, 5);

            var enemy = EnemyPoolManager.instance.ReuseEnemy(enemyTags[spawnUnitIndex], spawnPoints[randomIndex], Quaternion.identity);
            enemy.GetComponent<EnemyController>().OnObjectSpawn((byte)randomIndex);

            if (!stopSpawn)
                Invoke("SpawnEnemy", spawnInterval);            //Invoke Spawn after a set time interval

            #region Testing
            //spawnCount++;
            //if (spawnCount < invokeCount)
            //    Invoke("SpawnEnemy", spawnInterval);            //Invoke Spawn after a set time interval
            #endregion Testing
        }

        private void StopEnemySpawn()
        {
            //stopSpawn = true;
        }
    }
}