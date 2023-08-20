using UnityEngine;

namespace Moving_Tower
{
    public class AirUnit1 : EnemyController
    {
        private bool wingsUp;

        private void OnDisable()
        {
            CancelInvoke(nameof(FlapWings));
        }

        public override void OnObjectSpawn(byte wayPointIndex)
        {
            base.OnObjectSpawn(wayPointIndex);
            InvokeRepeating(nameof(FlapWings), 0f, 1f);
        }

        private void FlapWings()
        {
            wingsUp = !wingsUp;
            if(wingsUp)
            {
                transform.GetChild(0).eulerAngles = new Vector3(0f, 0f, -36f);
                transform.GetChild(1).eulerAngles = new Vector3(0f, 0f, 36f);
                transform.position -= new Vector3(0f, 1f, 0f);
            }
            else
            {
                transform.GetChild(0).eulerAngles = new Vector3(0f, 0f, 36f);
                transform.GetChild(1).eulerAngles = new Vector3(0f, 0f, -36f);
                transform.position += new Vector3(0f, 1f, 0f);
            }
        }
    }
}