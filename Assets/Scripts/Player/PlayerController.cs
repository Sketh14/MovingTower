#define MOBILE_MODE

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Moving_Tower
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        private const string TOWER_TAG = "Tower", UPGRADEPLATFORM_TAG = "UpgradePlatform",
            TOKEN_TAG = "Token";
        private bool towerCollected, interactAvailable;
        private byte towerCollectionStatus;
        private Transform towerTransform;
        [SerializeField] private Animator playerAnimator;

#if !MOBILE_MODE
        [Header("Input Actions")]
        private PlayerDefaultControls playerControls;
#endif

        [Header("Local Reference Scripts")]
        [SerializeField] private GameLogic localGameLogic;

        [Header ("Movement Controls")]
        [SerializeField] private Rigidbody playerRB;
        [SerializeField] private Vector2 moveVector;
        [SerializeField] private float moveSpeedMultiplier = 2f;
        private bool move;

        [Header("UI")]
        [SerializeField] private FloatingJoystick joystick;

        private void OnEnable()
        {
#if !MOBILE_MODE
            playerControls.Enable();
#endif

            localGameLogic.OnGameplayStart += PutBookDown;
            localGameLogic.OnCastleReached += 
                () => {
                    InteractWithTower();        //Uncouple Tower
                    this.enabled = false; };      //Just disable the Controller
        }

        private void OnDisable()
        {
#if !MOBILE_MODE
            playerControls.Disable();
#endif

            localGameLogic.OnGameplayStart -= PutBookDown;
        }

        private void Awake()
        {
#if !MOBILE_MODE
            playerControls = new PlayerDefaultControls();
            playerControls.Player.Move.performed += Move;
            playerControls.Player.Move.canceled += Move;
            playerControls.Player.Interact.performed += (context) => { CoupleTower(); };
#endif
            //playerControls.Player.Interact.canceled += CoupleTower;
        }

        private void Update()
        {
            {
                //if (Keyboard.current.eKey.wasPressedThisFrame)
                //{
                //    interactAvailable = true;
                //    Debug.Log("Key-E was pressed");
                //}
                //else
                //    interactAvailable = false;
            }

#if !MOBILE_MODE
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                if (interactAvailable && towerCollectionStatus == 0)
                {
                    towerCollected = true;
                    localGameLogic.OnTowerCollected?.Invoke(towerCollected);
                    towerTransform.parent.parent = transform;
                    towerCollectionStatus = 1;
                    StartCoroutine(UpdateTowerStatus(2));
                }
            }
#endif
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            if (!GameManager.instance.gameStarted)
                return;

#if !MOBILE_MODE
            if (move)
            {
                //playerRB.velocity = new Vector3(moveVector.x, playerRB.velocity.y, moveVector.y) * moveSpeedMultiplier;
                //transform.Translate(new Vector3(moveVector.x, 0, moveVector.y) * moveSpeedMultiplier);
                playerRB.AddForce(new Vector3(moveVector.x, 0, moveVector.y) * moveSpeedMultiplier, ForceMode.Impulse);
            }
#endif
                        
            playerRB.AddForce(new Vector3(joystick.Direction.x, 0, joystick.Direction.y) * moveSpeedMultiplier, ForceMode.Impulse);
        }

        #region PC_CONTROLS
#if !MOBILE_MODE
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
                move = false;
                playerRB.velocity = Vector3.zero;
                //moveVector = context.ReadValue<Vector2>() * -3f;
                Invoke("DisableMove", 0.1f);
            }
        }

        private void DisableMove()
        {
            move = false;
        }
#endif
        #endregion PC_CONTROLS

#region TowerFunctions
        //On the Interact button, under Main Canvas
        public void InteractWithTower()
        {
            if (interactAvailable && towerCollectionStatus == 0 && !GameManager.instance.enemyReachedCastle)
            {
                //Debug.Log($"Coupling Tower");
                towerCollected = true;
                localGameLogic.OnTowerCollected?.Invoke(towerCollected);
                GameManager.instance.towerCollected = true;
                towerTransform.parent.parent = transform;
                towerCollectionStatus = 1;
                StartCoroutine(UpdateTowerStatus(2));
            }
            else
                UnCoupleTower();
        }

        private void UnCoupleTower()
        {
            if (towerCollectionStatus == 2)
            {
                towerCollected = false;
                GameManager.instance.towerCollected = false;
                transform.GetChild(1).parent = null;                        //Since the tower will be the 1th child of the player
                towerCollectionStatus = 3;
                StartCoroutine(UpdateTowerStatus(0, true));

                if (!GameManager.instance.enemyReachedCastle)
                    localGameLogic.OnTowerCollected?.Invoke(towerCollected);
                //Debug.Log($"Setting Tower Free");
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
#endregion TowerFunctions

#region TriggerFunctions
        private void OnTriggerStay(Collider collidedObject)
        {
            //Debug.Log($"Trigger Found : {collidedObject.name}");
            //Show Interact button
            if (towerCollectionStatus == 0 && collidedObject.CompareTag(TOWER_TAG))   //Called twice if pressed even once
            {
                localGameLogic.OnInteraction?.Invoke(0);
                interactAvailable = true;
                towerTransform = collidedObject.transform;
                //Debug.Log($"Tower Trigger Found : {collidedObject.name}, Tower Collected : {towerCollected}");
            }

            //Show upgrade button
            if (collidedObject.CompareTag(UPGRADEPLATFORM_TAG))
            {
                //Debug.Log("Upgrade PLatform found");
                localGameLogic.OnInteraction?.Invoke(2);
            }
        }

        private void OnTriggerExit(Collider collidedObject)
        {
            //Disable Interact button
            if (collidedObject.CompareTag(TOWER_TAG))
            {
                localGameLogic.OnInteraction?.Invoke(69);
                interactAvailable = false;
                towerTransform = null;
            }

            //Disable upgrade button
            if (collidedObject.CompareTag(UPGRADEPLATFORM_TAG))
            {
                //Debug.Log("Upgrade PLatform found");
                localGameLogic.OnInteraction?.Invoke(68);
            }
        }

        private void OnTriggerEnter(Collider collidedObject)
        {
            if (collidedObject.CompareTag(TOKEN_TAG))
            {
                collidedObject.gameObject.SetActive(false);
                GameManager.instance.tokenCollected++; 
                localGameLogic.OnCollectibleCollected?.Invoke(0);
            }
        }
#endregion TriggerFunctions

#region IntroSequence
        private void PutBookDown()
        {
            playerAnimator.Play("PlaceBook", 0);
            //Invoke(nameof(EnablePlayerForGameplay), 2f);
        }

        //On the player animtor timeline, under place book anim
        public void DetachBook()
        {
            Transform book = transform.GetChild(1);
            book.transform.localPosition = new Vector3(-2.5f, -1.78f, -2.38f);
            book.SetParent(null);
        }

        //On the player animtor timeline, under stand up anim
        public void EnablePlayerForGameplay()
        {
            transform.position = new Vector3(-9.93f, 10.67f, 38.4f);
            transform.GetChild(0).localPosition = new Vector3(0f, -1.25f, 0f);

            transform.GetComponent<Collider>().enabled = true;
            playerRB.isKinematic = false;
            playerRB.useGravity = true;

            GameManager.instance.gameStarted = true;
            playerAnimator.enabled = false;
        }
#endregion IntroSequence
    }
}