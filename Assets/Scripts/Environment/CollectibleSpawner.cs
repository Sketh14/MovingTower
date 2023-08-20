using System.Collections.Generic;
using UnityEngine;

namespace Moving_Tower
{
    public class CollectibleSpawner : MonoBehaviour
    {
        //private bool firstInvoke;
        private string TOKEN_TAG = "Token";
        private byte invokeCount;
        [SerializeField] private byte upperTimeLimit, lowerTimeLimit;
        private List<GameObject> spawnedCollectibles;

        [Header("Local Reference Scritps")]
        [SerializeField] private GameLogic localGameLogic;

        [Header("Local Reference Scritps")]
        [SerializeField] private Transform[] spawnPoints;

        private void OnEnable()
        {
            localGameLogic.OnIntroFinished += SpawnCollectible;
        }

        private void OnDisable()
        {
            localGameLogic.OnIntroFinished -= SpawnCollectible;
        }

        private void Start()
        {
            spawnedCollectibles = new List<GameObject>();
        }

        //Recursive function
        private void SpawnCollectible()
        {
            Invoke(nameof(SpawnCollectible), Random.Range(lowerTimeLimit, upperTimeLimit));       //20,25

            if (invokeCount++ == 0)           //(!firstInvoke) //firstInvoke = true;
                return;
            else if (invokeCount < 3)           //it has already incremented twice
                localGameLogic.OnPromptCalled?.Invoke(1);

            //Spawn collectible, decide position, activate gameObject
            GameObject collectible = CollectiblePoolManager.instance.ReuseCollectible(TOKEN_TAG);
            int randomIndex = Random.Range(0, 3) * 2;           //between 0-4 with an interval of 1, so 0,2,4
            float randomX = Random.Range(spawnPoints[randomIndex].position.x, spawnPoints[randomIndex + 1].position.x);
            float randomZ = Random.Range(spawnPoints[randomIndex].position.z, spawnPoints[randomIndex + 1].position.z);
            collectible.transform.localPosition = new Vector3(randomX, 7.35f, randomZ);     //y value is same as the walkable area is in same plane.
            collectible.SetActive(true);
            //Debug.Log($"Token spawned at : {collectible.transform.localPosition}");
        }
    }
}