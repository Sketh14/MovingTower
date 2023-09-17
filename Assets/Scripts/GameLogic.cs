using System.Collections;
using UnityEngine;

namespace Moving_Tower
{
    public class GameLogic : MonoBehaviour
    {
        [Header("Actions")]
        public System.Action OnCastleReached, OnGameplayStart, OnIntroFinished, 
            OnWaveCompletion, OnNextWaveRequested;
        public System.Action<bool> OnTowerCollected;
        public System.Action<byte> OnInteraction, OnPromptCalled, OnPowersSelected, OnCollectibleCollected;
        public System.Action<Transform> OnEnemyKilled;

        [Header("Local Reference Scritps")]
        [SerializeField] private PlayerController localPlayerController;
        [SerializeField] private EnemySpawner localEnemySpawner;
        [SerializeField] private EnvironmentController localEnvironmentController;

        private void OnEnable()
        {
            OnWaveCompletion += () => { GameManager.instance.currentWave++; };
            OnIntroFinished += () => { _ = StartCoroutine(ChangeCameraSettings()); };
            //OnInteraction += ChangeGameStatus;
        }

        private void OnDisable()
        {
            //OnInteraction -= ChangeGameStatus;
        }

        //On Main Canvas, under MainMenuCanvas / MainMenuCanvas
        public void StartGame()
        {
            localPlayerController.enabled = true;
            //localEnemySpawner.gameObject.SetActive(true);

            OnGameplayStart?.Invoke();
        }

        private IEnumerator ChangeCameraSettings()
        {
            float timeTaken = 0f;
            float timeMultiplier = 0.5f;

            Camera mainCamera = Camera.main;
            Vector3 startPos = mainCamera.transform.localPosition;
            Vector3 finalPos = new Vector3(0f, 47.4f, -45.1f);
            Quaternion startRot = mainCamera.transform.localRotation;
            Quaternion finalRot = Quaternion.Euler(new Vector3(45f, 0f, 0f));
            float startSize = 10f, endSize = 49.5f;

            while (true)
            {
                timeTaken += timeMultiplier * Time.deltaTime;

                if (timeTaken > 1)
                {
                    mainCamera.orthographicSize = 49.5f;
                    mainCamera.transform.localPosition = new Vector3(0f, 47.4f, -45.1f);
                    mainCamera.transform.localRotation = Quaternion.Euler(new Vector3(45f, 0f, 0f));
                    break;
                }

                mainCamera.transform.localPosition = Vector3.Lerp(startPos, finalPos, timeTaken);
                mainCamera.transform.localRotation = Quaternion.Lerp(startRot, finalRot, timeTaken);
                mainCamera.orthographicSize = Mathf.Lerp(startSize, endSize, timeTaken); 
                yield return null;
            }

            yield return new WaitForSeconds(2f);        //Wait some time before spawning Enemies
            localEnemySpawner.gameObject.SetActive(true);
        }

        /*private void ChangeGameStatus(byte chosenIndex)
        {
            if (chosenIndex == 2)
                PauseGame(true);
        }*/

        //On the close button, under UpgradePanel/GameplayCanvas/UI
        //On the Upgrade button, under Gameplaycanvas/UI
        public void PauseGame(bool pause)
        {
            GameManager.instance.gamePaused = pause;
            Time.timeScale = pause ? 0.0f : 1.0f;
        }
    }
}
