using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoShamBot
{
    public class Player : MonoBehaviour
    {
        #region Data

        // Instance
        public static Player Instance;

        [Header("Health")]
        [SerializeField] private int initialHealth = 3;
        [SerializeField] private int currentHealth;

        [Header("Movement")]
        [SerializeField] private float initialSpeed = 5f;
        private float speed;
        public bool isMoving = true;
        public bool isCatchingUp = false;
        private Vector2 movementDirection;

        [Header("Attacks")]
        [SerializeField] private GameObject playerHitbox;
        [SerializeField] public GameObject playerSprite;
        [SerializeField] private float attackActiveTime = 1;
        [SerializeField] private Vector2 bubbleOffset;
        [HideInInspector] public RPS.Shoot currentAttackType = RPS.Shoot.none;
        public BattleMode battleMode;

        [Header("Inputs")]
        [SerializeField] private KeyCode rockInput = KeyCode.Q;
        [SerializeField] private KeyCode paperInput = KeyCode.W;
        [SerializeField] private KeyCode scissorsInput = KeyCode.E;
        private KeyCode[] attackInputs;

        #endregion

        #region Setup & Update

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this.gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        // Start is called before the first frame update
        void Start()
        {
            playerSprite = this.gameObject;
            battleMode.active = false;
            movementDirection = Vector2.right;
            isMoving = true;
            attackInputs = new KeyCode[] { rockInput, paperInput, scissorsInput };
            currentHealth = initialHealth;
            speed = initialSpeed;
        }

        // Update is called once per frame
        void Update()
        {
            if (currentHealth > 0)
            {
                if (!battleMode.active)
                {
                    // Move the player
                    playerSprite.transform.Translate(speed * Time.deltaTime * movementDirection);

                    // Check for one of the three inputs
                    HandleInput();

                    if (!isCatchingUp && isMoving && CameraController.Instance.isMoving && this.transform.position.x + 1 < CameraController.Instance.gameObject.transform.position.x)
                    {
                        isCatchingUp = true;
                        StartCoroutine(CatchUpToCamera());
                    }

                    if (isCatchingUp && isMoving && CameraController.Instance.isMoving && this.transform.position.x + 1 >= CameraController.Instance.gameObject.transform.position.x)
                    {
                        isCatchingUp = false;
                        speed = initialSpeed;
                    }
                }
                else
                {
                    isMoving = isCatchingUp = false;
                }
            }
            else HandleDeath();
        }

        #endregion

        #region Getters & Setters

        public int InitialHealth => this.initialHealth;

        public int Health => this.currentHealth;

        /// <summary>
        /// Sets the player's health to the passed in value.
        /// </summary>
        /// <param name="value">The value to set the player's health to.</param>
        public void SetHealthTo(int value) => this.currentHealth = value;

        /// <summary>
        /// Changes the player's health by the passed in value.
        /// </summary>
        /// <param name="value">Positive or negative value.</param>
        public void ChangeHealthBy(int value) => this.currentHealth += value;

        public float InitialSpeed => this.initialSpeed;

        public float Speed => this.speed;

        public void SetSpeed(float value) => this.speed = value;

        public RPS.Shoot Attack => this.currentAttackType;

        #endregion

        #region Methods

        /// <summary>
        /// Resets attack type, stops the camera, and destroys the player gameObject.
        /// </summary>
        private void HandleDeath()
        {
            ResetAttackType();
            CameraController.Instance.isMoving = false;
            Destroy(this.gameObject);
        }

        public void HandleInput()
        {
            foreach (KeyCode key in attackInputs)
            {
                // If not already attacking, set attack state to match the input and instantiate bubble
                if (Input.GetKeyDown(key) && currentAttackType == RPS.Shoot.none) 
                {
                    GameObject bubble = Instantiate(RPS.Instance.bubble, new Vector2(playerSprite.transform.position.x + bubbleOffset.x, playerSprite.transform.position.y + bubbleOffset.y), RPS.Instance.bubble.transform.rotation);
                    bubble.transform.parent = transform;
                    GameObject hitbox = Instantiate(playerHitbox, playerSprite.transform);
                    hitbox.transform.parent = transform;
                    SetAttackType(key);
                    StartCoroutine(ResetAttackTypeDelayed());
                }
            }
        }

        public void EnterBattleMode(EnemyObstacle enemy)
        {
            isMoving = false;
            isCatchingUp = false;
            battleMode.enemy = enemy;
            battleMode.active = true;
            Instantiate(battleMode.gameObject);
        }

        public void ExitBattleMode(GameObject battleModeObj)
        {
            battleMode.active = false;
            Destroy(battleModeObj);
            isMoving = true;
        }

        public IEnumerator Knockback()
        {
            isMoving = false;
            yield return new WaitForSeconds(0.1f);
            isMoving = true;
        }

        /// <summary>
        /// Sets the player's attack according to keyboard input.
        /// </summary>
        /// <param name="input">The player's input.</param>
        private void SetAttackType(KeyCode input)
        {
            GameObject handSprite = null;
            if (input == rockInput)
            {
                currentAttackType = RPS.Shoot.rock;
                handSprite = RPS.Instance.rockSprite;
            }
            else if (input == paperInput)
            {
                currentAttackType = RPS.Shoot.paper;
                handSprite = RPS.Instance.paperSprite;
            }
            else if (input == scissorsInput) 
            { 
                currentAttackType = RPS.Shoot.scissors;
                handSprite = RPS.Instance.scissorsSprite;
            }

            // Early return if something goes wrong.
            if (handSprite == null) return;

            // Instantiate the hand sprite and set it as a child of the player object.
            handSprite = Instantiate(
                handSprite, 
                new Vector2(playerSprite.transform.position.x + bubbleOffset.x, playerSprite.transform.position.y + bubbleOffset.y), 
                RPS.Instance.bubble.transform.rotation);
            handSprite.transform.SetParent(transform);
        }

        /// <summary>
        /// Resets the attack type to none and destroys associated bubble/hand sprites.
        /// </summary>
        /// <param name="offset">(Optional) Number of children to skip over.</param>
        public void ResetAttackType(int offset = 0)
        {
            // Confirm that attack type hasn't already been reset by an interaction.
            if (currentAttackType == RPS.Shoot.none) return;

            // Reset attack type and destroy bubble and hand sprites.
            currentAttackType = RPS.Shoot.none;
            for (int i = 0 + offset; i < transform.childCount; i++) Destroy(transform.GetChild(i).gameObject);
        }

        /// <summary>
        /// Resets the attack type to none and destroys associated bubble/hand sprites after a delay.
        /// </summary>
        /// <returns>IEnumerator WaitForSeconds (Player.attackActiveTime).</returns>
        public IEnumerator ResetAttackTypeDelayed(int offset = 0)
        {
            yield return new WaitForSeconds(attackActiveTime);

            // Confirm that attack type hasn't already been reset by an interaction.
            if (currentAttackType == RPS.Shoot.none) yield break;

            // Reset attack type and destroy bubble and hand sprites.
            currentAttackType = RPS.Shoot.none;
            for (int i = 0 + offset; i < transform.childCount; i++) Destroy(transform.GetChild(i).gameObject);
        }

        public IEnumerator CatchUpToCamera()
        {
            yield return new WaitForSeconds(3f);
            if (!isCatchingUp) yield break;
            Debug.Log("Catching up...");
            this.speed += .5f;
        }

        #endregion
    }
}