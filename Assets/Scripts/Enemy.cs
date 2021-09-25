using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoShamBot
{
    public abstract class Enemy : EnemyObstacle
    {
        [SerializeField, Tooltip("Whether the enemy should display their attack intent in a bubble.")]
        protected bool displayIntent = true;
        [SerializeField, Tooltip("Position offset of the intent bubble, relative to the enemy sprite.")]
        protected Vector2 bubbleOffset;
        [SerializeField, Tooltip("The distance the player must be from the enemy to initiate battle mode.")]
        protected float battleModeDistance = 4f;
        protected bool isStationary = true;
        protected Rigidbody2D RB;
        [SerializeField] protected float drawKnockback = 2;
        [SerializeField] protected float loseKnockback = 3;

        // Start is called before the first frame update
        protected override void Start() 
        {
            base.Start();
            RB = this.gameObject.GetComponent<Rigidbody2D>(); 
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();

            // Trigger battle mode if player is within battleModeDistance.
            if (!Player.Instance.battleMode.active && Vector2.Distance(this.transform.position, Player.Instance.gameObject.transform.position) <= battleModeDistance)
                Player.Instance.EnterBattleMode(this);

            // Check if the enemy is stationary or not.
            if (isStationary && RB.velocity.magnitude > 0.01f) isStationary = false;
            else if (!isStationary && RB.velocity.magnitude <= 0.01f) isStationary = true;
        }

        public bool Stationary => isStationary;

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
                RPS.Instance.bubble.transform.rotation,
                transform);
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
        /// Destroys all child objects of the Enemy. Intended to remove attack bubble and hand sprites.
        /// </summary>
        /// <param name="offset">(Optional) Number of children to skip over.</param>
        public void RemoveIntentBubble(int offset = 0)
        {
            for (int i = 0 + offset; i < transform.childCount; i++) Destroy(transform.GetChild(i).gameObject);
        }

        public override void Win() { return; }

        public override void Draw() => RB.AddForce(new Vector2(drawKnockback, 0), ForceMode2D.Impulse);

        public override void Lose() 
        { 
            RB.AddForce(new Vector2(loseKnockback, 0), ForceMode2D.Impulse);
            currentHealth -= 1;
        }
    }
}