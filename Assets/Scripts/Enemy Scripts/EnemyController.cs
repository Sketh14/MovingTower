using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moving_Tower
{
    public class EnemyController : MonoBehaviour, IEnemyObject
    {
        [SerializeField] private WayPoints_SO waypoints;
        [SerializeField] private Enemy_SO enemyStats;
        private Vector3 targetDir;

        private byte _wayPointCount = 0;
        public byte wayPointCount { set => _wayPointCount = value; }

        // Start is called before the first frame update
        public void OnObjectSpawn(byte wayPointIndex)
        {
            enemyStats = GameManager.instance.enemyStats[2];            //by default the unit will be a normal unit

            waypoints = GameManager.instance.wayPointsList[wayPointIndex];
            targetDir = waypoints.wayPoints[_wayPointCount] - transform.position;

        }

        // Update is called once per frame
        void Update()
        {
            transform.Translate(targetDir.normalized * enemyStats.speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, waypoints.wayPoints[_wayPointCount]) <= 0.4f)
                UpdateWayPoint();
        }

        private void UpdateWayPoint()
        {
            if (_wayPointCount < waypoints.wayPoints.Count - 1)
            {
                _wayPointCount++;
                targetDir = waypoints.wayPoints[_wayPointCount] - transform.position;
            }
            else
            {
                GameManager.instance.gameLogicReference.OnCastleReached?.Invoke();
                ResetEnemy();
            }
        }

        private void ResetEnemy()
        {
            gameObject.SetActive(false);
            _wayPointCount = 0;
        }
    }
}