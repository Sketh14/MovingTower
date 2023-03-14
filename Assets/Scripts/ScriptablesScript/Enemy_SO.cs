using UnityEngine;

namespace Moving_Tower
{
    [CreateAssetMenu(fileName = "Enemy_Stats", menuName = "Enemy_Stats")]
    public class Enemy_SO : ScriptableObject
    {
        public string tag;
        public float speed;
    }
}