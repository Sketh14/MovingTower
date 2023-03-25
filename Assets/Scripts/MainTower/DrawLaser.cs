using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moving_Tower
{
    public class DrawLaser : MonoBehaviour
    {
        public bool targetFound = false;

        private LineRenderer laserProjection;
        private RaycastHit hit;

        private Transform _targetPos;
        public Transform targetPos { set => _targetPos = value; }

        // Start is called before the first frame update
        void Start()
        {
            laserProjection = GetComponent<LineRenderer>();
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            if (targetFound)
            {
                laserProjection.SetPosition(0, transform.position);                 //can check if the tower is moving or not and then update the position of the laser
                
                if (Physics.Raycast(transform.position, transform.forward, out hit, 30f))
                {
                    //if (hit.collider.CompareTag("Enemy"))
                    laserProjection.SetPosition(1, hit.point);
                }
            }
        }

        public void EnableLaser(bool status)
        {
            //Debug.Log($"Laser Status : {status}");
            laserProjection.enabled = status;
            laserProjection.SetPosition(1, transform.position);
        }
    }
}