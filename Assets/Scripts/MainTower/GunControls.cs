using System.Collections;
using System.Linq;
using UnityEngine;

namespace Moving_Tower
{
    public class GunControls : MonoBehaviour
    {
        [SerializeField] private bool rotate, enemyFound, enemyLeft;
        [SerializeField] private Transform activeTarget;
        private Quaternion startRot;
        private Vector3 tempVec1, tempVec2;

        private Collider[] colliders;
        [SerializeField] private float _turretRange;
        public float turretRange { set => _turretRange = value; }

        // Update is called once per frame
        private void FixedUpdate()
        {
            if (rotate)
            {
                transform.LookAt(activeTarget);
                //transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, transform.eulerAngles.z);
            }

            if (!enemyFound)
            {
                //Debug.Log($"Enemy Found Before : {enemyFound}");
                activeTarget = CheckEnemyWithinPerimeter();
            }
            else if (activeTarget != null)
            {
                //Debug.Log($"Active Target Found");
                if (!rotate)
                {
                    if (startRot != null)
                        startRot = transform.rotation;

                    _ = StartCoroutine(RotateToDirection(activeTarget.position, 0.5f));
                }

                CheckIfActiveTargetLeftPerimeter();
            }
        }
        public IEnumerator RotateToDirection(Vector3 positionToLook, float timeToRotate)
        {
            var startRotation = transform.rotation;
            var direction = positionToLook - transform.position;
            var finalRotation = Quaternion.LookRotation(direction);
            var t = 0f;
            while (t <= 1f)
            {
                t += Time.deltaTime / timeToRotate;
                transform.rotation = Quaternion.Lerp(startRotation, finalRotation, t);
                yield return null;
            }
            transform.rotation = finalRotation;
            rotate = true;
        }

        //Might ad performance if making custom code to detect within range by checking the distance between
        //turret and enemy.
        /*The problem with overlapsphere is that how can we know that the enemy has left the range?*/
        private Transform CheckEnemyWithinPerimeter()
        {
            if (!enemyFound)
            {
                colliders = Physics.OverlapSphere(transform.position, _turretRange);
                var orderedByProximity = colliders.OrderBy(c => (transform.position - c.transform.position).sqrMagnitude).ToArray();
                foreach (Collider collider in orderedByProximity)
                {
                    if (collider.CompareTag("Enemy"))
                    {
                        //tempVec1 = new Vector3(transform.position.x, 0f, transform.position.z);
                        //tempVec2 = new Vector3(collider.transform.position.x, 0f, collider.transform.position.z);
                        //Debug.Log($"Enemy Found After : {enemyFound}, Distance : " + (tempVec1- tempVec2).magnitude);
                        enemyFound = true;
                        return collider.transform;
                    }
                }
            }

            return null;
        }

        private void CheckIfActiveTargetLeftPerimeter()
        {
            tempVec1 = new Vector3(transform.position.x, 0f, transform.position.z);
            tempVec2 = new Vector3(activeTarget.transform.position.x, 0f, activeTarget.transform.position.z);

            if (activeTarget != null && (tempVec1 - tempVec2).sqrMagnitude >= _turretRange * _turretRange)
            {
                //Debug.Log($"Diff : {(tempVec1 - tempVec2).sqrMagnitude}");
                enemyFound = false;
                activeTarget = null;
                rotate = false;
            }
        }

        //Not using this for now. Using a collider instead
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, _turretRange);
        }
    }
}