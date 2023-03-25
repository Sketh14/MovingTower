using System.Collections;
using System.Linq;
using UnityEngine;

namespace Moving_Tower
{
    public class GunControls : MonoBehaviour
    {
        [Header("Test Variables")]
        [SerializeField] private bool testRange;

        [Space]
        [SerializeField] private bool rotate, enemyFound, enemyLeft, enableTurret;
        [SerializeField] private Transform activeTarget, radiusCheck;
        private Quaternion startRot, finalRot;
        private Vector3 tempVec1, tempVec2;
        private float time;

        [Header("Local Reference Scritps")]
        [SerializeField] private DrawLaser localDrawLaser;
        [SerializeField] private GameLogic localGameLogic;

        private Collider[] colliders;
        [SerializeField] private float _turretRange;
        public float turretRange { set => _turretRange = value; }

        private void OnEnable()
        {
            localGameLogic.OnTowerCollected += DisableTurret;
        }

        private void OnDisable()
        {
            localGameLogic.OnTowerCollected -= DisableTurret;
                }

        // Update is called once per frame
        private void FixedUpdate()
        {
            if (!enableTurret)
                return;

            if (rotate)
            {
                transform.LookAt(activeTarget);
                //transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, transform.eulerAngles.z);
            }

            if (!enemyFound )                  //Comment testRange for actual gameplay //&& !testRange
            {
                //Debug.Log($"Enemy Found Before : {enemyFound}");
                activeTarget = CheckEnemyWithinPerimeter();
                //testRange = true;
            }
            else if (activeTarget != null)
            {
                //Debug.Log($"Active Target Found");

                //tempVec1 = new Vector3(transform.parent.position.x, 0f, transform.parent.position.z);
                //tempVec2 = new Vector3(activeTarget.transform.position.x, 0f, activeTarget.transform.position.z);
                //Debug.Log($"Enemy Found Before : {enemyFound}, Distance : " + (tempVec1 - tempVec2).sqrMagnitude);

                if (!rotate)
                {
                    {
                        //rotate = true;
                        //if (startRot != null)
                        //    startRot = transform.rotation;

                        //Debug.Log($"Starting to rotate");
                        //_ = StartCoroutine(RotateToDirection(activeTarget, 0.3f));
                    }

                    {
                        //if (startRot == null)
                        //    startRot = transform.rotation;

                        //Debug.Log($"Starting to rotate");
                        RotateToDirection2(0.3f);

                        {
                            //rotate = true;
                            //localDrawLaser.EnableLaser(true);
                            //localDrawLaser.targetFound = true;
                            //localDrawLaser.targetPos = activeTarget;
                        }
                    }
                }
                else if (localDrawLaser.targetFound)
                    CheckIfActiveTargetLeftPerimeter();
            }
        }

        private void DisableTurret()
        {
            enableTurret = false;
        }

        //Wait for the turret to face the direction of the enemy
        /*public IEnumerator RotateToDirection(Transform positionToLook, float timeToRotate)
        {
            var startRotation = transform.rotation;
            var direction = positionToLook.position - transform.position;
            var finalRotation = Quaternion.LookRotation(direction);
            var t = 0f;
            while (t <= 1f)
            {
                t += Time.deltaTime / timeToRotate;
                transform.rotation = Quaternion.Lerp(startRotation, finalRotation, t);
                yield return null;
            }
            transform.rotation = finalRotation;
            localDrawLaser.EnableLaser(true);
            localDrawLaser.targetFound = true;
            localDrawLaser.targetPos = activeTarget;
            Debug.Log($"Target Foud : {localDrawLaser.targetFound}");
        }*/

        //Wait for the turret to face the direction of the enemy
        public void RotateToDirection2(float rotateSpeed)
        {
            var direction = activeTarget.position - transform.position;
            finalRot = Quaternion.LookRotation(direction);
            startRot = transform.rotation;
            time += rotateSpeed * Time.deltaTime;
            if (Quaternion.Angle(transform.rotation, finalRot) <= 5f)
            {
                time = 0f;
                rotate = true;
                transform.rotation = finalRot;
                localDrawLaser.EnableLaser(true);
                localDrawLaser.targetFound = true;
                localDrawLaser.targetPos = activeTarget;
                Debug.Log($"Target Foud : {localDrawLaser.targetFound}");
            }
            transform.rotation = Quaternion.Lerp(transform.rotation, finalRot, time);
        }

        //Might ad performance if making custom code to detect within range by checking the distance between
        //turret and enemy.
        /*The problem with overlapsphere is that how can we know that the enemy has left the range?*/
        private Transform CheckEnemyWithinPerimeter()
        {
            if (!enemyFound)
            {
                //Check From the base of the turret
                tempVec1 = new Vector3(transform.parent.position.x, 0f, transform.parent.position.z);
                colliders = Physics.OverlapSphere(tempVec1, (_turretRange));             //(range - 3.5) as the size of the enemy
                var orderedByProximity = colliders.OrderBy(c => (transform.parent.position - c.transform.position).sqrMagnitude).ToArray();
                foreach (Collider collider in colliders)
                {
                    //Debug.Log($"Collider {collider.name} found, Distance : {(tempVec1 - tempVec2).sqrMagnitude}");

                    if (collider.CompareTag("Enemy"))
                    {
                        tempVec1 = new Vector3(transform.parent.position.x, 0f, transform.parent.position.z);
                        tempVec2 = new Vector3(collider.transform.position.x, 0f, collider.transform.position.z);
                        Debug.Log($"Enemy :{collider.name}, Found After : {enemyFound}, Distance : " + (tempVec1 - tempVec2).sqrMagnitude);
                        
                        enemyFound = true;
                        return collider.transform;
                    }
                }
            }

            return null;
        }

        private void CheckIfActiveTargetLeftPerimeter()
        {
            tempVec1 = new Vector3(transform.parent.position.x, 0f, transform.parent.position.z);   //The base of the turret
            tempVec2 = new Vector3(activeTarget.transform.position.x, 0f, activeTarget.transform.position.z);

            if (activeTarget != null && (tempVec1 - tempVec2).sqrMagnitude >= _turretRange * _turretRange)
            {
                Debug.Log($"Diff : {(tempVec1 - tempVec2).sqrMagnitude}");
                enemyFound = false;
                activeTarget = null;
                rotate = false;
                localDrawLaser.targetFound = false;
                localDrawLaser.EnableLaser(false);
            }
        }

        //Not using this for now. Using a collider instead
        private void OnDrawGizmosSelected()
        {
            tempVec1 = new Vector3(transform.parent.position.x, 0f, transform.parent.position.z);
            Gizmos.DrawWireSphere(tempVec1, (_turretRange) );
        }
    }
}