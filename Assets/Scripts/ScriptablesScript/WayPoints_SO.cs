using System.Collections.Generic;
using UnityEngine;

namespace Moving_Tower
{
    [CreateAssetMenu(fileName = "Waypoints", menuName = "Waypoints")]
    public class WayPoints_SO : ScriptableObject
    {
        public List<Vector3> wayPoints;
    }
}