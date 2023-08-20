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
        [SerializeField] private Canvas mainCanvas, gameplayCanvas;
        [SerializeField] private GameObject upgradePanel, levelTransitionUI;
        [SerializeField] private Animator effectsCanvasAnimator;

        [Header("Buttons")]
        [SerializeField] private Image interactBtImg;
        [SerializeField] private Button interactButton, upgradeButton;
        [SerializeField] private Button[] upgradeBts;

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
            switch(promptIndex)
            {
                case 0:
#if !MOBILE_MODE
                    promptTxt.text = "Press 'E' to carry.";
#else
                    interactButton.gameObject.SetActive(true);
#endif
                    break;

                case 1:
                    promptTxt.text = "Collect \"Tokens\" to upgrade tower";
                    Invoke(nameof(DisablePrompt), 3f);
                    break;

                case 2:
                    upgradeButton.gameObject.SetActive(true);
                    break;

                case 68:
                    upgradeButton.gameObject.SetActive(false);
                    break;

                case 69:
                    interactButton.gameObject.SetActive(false);
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
        }

        //On Upgrade button, under GameplayCanvas/UI
        public void OpenUpgradePanel()
        {
            upgradePanel.SetActive(true);

            //Check if the player has enough tokens availbale
            int tokenCollected = GameManager.instance.tokenCollected;
            for (int i = 0; i < GameManager.instance.upgradePrices.Length; i++)
            {
                int upgradePrice = GameManager.instance.upgradePrices[i];
                if (tokenCollected >= upgradePrice)
                    upgradeBts[i].interactable = true;
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