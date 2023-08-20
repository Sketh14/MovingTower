using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Moving_Tower
{
    public class CollectiblePoolManager : MonoBehaviour
    {
        [System.Serializable]
        public class Collectible
        {
            public GameObject collectible;
            public byte count = 0;
            public string tag = "";
        }

        [SerializeField] private List<Collectible> collectibleList;
        [SerializeField] private Dictionary<string, Queue<GameObject>> collectibleDictionary;

        private static CollectiblePoolManager _instance;
        public static CollectiblePoolManager instance { get { return _instance; } }

        private void Awake()
        {
            if (instance == null && instance != this)
                _instance = this;
            else
                Destroy(this.gameObject);

            AllocatePool();
        }

        private void AllocatePool()
        {
            collectibleDictionary = new Dictionary<string, Queue<GameObject>>();

            foreach (Collectible collectible in collectibleList)
            {
                GameObject poolHolder = new GameObject("Collectible_" + collectible.tag);
                poolHolder.transform.parent = transform;

                Queue<GameObject> prefabQueue = new Queue<GameObject>();
                for (int i = 0; i < collectible.count; i++)
                {
                    GameObject tempCollectible = Instantiate(collectible.collectible, poolHolder.transform);
                    tempCollectible.name = collectible.tag + i;
                    tempCollectible.SetActive(false);

                    prefabQueue.Enqueue(tempCollectible);
                }

                collectibleDictionary.Add(collectible.tag, prefabQueue);
            }
        }

        public GameObject ReuseCollectible(string tag)
        {
            if (!collectibleDictionary.ContainsKey(tag))
            {
                Debug.LogError($"Collectible Dictionary does not contain the key : {tag}");
                return null;
            }

            GameObject collectible = collectibleDictionary[tag].Dequeue();
            collectibleDictionary[tag].Enqueue(collectible);
            return collectible;
        }
    }
}