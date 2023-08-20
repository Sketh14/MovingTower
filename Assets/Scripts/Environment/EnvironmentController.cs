using System.Collections;
using UnityEngine;

namespace Moving_Tower
{
    public class EnvironmentController : MonoBehaviour
    {
        [SerializeField] private Transform drawer;
        //private Vector3 poppedPos = new Vector3(-12.84f, 4.98f, 36.67f);
        //private float timeTaken;

        [Header("Local Reference Scripts")]
        [SerializeField] private GameLogic localGameLogic;

        private void OnEnable()
        {
            localGameLogic.OnGameplayStart += () => { _ = StartCoroutine(PopDrawer(true)); };
        }

        private IEnumerator PopDrawer(bool forwardDir)
        {
            float timeTaken = 0f;
            float timeMultiplier = forwardDir ? 3f : 3.5f;
            //bool popped = false;
            Vector3 startPos = drawer.localPosition;
            Vector3 poppedPos = startPos;
            poppedPos.x += forwardDir ? -3f : 3f;

            while (true)
            {
                timeTaken += timeMultiplier * Time.deltaTime;

                if (timeTaken > 1)
                {
                    if (forwardDir)
                        _ = StartCoroutine(PopDrawerCover());
                    else
                        localGameLogic.OnIntroFinished?.Invoke();
                    break;
                    /*if (popped) 
                        break;

                    timeTaken = 0;
                    startPos = drawer.localPosition;
                    poppedPos.x += 4f;
                    timeMultiplier = 1.6f;      //Withdrawal time will be more, so that it is fast
                    popped = true;*/
                }

                drawer.localPosition = Vector3.Lerp(startPos, poppedPos, timeTaken);
                yield return null;
            }
        }

        private IEnumerator PopDrawerCover()
        {
            yield return new WaitForSeconds(0.3f);
            float timeTaken = 0f;
            float timeMultiplier = 4f;
            Vector3 startPos = drawer.GetChild(0).localPosition;
            Vector3 poppedPos = startPos;
            poppedPos.z = 1f;
            while (true)
            {
                timeTaken += timeMultiplier * Time.deltaTime;

                if (timeTaken > 1)
                {
                    _ = StartCoroutine(PopDrawer(false));
                    break;
                }

                drawer.GetChild(0).localPosition = Vector3.Lerp(startPos, poppedPos, timeTaken);
                yield return null;
            }
        }
    }
}