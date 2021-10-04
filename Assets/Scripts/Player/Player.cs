using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        public bool isMoving = false;
        public bool isCatchingUp = false;
        private Coroutine catchUpCoroutine;
        private Vector2 movementDirection;

        [Header("Attacks")]
        [SerializeField] private GameObject hitbox;
        private Rigidbody2D RB;
        [SerializeField] private float attackActiveTime = 1;
        [SerializeField] private Vector2 bubbleOffset;
        [HideInInspector] public RPS.Shoot currentAttackType = RPS.Shoot.none;
        public BattleMode battleMode;
        [SerializeField] private float drawKnockback = -2f;
        [SerializeField] private float loseKnockback = -3f;


        [Header("Inputs")]
        [SerializeField] private KeyCode rockInput = KeyCode.Q;
        [SerializeField] private KeyCode paperInput = KeyCode.W;
        [SerializeField] private KeyCode scissorsInput = KeyCode.E;
        private KeyCode[] attackInputs;
        private Coroutine attackCoroutine;

        [Header("SFX")]
        [SerializeField] private AudioClip winSFX;
        [SerializeField] private AudioClip drawSFX;
        [SerializeField] private AudioClip loseSFX;

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
            //DontDestroyOnLoad(this.gameObject);
        }

        // Start is called before the first frame update
        void Start()
        {
            RB = this.gameObject.GetComponent<Rigidbody2D>();
            battleMode.active = false;
            movementDirection = Vector2.right;
            isMoving = false;
            attackInputs = new KeyCode[] { rockInput, paperInput, scissorsInput };
            currentHealth = initialHealth;
            speed = initialSpeed;
        }

        // Update is called once per frame
        void Update()
        {
            if (currentHealth > 0)
            {
                if (!battleMode.active && isMoving)
                {
                    // Move the player
                    this.gameObject.transform.Translate(speed * Time.deltaTime * movementDirection);

                    // Check for one of the three inputs
                    HandleInput();

                    if (!isCatchingUp && isMoving && CameraController.Instance.isMoving && this.transform.position.x + 1.5f < CameraController.Instance.gameObject.transform.position.x)
                    {
                        isCatchingUp = true;
                        catchUpCoroutine = StartCoroutine(CatchUpToCamera());
                    }

                    if (isCatchingUp && isMoving && CameraController.Instance.isMoving && this.transform.position.x + 1.5f >= CameraController.Instance.gameObject.transform.position.x)
                    {
                        isCatchingUp = false;
                        speed = initialSpeed;
                    }
                }
                else
                {
                    if (catchUpCoroutine != null) StopCoroutine(catchUpCoroutine);
                    catchUpCoroutine = null;
                    isMoving = isCatchingUp = false;
                }
            }
            else Defeat();
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
        private void Defeat()
        {
            StopAllCoroutines();
            ResetAttackType();
            CameraController.Instance.isMoving = false;
            isMoving = isCatchingUp = false;

            SpriteRenderer[] spriteRenderers = this.gameObject.transform.GetComponentsInChildren<SpriteRenderer>();
            foreach (var spriteRenderer in spriteRenderers) spriteRenderer.color = Color.clear;

            if (battleMode.active) ExitBattleMode(BattleMode.Instance.gameObject);
            Level.Instance.GameOver();

            //SceneManager.LoadScene("Title");
        }

        public void HandleInput()
        {
            foreach (KeyCode key in attackInputs)
            {
                // If not already attacking, set attack state to match the input and instantiate bubble
                if (Input.GetKeyDown(key) && currentAttackType == RPS.Shoot.none) 
                {
                    GameObject bubble = Instantiate(
                        RPS.Instance.bubble, 
                        new Vector2(transform.position.x + bubbleOffset.x, transform.position.y + bubbleOffset.y),
                        RPS.Instance.bubble.transform.rotation,
                        transform);
                    bubble.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
                    Instantiate(hitbox, transform);
                    SetAttackType(key);
                    attackCoroutine = StartCoroutine(ResetAttackTypeDelayed());
                }
            }
        }

        public void EnterBattleMode(Enemy enemy)
        {
            isMoving = false;
            isCatchingUp = false;
            battleMode.enemy = enemy;
            battleMode.active = true;
            GameObject battleModeInstance = Instantiate(battleMode.gameObject);
            battleModeInstance.transform.position = CameraController.Instance.transform.position;
        }

        public void ExitBattleMode(GameObject battleModeInstance)
        {
            battleMode.active = false;
            Destroy(battleModeInstance);
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
                new Vector2(transform.position.x + bubbleOffset.x, transform.position.y + bubbleOffset.y), 
                RPS.Instance.bubble.transform.rotation);
            handSprite.transform.SetParent(transform);
        }

        /// <summary>
        /// Resets the attack type to none and destroys associated bubble/hand sprites.
        /// </summary>
        /// <param name="offset">(Optional) Number of children to skip over.</param>
        public void ResetAttackType()
        {
            // Confirm that attack type hasn't already been reset by an interaction.
            if (currentAttackType == RPS.Shoot.none) return;

            // Reset attack type and destroy bubble and hand sprites.
            currentAttackType = RPS.Shoot.none;
            for (int i = 1; i < transform.childCount; i++) Destroy(transform.GetChild(i).gameObject);
        }

        /// <summary>
        /// Resets the attack type to none and destroys associated bubble/hand sprites after a set delay.
        /// </summary>
        /// <returns>IEnumerator WaitForSeconds (Player.attackActiveTime).</returns>
        public IEnumerator ResetAttackTypeDelayed()
        {
            yield return new WaitForSeconds(attackActiveTime);

            // Confirm that attack type hasn't already been reset by an interaction.
            if (currentAttackType == RPS.Shoot.none) yield break;

            // Reset attack type and destroy bubble and hand sprites.
            currentAttackType = RPS.Shoot.none;
            for (int i = 1; i < transform.childCount; i++) Destroy(transform.GetChild(i).gameObject);
        }

        /// <summary>
        /// Resets the attack type to none and destroys associated bubble/hand sprites after a provided delay.
        /// </summary>
        /// <param name="wait">Time to wait for in seconds.</param>
        /// <returns>IEnumerator WaitForSeconds (Player.attackActiveTime).</returns>
        public IEnumerator ResetAttackTypeDelayed(float wait)
        {
            if (attackCoroutine != null) StopCoroutine(attackCoroutine);
            yield return new WaitForSeconds(wait);

            // Confirm that attack type hasn't already been reset by an interaction.
            if (currentAttackType == RPS.Shoot.none) yield break;

            // Reset attack type and destroy bubble and hand sprites.
            currentAttackType = RPS.Shoot.none;
            for (int i = 1; i < transform.childCount; i++) Destroy(transform.GetChild(i).gameObject);
        }

        private IEnumerator RunningKnockback()
        {
            if (catchUpCoroutine != null)
            {
                StopCoroutine(catchUpCoroutine);
                catchUpCoroutine = null;
            }
            isMoving = isCatchingUp = false;
            yield return new WaitForSeconds(1f);
            if (!battleMode.active) isMoving = true;
        }

        public void Win() 
        {
            Audio.Instance.Source.PlayOneShot(winSFX, 0.5f);
            StartCoroutine(ResetAttackTypeDelayed(.5f));
        }

        public void Draw()
        {
            Audio.Instance.Source.PlayOneShot(drawSFX, 1f);
            StartCoroutine(RunningKnockback());
            RB.AddForce(new Vector2(drawKnockback, 0), ForceMode2D.Impulse);
            StartCoroutine(ResetAttackTypeDelayed(.5f));
        }

        public void Draw(float overrideKnockback = 0) 
        {
            Audio.Instance.Source.PlayOneShot(drawSFX, 1f);
            StartCoroutine(RunningKnockback());
            RB.AddForce(new Vector2(overrideKnockback, 0), ForceMode2D.Impulse);
            StartCoroutine(ResetAttackTypeDelayed(.5f));
        }

        public void Lose()
        {
            Audio.Instance.Source.PlayOneShot(loseSFX, 0.9f);
            currentHealth -= 1;
            StartCoroutine(FlashRed());
            UI.Instance.healthUI.UpdateHealthDisplay();
            StartCoroutine(RunningKnockback());
            RB.AddForce(new Vector2(loseKnockback, 0), ForceMode2D.Impulse);
            StartCoroutine(ResetAttackTypeDelayed(.5f));
        }

        public void Lose(float overrideKnockback = 0)
        {
            Audio.Instance.Source.PlayOneShot(loseSFX, 0.9f);
            currentHealth -= 1;
            StartCoroutine(FlashRed());
            UI.Instance.healthUI.UpdateHealthDisplay();
            StartCoroutine(RunningKnockback());
            RB.AddForce(new Vector2(overrideKnockback, 0), ForceMode2D.Impulse);
            StartCoroutine(ResetAttackTypeDelayed(.5f));
        }

        public IEnumerator CatchUpToCamera()
        {
            yield return new WaitForSeconds(3f);
            if (!isCatchingUp) yield break;
            Debug.Log("Catching up...");
            speed = initialSpeed + .5f;
        }

        public IEnumerator FlashRed()
        {
            SpriteRenderer[] spriteRenderers = this.gameObject.transform.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < 3; i++)
            {
                foreach (var spriteRenderer in spriteRenderers) spriteRenderer.color = Color.red;
                yield return new WaitForSeconds(0.05f);
                foreach (var spriteRenderer in spriteRenderers) spriteRenderer.color = Color.white;
                yield return new WaitForSeconds(0.05f);
            }
            
        }

        #endregion
    }
}