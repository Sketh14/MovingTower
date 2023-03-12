using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moving_Tower
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private WayPoints_SO waypoints;
        [SerializeField] private Enemy_SO enemyStats;
        private Vector3 targetDir;
        private int wayPointCount = 0;

        // Start is called before the first frame update
        void Start()
        {
            enemyStats = GameManager.instance.enemyStats[2];            //by default the unit will be a normal unit
        }

        public void SetWayPoints(byte waypointIndex)
        {
            waypoints = GameManager.instance.wayPoints[waypointIndex];
            targetDir = waypoints.wayPoints[wayPointCount] - transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            transform.Translate(targetDir.normalized * enemyStats.speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, waypoints.wayPoints[wayPointCount]) <= 0.4f)
                UpdateWayPoint();
        }

        private void UpdateWayPoint()
        {
            if (wayPointCount < waypoints.wayPoints.Count - 1)
            {
                wayPointCount++;
                targetDir = waypoints.wayPoints[wayPointCount] - transform.position;
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
            wayPointCount = 0;
        }
    }
}