using System.Collections.Generic;
using UnityEngine;

namespace Moving_Tower
{
    public class GunControls : MonoBehaviour
    {
        [Header("Test Variables")]
        [SerializeField] private bool testRange;

        [Space]
        [SerializeField] private bool rotate, enemyFound, enemyLeft, enableTurret;
        [SerializeField] private Transform activeTarget, perimeterArea;
        private Vector3 tempVec1, tempVec2;

        [Header("Main Menu")]
        private Quaternion finalRot;              //startRot,
        private float time = -0.01f;               //For Main Menu, -.01

        [Header("Shoot Controls")]
        [SerializeField] private bool enableShooting;
        [SerializeField] private Transform shootingPoint;
        [SerializeField] private ParticleSystem gunFlash;
        [SerializeField] private float _shootInterval = 1f;
        public float shootInterval { set => _shootInterval = value; }
        [SerializeField] private float _turretRange;
        public float turretRange { set => _turretRange = value; }

        [Header("Local Reference Scritps")]
        [SerializeField] private DrawLaser localDrawLaser;
        [SerializeField] private GameLogic localGameLogic;

        private Collider[] colliders;

        private void OnEnable()
        {
            localGameLogic.OnTowerCollected += DisableTurret;
            localGameLogic.OnEnemyKilled += DisableShooting;
            localGameLogic.OnPowersSelected += UpgradeTower;

            localGameLogic.OnIntroFinished += 
                () => { CancelInvoke(nameof(EnableRotate));
                    rotate = false; 
                    enableTurret = true; };

            localGameLogic.OnCastleReached +=
                () => { DisableShooting(null);
                    perimeterArea.gameObject.SetActive(false);
                    Debug.Log($"Rotate after Enemy reached csatle");
                    CancelInvoke(nameof(EnableRotate));
                    rotate = true;
                    enableTurret = false; };
        }

        private void OnDisable()
        {
            localGameLogic.OnTowerCollected -= DisableTurret;
            localGameLogic.OnEnemyKilled -= DisableShooting;
            localGameLogic.OnPowersSelected -= UpgradeTower;
        }

        private void Start()
        {
            //GameManager.instance.currentBulletDamage = 10;
            InvokeRepeating(nameof(UpdatePerimeterHiglightArea), 0f, 2f);
            //RotateTurretRandomly();
            //InvokeRepeating(nameof(UpdatePerimeterHiglightArea), 0f, Time.fixedDeltaTime);
        }

        #region TurretControls

