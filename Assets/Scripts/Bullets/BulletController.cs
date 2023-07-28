using UnityEngine;

namespace Moving_Tower
{
    public class BulletController : MonoBehaviour
    {
        [SerializeField] private Rigidbody bulletRb;
        [SerializeField] private float bulletSpeed = 5f;

        // Start is called before the first frame update
        void Start()
        {
            bulletRb.AddForce(transform.forward * bulletSpeed, ForceMode.Impulse);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                other.GetComponent<EnemyController>().HitByBullet();
                gameObject.SetActive(false);
            }
        }
    }
}