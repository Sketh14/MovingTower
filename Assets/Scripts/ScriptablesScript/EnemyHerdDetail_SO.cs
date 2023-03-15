using UnityEngine;

namespace Moving_Tower
{
    [CreateAssetMenu(fileName = "Enemy Herd Detail", menuName = "Enemy Herd Detail")]
    public class EnemyHerdDetail_SO : ScriptableObject
    {
        public string herdTag;
        public int herdCount;
    }
}