        // Update is called once per frame
        private void FixedUpdate()
        {
            if (!enableTurret)
            {
                if (rotate)
                    RotateTurretRandomly();
                return;
            }

            if (rotate)
            {
                transform.LookAt(activeTarget);

                if (!enableShooting)
                {
                    enableShooting = true;
                    InvokeRepeating(nameof(ShootBullets), 0f, _shootInterval);
                }

                //transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, transform.eulerAngles.z);
            }

            if (!enemyFound )                  //Comment testRange for actual gameplay //&& !testRange
            {
                //Debug.Log($"Enemy Found Before : {enemyFound}");
                //activeTarget = CheckEnemyWithinPerimeter();
                activeTarget = CheckEnemyWithinPerimeter2();
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

        private void DisableTurret(bool towerMoveStatus)
        {
            enableTurret = !towerMoveStatus;
            perimeterArea.gameObject.SetActive(towerMoveStatus);

            if (towerMoveStatus && enemyFound)
                DisableShooting(null);
        }

        private void DisableShooting(Transform dummyData)
        {
            //Debug.Log("Disabling Shooting");

            enemyFound = false;
            activeTarget = null;
            rotate = false;
            localDrawLaser.targetFound = false;
            localDrawLaser.EnableLaser(false);

            enableShooting = false;
            CancelInvoke(nameof(ShootBullets));
        }
        private void ShootBullets()
        {
            gunFlash.Play();
            GameObject tempBullet = BulletPoolManager.instance.ReuseBullet("Bullet", shootingPoint);
            tempBullet.SetActive(true);
        }
        #endregion TurretControls

        private void UpgradeTower(byte chosenPower)
        {
            switch(chosenPower)
            {
                case 0:
                    _turretRange += 1f;
                    break;

                case 1:
                    _shootInterval -= 0.1f;
                    break;

                case 2:
                    GameManager.instance.currentBulletDamage += 1f;
                    break;

                default:
                    Debug.Log($"Inside GunControl. Power of Index : {chosenPower}, not available");
                    break;
            }
        }

        #region TurretRotation
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
            Vector3 direction = activeTarget.position - transform.position;
            Quaternion finalRot = Quaternion.LookRotation(direction);
            time += rotateSpeed * Time.deltaTime;
            if (Quaternion.Angle(transform.rotation, finalRot) <= 5f)
            {
                time = 0f;
                rotate = true;
                transform.rotation = finalRot;
                //localDrawLaser.enabled = true;
                localDrawLaser.targetFound = true;
                localDrawLaser.targetPos = activeTarget;
                localDrawLaser.EnableLaser(true);
                //Debug.Log($"Target Foud : {localDrawLaser.targetFound}");
            }
            transform.rotation = Quaternion.Lerp(transform.rotation, finalRot, time);
        }
        #endregion TurretRotation

        #region PerimeterCheck
        //Might add performance if making custom code to detect within range by checking the distance between
        //turret and enemy.
        /*The problem with overlapsphere is that how can we know that the enemy has left the range?*/
        //Old Code
        /*private Transform CheckEnemyWithinPerimeter()
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
        }*/

        //Checking By distance if enemy is within perimeter
        private Transform CheckEnemyWithinPerimeter2()
        {
            if (!enemyFound && GameManager.instance.activeEnemies.Count != 0)
            {
                tempVec1 = new Vector3(transform.parent.position.x, 0f, transform.parent.position.z);   //The base of the turret
                
                List<Transform> enemies = GameManager.instance.activeEnemies;
                foreach (Transform t in enemies)
                {
                    if (t.gameObject.activeSelf)
                    {
                        tempVec2 = new Vector3(t.position.x, 0f, t.position.z);

                        if ((tempVec1 - tempVec2).sqrMagnitude <= _turretRange * _turretRange)
                        {
                            //Debug.Log($"Enemy :{t.name}, Found After : {enemyFound}, Distance : " + (tempVec1 - tempVec2).sqrMagnitude);

                            enemyFound = true;
                            return t;
                        }
                    }
                }
            }
            return null;
        }

        private void CheckIfActiveTargetLeftPerimeter()
        {
            tempVec1 = new Vector3(transform.parent.position.x, 0f, transform.parent.position.z);   //The base of the turret
            tempVec2 = new Vector3(activeTarget.transform.position.x, 0f, activeTarget.transform.position.z);

            if (activeTarget != null && (tempVec1 - tempVec2).sqrMagnitude >= _turretRange * _turretRange )
            {
                //Debug.Log($"Target Left, Diff : {(tempVec1 - tempVec2).sqrMagnitude}");
                DisableShooting(null);

                /*enemyFound = false;
                activeTarget = null;
                rotate = false;
                localDrawLaser.targetFound = false;
                localDrawLaser.EnableLaser(false);

                enableShooting = false;
                CancelInvoke(nameof(ShootBullets));*/

                //localDrawLaser.enabled = false;
            }
        }
        #endregion PerimeterCheck

        #region TurretVisuals
        //Not using this for now. Using a collider instead
        private void OnDrawGizmosSelected()
        {
            tempVec1 = new Vector3(transform.parent.position.x, 0f, transform.parent.position.z);
            Gizmos.DrawWireSphere(tempVec1, (_turretRange) );
        }

        private void UpdatePerimeterHiglightArea()
        {
            float higlightScaleXZ = 1.25f * _turretRange;               //y = 1.25x
            Vector3 tempScale = new Vector3(higlightScaleXZ, 15f, higlightScaleXZ);

            perimeterArea.transform.localScale = tempScale;
        }
        #endregion TurretVisuals

        #region MainMenu
        private void RotateTurretRandomly()
        {
            if (time <= 0)
                finalRot = Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 0f));

            time += 0.1f * Time.deltaTime;              //0.2f is rotate speed
            if (Quaternion.Angle(transform.rotation, finalRot) <= 1f)
            {
                rotate = false;
                time = -0.01f;
                transform.rotation = finalRot;

                Invoke(nameof(EnableRotate), Random.Range(1, 4));
                //Debug.Log($"Rotaion Done");
            }
            transform.rotation = Quaternion.Lerp(transform.rotation, finalRot, time);
            //Invoke(nameof(RotateTurretRandomly), Time.fixedDeltaTime);
        }

        private void EnableRotate()
        {
            rotate = true;
        }
        #endregion MainMenu
    }
}