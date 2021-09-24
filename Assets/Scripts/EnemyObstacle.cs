using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoShamBot
{
    public abstract class EnemyObstacle : MonoBehaviour
    {
        [HideInInspector]
        public RPS.Shoot enemyAttack;
        [Tooltip("Position offset of the intent bubble, relative to the enemy sprite.")]
        public Vector2 bubbleOffset;
        [Tooltip("An RPS option the enemy is guaranteed to choose.")]
        public RPS.Shoot fixedShoot;
        [Tooltip("Limit possible enemy attacks to one of these options.")]
        public List<RPS.Shoot> includeShoots;
        public bool displayIntent = true;
        public int initialHealth = 1;
        [HideInInspector]
        public int currentHealth;
        public bool defeated;
        public bool isStationary = true;
        private float lastFrameRotation = 0;
        private Vector3 lastFramePosition;

        protected virtual void Start()
        {
            lastFramePosition = this.gameObject.transform.position;
            isStationary = true;
            currentHealth = initialHealth;
            if (fixedShoot != RPS.Shoot.none) SetAttack(fixedShoot);
            else if (includeShoots.Count > 0) SetAttack(includeShoots);
            else SetAttack();
        }

        protected virtual void Update() 
        {
            if (currentHealth == 0 && !defeated)
            {
                defeated = true;
                Defeated();
            }

            if (isStationary && this.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude > 0.01f) isStationary = false;
            else if (!isStationary && this.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude <= 0.01f) isStationary = true;
            lastFrameRotation = this.gameObject.transform.rotation.z;
            lastFramePosition = this.gameObject.transform.position;
        } 

        /// <summary>
        /// Sets the enemy's attack to a random RPS.Shoot.
        /// </summary>
        public void SetAttack()
        {
            if (fixedShoot != RPS.Shoot.none) 
            {
                SetAttack(fixedShoot);
                return;
            }
            if (includeShoots.Count > 0)
            {
                SetAttack(includeShoots);
                return;
            }
            else enemyAttack = (RPS.Shoot)Random.Range(1, 4);
        }

        /// <summary>
        /// Sets the enemy's attack to the provided RPS.Shoot.
        /// </summary>
        /// <param name="fixedShoot">RPS.Shoot to set.</param>
        protected void SetAttack(RPS.Shoot fixedShoot) => enemyAttack = fixedShoot;

        /// <summary>
        /// Sets the enemy's attack to one RPS.Shoot in the provided collection.
        /// </summary>
        /// <param name="includeShoots">Collection of RPS.Shoot options the enemy can pick from.</param>
        protected void SetAttack(List<RPS.Shoot> includeShoots)
        {
            do enemyAttack = (RPS.Shoot)Random.Range(1, 4);
            while (!includeShoots.Contains(enemyAttack));
        }

        /// <summary>
        /// Displays the enemy's attack in a bubble.
        /// </summary>
        public void DisplayIntent()
        {
            // Child gameObjects are intent bubble sprites, so remove them before instantiating a new bubble.
            if (transform.childCount > 0) RemoveIntentBubble();

            // Instantiate a bubble, flip it, and shade it red.
            GameObject bubble = Instantiate(
                RPS.Instance.bubble, 
                new Vector2(transform.position.x + bubbleOffset.x, transform.position.y + bubbleOffset.y), 
                RPS.Instance.bubble.transform.rotation);
            bubble.transform.parent = transform;
            bubble.GetComponent<SpriteRenderer>().flipX = true;
            bubble.GetComponent<SpriteRenderer>().color = new Color(1, .6f, .6f);

            // Determine handSprite based on enemyAttack.
            GameObject handSprite = enemyAttack switch
            {
                RPS.Shoot.rock => RPS.Instance.rockSprite,
                RPS.Shoot.paper => RPS.Instance.paperSprite,
                RPS.Shoot.scissors => RPS.Instance.scissorsSprite,
                _ => null
            };

            // Early return if no attack selected.
            if (handSprite == null) return;

            // Set the enemy's hand sprite.
            handSprite = Instantiate(handSprite, 
                new Vector2(transform.position.x + bubbleOffset.x, transform.position.y + bubbleOffset.y), 
                RPS.Instance.bubble.transform.rotation);
            handSprite.transform.SetParent(transform);
            handSprite.GetComponent<SpriteRenderer>().flipX = true;
        }

        /// <summary>
        /// Destroys all child objects of the EnemyObstacle. Intended to remove attack bubble and hand sprites.
        /// </summary>
        /// <param name="offset">(Optional) Number of children to skip over.</param>
        public void RemoveIntentBubble(int offset = 0)
        {
            for (int i = 0 + offset; i < transform.childCount; i++) Destroy(transform.GetChild(i).gameObject);
        }

        public abstract void ClearObstacle();

        protected abstract void OnTriggerEnter2D(Collider2D collision);

        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("PlayerHurtbox"))
            {
                RPS.LoseDefault(Player.Instance.gameObject);
                Player.Instance.ResetAttackType();
                if (fixedShoot != RPS.Shoot.none) SetAttack(fixedShoot);
                else if (includeShoots.Count != 0) SetAttack(includeShoots);
                else SetAttack();
                if (displayIntent) DisplayIntent();
            }
            if (!isStationary && collision.gameObject.CompareTag("Ground")) isStationary = true;
        }

        protected void OnCollisionStay2D(Collision2D collision)
        {
            if (!isStationary && collision.gameObject.CompareTag("Ground")) isStationary = true;
        }

        protected void OnCollisionExit2D(Collision2D collision)
        {
            if (isStationary && collision.gameObject.CompareTag("Ground")) isStationary = false;
        }

        public abstract void Defeated();

        protected virtual void OnDestroy()
        {
            this.gameObject.GetComponent<OnDestroyEvent>()?.Event();
        }
    }
}