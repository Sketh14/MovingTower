using UnityEngine;

namespace Moving_Tower
{
    [CreateAssetMenu(fileName = "Enemy Herd Formation", menuName = "Enemy Herd Formation")]
    public class EnemyHerdFormations_SO : ScriptableObject
    {
        public EnemyHerdDetail_SO[] herdFormations;
    }
}