#define TEST_MODE

using System.Collections.Generic;
using UnityEngine;

namespace Moving_Tower
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private string[] enemyTags;
        [SerializeField] private EnemyHerdFormations_SO[] enemyHerdFormations;

        [Header("Local Reference Scritps")]
        [SerializeField] private GameLogic localGameLogic;

        [Header("Spawn Controls")]
        [SerializeField] private List<Vector3> spawnPoints;
        [SerializeField] private byte spawnUnitIndex = 1, spawnPointRandomIndex = 0;
        private byte herdCount = 0, spawnCount = 0, totalHerdsToSpawn = 4,
            invokeCount = 0, totalEnemyUnits = 4, prevUnitIndex = 0;
        //private readonly const byte ;
        //[Range(1f, 5f)]
        [SerializeField] private float spawnInterval = 1f, herdInterval = 0f;
        //private bool stopSpawn;

        private void OnEnable()
        {
            localGameLogic.OnCastleReached += ResetEnemySpawner;
            localGameLogic.OnWaveCompletion += SetSpawnerForNextWave;
            localGameLogic.OnNextWaveRequested += SpawnEnemy;
        }

        private void OnDisable()
        {
            localGameLogic.OnCastleReached -= ResetEnemySpawner;
            localGameLogic.OnWaveCompletion -= SetSpawnerForNextWave;
            localGameLogic.OnNextWaveRequested -= SpawnEnemy;
        }

        // Start is called before the first frame update
        private void Start()
        {
            //Invoke("SpawnEnemy", 0f);
        }

        private void SpawnEnemy()
        {
            //spawnRandomIndex = (byte) Random.Range(0, spawnPoints.Count);
            //spawnPointRandomIndex = 2;
            //Debug.Log("Spawning Herd");

#if !TEST_MODE
            //Make sure prevUnitIndex is equal to the spawnUnitIndex for the first invoke
            while (spawnUnitIndex == prevUnitIndex)
            {
                if (GameManager.instance.currentWave <= totalEnemyUnits)
                    spawnUnitIndex++;
                else
                    spawnUnitIndex = (byte)Random.Range(1, 5);
            }
#endif

            int totalHerdForamtions = enemyHerdFormations[spawnUnitIndex].totalHerdForamtions;
            spawnInterval = enemyHerdFormations[spawnUnitIndex].spawnInterval;
            herdCount = (byte) enemyHerdFormations[spawnUnitIndex].herdFormations[Random.Range(0, totalHerdForamtions)].herdCount;
            //Debug.Log($"Name : {enemyHerdFormations[spawnUnitIndex].name}, SpawnUnitIndex : {spawnUnitIndex}, " +
            //    $"spawnInterval : {spawnInterval}, totalHerdFormations : {totalHerdForamtions}, " +
            //    $"herdCount : {herdCount}");

            SpawnEnemyObject();

            #region Testing
            //spawnCount++;
            //if (spawnCount < invokeCount)
            //    Invoke("SpawnEnemy", spawnInterval);            //Invoke Spawn after a set time interval
            #endregion Testing
        }

        private void SpawnEnemyObject()
        {
            if (spawnCount < herdCount)
            {
                GameObject enemy = EnemyPoolManager.instance.ReuseEnemy(enemyTags[spawnUnitIndex], spawnPoints[spawnPointRandomIndex], Quaternion.identity);
                enemy.GetComponent<EnemyController>().OnObjectSpawn((byte)spawnPointRandomIndex);
                spawnCount++;
                
                GameManager.instance.activeEnemies.Add(enemy.transform);
                Invoke(nameof(SpawnEnemyObject), spawnInterval);            //Invoke Spawn after a set time interval
            }
            else
            {
                invokeCount++;
                prevUnitIndex = spawnUnitIndex;
                spawnCount = 0;
                if (invokeCount < totalHerdsToSpawn)
                    Invoke(nameof(SpawnEnemy), herdInterval);            //Invoke Next Herd Spawn after a set time interval
                else
                {
                    GameManager.instance.totalHerdsSpawned = true;
                    //Debug.Log($"Wave Over : {GameManager.instance.currentWave}");
                }
            }
        }

        private void SetSpawnerForNextWave()
        {
            invokeCount = 0;
            if (GameManager.instance.currentWave > 5)
                totalHerdsToSpawn++;
        }

        private void ResetEnemySpawner()
        {
            //Debug.Log($"Resetting Enemy Spawner");
            CancelInvoke(nameof(SpawnEnemy));
            CancelInvoke(nameof(SpawnEnemyObject));
            //stopSpawn = true;

            //Resetting Stats
            spawnCount = invokeCount = prevUnitIndex = 0;
            totalHerdsToSpawn = 4;
        }
    }
}