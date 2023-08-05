#define MOBILE_MODE

using UnityEngine;
using UnityEngine.UI;

namespace Moving_Tower
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private TMPro.TMP_Text promptTxt, interactBtTxt;
        [SerializeField] private Transform turretPerimeter;
        [SerializeField] private Button interactButton;
        [SerializeField] private Image interactBtImg;

        [Header("Local Reference Scritps")]
        [SerializeField] private GameLogic localGameLogic;
        [SerializeField] private GunControls localGunControls;      //Test variable

        [Space]
        private bool interactDone;

        private void OnEnable()
        {
            localGameLogic.OnInteraction += DisplayPrompt;
        }

        private void OnDisable()
        {
            localGameLogic.OnInteraction -= DisplayPrompt;
        }

        private void Start()
        {
            InvokeRepeating(nameof(UpdatePerimeterHiglightArea), 0f, 2f);
        }

        private void UpdatePerimeterHiglightArea()
        {
            float spriteScale = (1.5f * localGunControls.turretRange) + 2.5f;          //y=1.5x+2.5
            Vector3 tempScale = new Vector3(spriteScale, spriteScale, 1f);

            turretPerimeter.transform.localScale = tempScale;
        }

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

                case 69:
                    DisablePrompt();
                    break;

                default:
                    Debug.Log($"Inside GameUI. Prompt of Index : {promptIndex}, not available");
                    break;
            }

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

        private void DisablePrompt()
        {
#if !MOBILE_MODE
            promptTxt.text = string.Empty;
#else
            interactButton.gameObject.SetActive(false);
#endif
        }
    }
}