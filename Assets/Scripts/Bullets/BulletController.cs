using UnityEngine;

namespace Moving_Tower
{
    public class BulletController : MonoBehaviour
    {
        //[SerializeField] private Rigidbody bulletRb;
        [SerializeField] private float bulletSpeed = 5f, maxDistance = 10f;
        [SerializeField] private Vector3 startPoint;

        private void OnEnable()
        {
            startPoint = transform.position;
            InvokeRepeating(nameof(CheckDistance), 0f, 1f);
            //bulletRb.isKinematic = false;
            //bulletRb.AddForce(transform.forward * bulletSpeed, ForceMode.Impulse);
        }

        private void Update()
        {
            transform.Translate(transform.forward * bulletSpeed * Time.deltaTime, Space.World);
        }

        /*private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                other.GetComponent<EnemyController>().HitByBullet();
                bulletRb.velocity = Vector3.zero;
                bulletRb.isKinematic = true;
                gameObject.SetActive(false);
            }
        }*/

        private void CheckDistance()
        {
            if (Vector3.Distance(startPoint, transform.position) >= maxDistance)
                gameObject.SetActive(false);
        }
    }
}