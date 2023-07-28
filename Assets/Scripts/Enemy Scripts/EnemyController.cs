using UnityEngine;

namespace Moving_Tower
{
    public class EnemyController : MonoBehaviour, IEnemyObject
    {
        [SerializeField] private WayPoints_SO waypoints;
        [SerializeField] private Enemy_SO enemyStats;
        private Vector3 targetDir;

        [Header("Stats Controls")]
        private float health;
        [SerializeField] private Transform statsCanvas;

        [Space]
        private byte _wayPointCount = 0;
        public byte wayPointCount { set => _wayPointCount = value; }

        // Start is called before the first frame update
        public void OnObjectSpawn(byte wayPointIndex)
        {

            waypoints = GameManager.instance.wayPointsList[wayPointIndex];
            targetDir = waypoints.wayPoints[_wayPointCount] - transform.position;

        }

        // Update is called once per frame
        void Update()
        {
            transform.Translate(targetDir.normalized * enemyStats.speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, waypoints.wayPoints[_wayPointCount]) <= 0.4f)
                UpdateWayPoint();

            statsCanvas.forward = Camera.main.transform.forward;
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

        private void OnTriggerEnter(Collider other)
        {
            //Debug.Log($"Collider : {other.name}");
        }

        private void ResetEnemy()
        {
            _wayPointCount = 0;
            gameObject.SetActive(false);
        }

        public void HitByBullet()
        {
            health -= GameManager.instance.currentBulletDamage;

            if (health <= 0)
                gameObject.SetActive(false);
        }
    }
}