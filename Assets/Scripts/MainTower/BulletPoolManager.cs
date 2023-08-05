using System.Collections.Generic;
using UnityEngine;

namespace Moving_Tower
{

    public class BulletPoolManager : MonoBehaviour
    {
        [System.Serializable]
        public class BulletType
        {
            public GameObject bullet;
            public byte count;
            public string tag;
        }

        [SerializeField] private List<BulletType> bulletList;
        [SerializeField] private Dictionary<string, Queue<GameObject>> bulletDictionary;

        private static BulletPoolManager _instance;
        public static BulletPoolManager instance { get { return _instance; } }

        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(this);
            else
                _instance = this;

            AllocatePool();
        }

        private void AllocatePool()
        {
            bulletDictionary = new Dictionary<string, Queue<GameObject>>();

            foreach (BulletType bulletType in bulletList)
            {
                GameObject poolHolder = new GameObject("EnemyPool_" + bulletType.tag);
                poolHolder.transform.parent = transform;

                Queue<GameObject> bullets = new Queue<GameObject>();

                for (int i = 0; i < bulletType.count; i++)
                {
                    var bullet = Instantiate(bulletType.bullet, poolHolder.transform);
                    bullet.name = bulletType.tag + i;
                    bullet.gameObject.SetActive(false);

                    bullets.Enqueue(bullet);
                }

                bulletDictionary.Add(bulletType.tag, bullets);
            }
        }

        public GameObject ReuseBullet(string bulletTag, Transform instantiatePoint)
        {
            if (!bulletDictionary.ContainsKey(bulletTag))
            {
                Debug.LogWarning($"Pool With Tag {bulletTag} does not exist");
                return null;
            }

            GameObject tempBullet = bulletDictionary[bulletTag].Dequeue();
            tempBullet.transform.position = instantiatePoint.position;
            tempBullet.transform.rotation = instantiatePoint.rotation;
            //tempBullet.SetActive(true);

            bulletDictionary[bulletTag].Enqueue(tempBullet);

            return tempBullet;
        }
    }
}