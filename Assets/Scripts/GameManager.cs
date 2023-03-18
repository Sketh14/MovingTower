using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moving_Tower
{
    public class GameManager : MonoBehaviour
    {
        public List<WayPoints_SO> wayPointsList;
        public List<GameObject> activeEnemies;          //Can be made to array for quicker access

        [Header ("Reference Scripts")]
        public GameLogic gameLogicReference;
        public EnemySpawner enemySpawnerReference;

        private static GameManager _instance;
        public static GameManager instance
        {
            get { return _instance; }
        }

        // Start is called before the first frame update
        void Start()
        {
            if (_instance != null && _instance != this)
                Destroy(this);
            else
                _instance = this;

            enemySpawnerReference.enabled = true;
        }
    }
}