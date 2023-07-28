using UnityEngine;

namespace Moving_Tower
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private TMPro.TMP_Text promptTxt;
        [SerializeField] private Transform turretPerimeter;

        [Header("Local Reference Scritps")]
        [SerializeField] private GameLogic localGameLogic;
        [SerializeField] private GunControls localGunControls;      //Test variable

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
                    promptTxt.text = "Press 'E' to carry.";
                    break;

                case 69:
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
    }
}