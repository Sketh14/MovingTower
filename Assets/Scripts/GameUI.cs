#define MOBILE_MODE

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Moving_Tower
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private TMPro.TMP_Text promptTxt, interactBtTxt, currentWaveTxt, tokenTxt;
        [SerializeField] private TMPro.TMP_Text[] powerPriceTxt;
        //[SerializeField] private Transform turretPerimeter;
        [SerializeField] private Canvas mainCanvas, gameplayCanvas, pauseCanvas;
        [SerializeField] private GameObject upgradePanel, levelTransitionUI;
        [SerializeField] private Animator effectsCanvasAnimator, gameOverCanvas_AC, pauseCanvas_AC;

        [Header("Buttons")]
        [SerializeField] private Image interactBtImg;
        [SerializeField] private Button interactButton, upgradeButton, pauseButton;
        [SerializeField] private Button[] upgradeBts;

        /************************************************************
         * 0 => interact button
         * 1 => upgrade button
         ************************************************************/
        private byte buttonsToggleStatus;

        [Header("Local Reference Scritps")]
        [SerializeField] private GameLogic localGameLogic;
        [SerializeField] private GunControls localGunControls;      //Test variable

        [Space]
        private bool interactDone;

        private void OnEnable()
        {
            localGameLogic.OnInteraction += DisplayPrompt;
            //localGameLogic.OnInteraction += DisplayUI;
            localGameLogic.OnIntroFinished += EnableGameplayUI;
            localGameLogic.OnPromptCalled += DisplayPrompt;
            localGameLogic.OnWaveCompletion += UpdateCurrentWave;
            localGameLogic.OnCollectibleCollected += UpdatecollectiblesUI;

            localGameLogic.OnCastleReached += 
                () => { gameplayCanvas.gameObject.SetActive(false);
                    gameOverCanvas_AC.gameObject.SetActive(true);
                    gameOverCanvas_AC.Play("SwipeOver_Entry", 0); };
        }

        private void OnDisable()
        {
            localGameLogic.OnInteraction -= DisplayPrompt;
            //localGameLogic.OnInteraction -= DisplayUI;
            localGameLogic.OnIntroFinished -= EnableGameplayUI;
            localGameLogic.OnPromptCalled -= DisplayPrompt;
            localGameLogic.OnWaveCompletion -= UpdateCurrentWave;
            localGameLogic.OnCollectibleCollected -= UpdatecollectiblesUI;
        }

        private void Start()
        {
            //InvokeRepeating(nameof(UpdatePerimeterHiglightArea), 0f, 2f);
        }

        /*private void UpdatePerimeterHiglightArea()
        {
            float spriteScale = (1.5f * localGunControls.turretRange) + 2.5f;          //y=1.5x+2.5     //y = 1.25x
            Vector3 tempScale = new Vector3(spriteScale, spriteScale, 1f);

            turretPerimeter.transform.localScale = tempScale;
        }*/

        #region CollectibleUI
        private void UpdatecollectiblesUI(byte collectibleIndex)
        {
            switch(collectibleIndex)
            {
                case 0:
                    tokenTxt.text = GameManager.instance.tokenCollected.ToString();
                    break;

                default:
                    Debug.LogError($"Under GameUI, Collectible Index : {collectibleIndex} not found");
                    break;
            }
        }
        #endregion CollectibleUI

        #region WaveTransition
        private void UpdateCurrentWave()
        {
            upgradeButton.gameObject.SetActive(false);
            gameplayCanvas.gameObject.SetActive(false);
            effectsCanvasAnimator.gameObject.SetActive(true);

            //Play Wave Increase Animation
            levelTransitionUI.transform.GetChild(1).GetComponent<TMPro.TMP_Text>().text =
                (GameManager.instance.currentWave).ToString();
            effectsCanvasAnimator.Play("LevelTransition", 0, 0f);

            Invoke(nameof(IncreaseWavetTxt), 2.6f);
            Invoke(nameof(DisableEffectsCanvas), 4.1f);
        }

        private void IncreaseWavetTxt()
        {
            gameplayCanvas.gameObject.SetActive(true);
            int waveIndex = GameManager.instance.currentWave;
            currentWaveTxt.text = "WAVE " + waveIndex.ToString();
            levelTransitionUI.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = waveIndex.ToString();
        }

        private void DisableEffectsCanvas()
        {
            effectsCanvasAnimator.gameObject.SetActive(false);
            localGameLogic.OnNextWaveRequested?.Invoke();
        }
        #endregion WaveTransition

        /*private void DisplayUI(byte uiIndex)
        {
            switch(uiIndex)
            {
                //Do Nothing
                case 0:
                case 1:
                    break;

                case 2:
                    upgradePanel.SetActive(true);
                    break;

                default:
                    Debug.Log($"Inside GameUI. UI of Index : {uiIndex}, not available");
                    break;
            }
        }*/

        #region Prompt
        private void DisplayPrompt(byte promptIndex)
        {
            byte maskIndex = 0;
            switch (promptIndex)
            {
                case 0:
#if !MOBILE_MODE
                    promptTxt.text = "Press 'E' to carry.";
#else
                    interactButton.gameObject.SetActive(true);
                    maskIndex = 1 << 0;
                    buttonsToggleStatus |= maskIndex;
#endif
                    break;

                case 1:
                    promptTxt.text = "Collect \"Tokens\" to upgrade tower";
                    Invoke(nameof(DisablePrompt), 3f);
                    break;

                case 2:
                    if (GameManager.instance.towerCollected)
                    {
                        upgradeButton.gameObject.SetActive(true); 
                        maskIndex = 1 << 1;
                        buttonsToggleStatus |= maskIndex;
                    }
                    break;

                case 68:
                    upgradeButton.gameObject.SetActive(false);
                    maskIndex = 1 << 1;
                    buttonsToggleStatus &= (byte)~maskIndex;
                    break;

                case 69:
                    interactButton.gameObject.SetActive(false);
                    maskIndex = 1 << 0;
                    buttonsToggleStatus &= (byte)~maskIndex;
                    DisablePrompt();
                    break;

                default:
                    Debug.Log($"Inside GameUI. Prompt of Index : {promptIndex}, not available");
                    break;
            }

        }

        private void DisablePrompt()
        {
            promptTxt.text = string.Empty;
        }
        #endregion Prompt

        #region UI_Buttons

        //On ExitButton, under MainMenuCanvas/UI
        //On ExitButton, under ButtonContainer/BG/StatsPanel/GameOverCanvas/UI
        //On ExitButton, under ButtonContainer/BG/PausePanel/PauseCanvas/UI
        public void ExitGame()
        {
            Application.Quit();
        }

        //On the Interact button, under the Main Canvas
        public void ChangeButtonColor()
        {
            if (interactDone)
            {
                interactBtImg.color = Color.cyan;
                interactBtTxt.text = "Pick Up";
            }
            else
            {
                interactBtImg.color = Color.red;
                interactBtTxt.text = "Put Down";
            }

            interactDone = !interactDone;
        }

        //On HomeButton, under ButtonContainer/BG/StatsPanel/GameOverCanvas/UI
        public void RestartGame()
        {
            //localGameLogic.PauseGame(false);

            if (GameManager.instance.gamePaused)
            {
                Debug.Log($"Pause Menu");
                pauseCanvas_AC.Play("SwipeOver_Exit", 0);
            }
            else
                gameOverCanvas_AC.Play("SwipeOver_Exit", 0);

            Invoke(nameof(RestartHelper), 1.5f);
        }

        //On PauseBt, under GamePlayCanvas/UI
        public void PauseGame(bool pause)
        {
            if (pause)
            {
                localGameLogic.PauseGame(true); 
                pauseCanvas.gameObject.SetActive(true);
                //Invoke(nameof(PauseHelper), 1f); 
            }
            else
            {
                localGameLogic.PauseGame(false);
                pauseButton.gameObject.SetActive(true);
                pauseCanvas.gameObject.SetActive(false);
            }
        }

        private void PauseHelper() {}

        private void RestartHelper()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        }

        //On the power buttons, under PowerBtHolder/UpgradePanel/GameplayCanvas
        public void Chosenpower(int powerIndex)
        {
            localGameLogic.OnPowersSelected((byte)powerIndex);

            ref int upgradePrice = ref GameManager.instance.upgradePrices[powerIndex];
            ref int tokenCollected = ref GameManager.instance.tokenCollected;
            
            if (tokenCollected < upgradePrice)
                return;
            tokenCollected -= upgradePrice;
            upgradePrice += 2;
            powerPriceTxt[powerIndex].text = upgradePrice.ToString();
            tokenTxt.text = tokenCollected.ToString();

            CheckUpgradeButtonsStatus();
        }

        //On Upgrade button, under GameplayCanvas/UI
        public void OpenUpgradePanel()
        {
            //upgradeButton.gameObject.SetActive(false);
            if ((buttonsToggleStatus & (1 << 0)) != 0)         //Check if Interact buttons is still available
                interactButton.gameObject.SetActive(false);

            upgradePanel.SetActive(true);
            CheckUpgradeButtonsStatus();
        }

        //On Close button, under UpgradePanel/GameplayCanvas/UI
        public void CheckInteractStatus()
        {
            if ((buttonsToggleStatus & (1 << 0)) != 0)
                interactButton.gameObject.SetActive(true);
        }

        private void CheckUpgradeButtonsStatus()
        {
            //Check if the player has enough tokens availbale
            int tokenCollected = GameManager.instance.tokenCollected;
            for (int i = 0; i < GameManager.instance.upgradePrices.Length; i++)
            {
                int upgradePrice = GameManager.instance.upgradePrices[i];
                if (tokenCollected >= upgradePrice)
                    upgradeBts[i].interactable = true;
                else
                    upgradeBts[i].interactable = false;
            }
        }
        #endregion UI_Buttons

        private void EnableGameplayUI()
        {
            gameplayCanvas.gameObject.SetActive(true);
            mainCanvas.gameObject.SetActive(false);
        }
    }
}