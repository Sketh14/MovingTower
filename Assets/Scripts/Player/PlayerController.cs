using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Moving_Tower
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        private const string TOWER_TAG = "Tower";
        private bool towerCollected, interactClicked;
        private byte towerCollectionStatus;

        [Header("Input Actions")]
        private PlayerDefaultControls playerControls;

        [Header("Local Reference Scripts")]
        [SerializeField] private GameLogic localGameLogic;

        [Header ("Movement Controls")]
        [SerializeField] private Rigidbody playerRB;
        [SerializeField] private Vector2 moveVector;
        [SerializeField] private float moveSpeedMultiplier = 2f;
        private bool move;

        private void OnEnable()
        {
            playerControls.Enable();
        }

        private void OnDisable()
        {
            playerControls.Disable();
        }

        private void Awake()
        {
            playerControls = new PlayerDefaultControls();
            playerControls.Player.Move.performed += Move;
            playerControls.Player.Move.canceled += Move;
            playerControls.Player.Interact.performed += CoupleTower;
            //playerControls.Player.Interact.canceled += CoupleTower;
        }

        private void Update()
        {

        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            if (move)
                playerRB.velocity = new Vector3(moveVector.x, 0f, moveVector.y) * moveSpeedMultiplier;
        }

        private void Move(InputAction.CallbackContext context)
        {
            //Debug.Log($"Context : {context.control.name}, Value : {context.ReadValue<Vector2>()}");

            if (context.phase.Equals(InputActionPhase.Performed))
            {
                moveVector = context.ReadValue<Vector2>();
                move = true;
            }
            else if (context.phase.Equals(InputActionPhase.Canceled))
            {
                moveVector = Vector2.zero;
                //moveVector = context.ReadValue<Vector2>() * -3f;
                Invoke("DisableMove", 0.1f);
            }
        }

        private void CoupleTower(InputAction.CallbackContext context)
        {
            if (towerCollectionStatus == 2)
            {
                transform.GetChild(1).parent = null;                        //Since the tower will be the 1th child of the player
                towerCollectionStatus = 3;
                StartCoroutine(UpdateTowerStatus(0, true));
                Debug.Log($"Setting Tower Free");
            }
        }

        private IEnumerator UpdateTowerStatus(byte status = 0, bool reset = false)
        {
            yield return new WaitForSeconds(0.5f);

            if (reset)
                towerCollectionStatus = 0;
            else
                towerCollectionStatus = status;
        }

        private void DisableMove()
        {
            move = false;
        }

        private void OnTriggerStay(Collider collidedObject)
        {
            //Debug.Log($"Trigger Found : {collidedObject.name}");
            if (towerCollectionStatus == 0 && collidedObject.CompareTag(TOWER_TAG))   //Called twice if pressed even once
            {
                localGameLogic.OnInteraction?.Invoke(0);

                if (Keyboard.current.fKey.wasPressedThisFrame)
                {
                    localGameLogic.OnTowerCollected?.Invoke(towerCollected);
                    collidedObject.transform.parent.parent = transform;
                    towerCollectionStatus = 1;
                    StartCoroutine(UpdateTowerStatus(2));

                    Debug.Log($"Tower Trigger Found : {collidedObject.name}, Tower Collected : {towerCollected}");
                }
            }
        }

        private void OnTriggerExit(Collider collidedObject)
        {
            if (collidedObject.CompareTag(TOWER_TAG))
                localGameLogic.OnInteraction?.Invoke(69);
        }

        private void SetTowerParent()
        {

        }
    }
}