using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPoolManager : MonoBehaviour
{
    Queue<GameObject> enemyPool = new Queue<GameObject>();
    [SerializeField] private GameObject enemyPrefab;

    private static EnemyPoolManager _instance;
    public static EnemyPoolManager instance { get { return _instance; } }

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            _instance = this;

        AllocatePool();
    }

    private void AllocatePool()
    {
        GameObject poolHolder = new GameObject("EnemyPool");
        poolHolder.transform.parent = transform;

        for (int i = 0; i < 10; i ++)
        {
            GameObject enemy = Instantiate(enemyPrefab, poolHolder.transform);
            enemy.name = "enemy" + i;
            enemy.gameObject.SetActive(false);

            enemyPool.Enqueue(enemy);
        }
    }

    public GameObject ReuseEnemy(Vector3 position, Quaternion rotation)
    {
        GameObject tempEnemy = enemyPool.Dequeue();
        enemyPool.Enqueue(tempEnemy);
        tempEnemy.SetActive(true);
        tempEnemy.transform.position = position;
        tempEnemy.transform.rotation = rotation;

        return tempEnemy;
    }
}
