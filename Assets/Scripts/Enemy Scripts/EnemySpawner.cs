using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moving_Tower
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private string[] enemyTags;
        [SerializeField] private EnemyHerdFormations_SO[] enemyHerdFormations;

        [Header("Spawn Controls")]
        [SerializeField] private List<Vector3> spawnPoints;
        private byte spawnUnitIndex = 0, totalHerdForamtions = 0, herdCount = 0, 
            spawnCount = 0, spawnRandomIndex = 0, prevUnitIndex = 0;
        //[Range(1f, 5f)]
        [SerializeField] private float spawnInterval = 1f, waveInterval = 0f;
        //private bool stopSpawn;

        [Header("Testing Variables")]
        private byte invokeCount = 0;

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
            Invoke("SpawnEnemy", 0f);
        }

        private void SpawnEnemy()
        {
            //spawnRandomIndex = (byte) Random.Range(0, spawnPoints.Count);
            spawnRandomIndex = 2;

            //while (spawnUnitIndex == prevUnitIndex)
            //    spawnUnitIndex = (byte) Random.Range(0, 5);            
            spawnUnitIndex = 0;

            spawnInterval = enemyHerdFormations[spawnUnitIndex].spawnInterval;
            totalHerdForamtions = (byte) enemyHerdFormations[spawnUnitIndex].totalHerdForamtions;
            herdCount = (byte) enemyHerdFormations[spawnUnitIndex].herdFormations[Random.Range(0, totalHerdForamtions)].herdCount;
            Debug.Log($"Name : {enemyHerdFormations[spawnUnitIndex].name}, SpawnUnitIndex : {spawnUnitIndex}, " +
                $"spawnInterval : {spawnInterval}, totalHerdFormations : {totalHerdForamtions}, " +
                $"herdCount : {herdCount}");

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
                var enemy = EnemyPoolManager.instance.ReuseEnemy(enemyTags[spawnUnitIndex], spawnPoints[spawnRandomIndex], Quaternion.identity);
                enemy.GetComponent<EnemyController>().OnObjectSpawn((byte)spawnRandomIndex);
                spawnCount++;
                
                GameManager.instance.activeEnemies.Add(enemy);
                Invoke("SpawnEnemyObject", spawnInterval);            //Invoke Spawn after a set time interval
            }
            else
            {
                prevUnitIndex = spawnUnitIndex;
                spawnCount = 0;
                Invoke("SpawnEnemy", waveInterval);            //Invoke Next Wave after a set time interval
            }
        }

        private void StopEnemySpawn()
        {
            //stopSpawn = true;
        }
    }
}