using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moving_Tower
{
    public class DrawProjection : MonoBehaviour
    {
        public bool targetFound = false;

        private LineRenderer bulletPathProjection;

        private Vector3 _targetPos;
        public Vector3 targetPos { set => _targetPos = value; }

        // Start is called before the first frame update
        void Start()
        {
            bulletPathProjection = GetComponent<LineRenderer>();
            bulletPathProjection.SetPosition(0, transform.position);
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            if (targetFound)
                bulletPathProjection.SetPosition(1, _targetPos);
            else
                bulletPathProjection.enabled= false;
        }
    }
}