using UnityEngine;
using UnityEngine.InputSystem;

namespace Moving_Tower
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        private const string TOWER_TAG = "Tower";
        private bool enableTowerToMove;

        [Header("Input Actions")]
        private PlayerDefaultControls playerControls;

        [Header("Local Reference Scripts")]
        [SerializeField] private GameLogic localGameLogic;

        [Space]
        [SerializeField] private Rigidbody playerRB;
        [SerializeField] private Vector2 moveVector;

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
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        private void Update()
        {
        }

        private void Move(InputAction.CallbackContext context)
        {
            Debug.Log($"Context : {context.control.name}, Value : {context.ReadValue<Vector2>()}");
            moveVector = context.ReadValue<Vector2>();
            playerRB.velocity = new Vector3(moveVector.x, 0f, moveVector.y);
        }

        private void OnTriggerStay(Collider collidedObject)
        {
            if (collidedObject.CompareTag(TOWER_TAG) && Keyboard.current.fKey.wasPressedThisFrame)
            {
                //Debug.Log($"Trigger Found : {collidedObject.name}");
                collidedObject.transform.parent.parent = transform;
                localGameLogic.OnTowerCollected?.Invoke();
            }
        }
    }
}