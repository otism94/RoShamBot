using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoShamBot
{
    public class RPS : MonoBehaviour
    {
        public static RPS Instance { get; private set; }

        public enum Shoot { none, rock, paper, scissors }
        public enum Outcome { win, lose, draw }

        public GameObject rockSprite;
        public GameObject paperSprite;
        public GameObject scissorsSprite;
        public GameObject bubble;

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

        /// <summary>
        /// Determines the RPS outcome from the enemy's perspective.
        /// </summary>
        /// <param name="playerAttack">The player's RPS.Shoot option.</param>
        /// <param name="enemyAttack">The enemy's RPS.Shoot option.</param>
        /// <returns>An RPS.Outcome (win, lose, or draw).</returns>
        public static Outcome GetOutcome(Shoot playerAttack, Shoot enemyAttack)
        {
            // Player hits with rock
            if (playerAttack == Shoot.rock)
            {
                switch (enemyAttack)
                {
                    case Shoot.rock:
                        return Outcome.draw;
                    case Shoot.paper:
                        return Outcome.win;
                    case Shoot.scissors:
                        return Outcome.lose;
                }
            }

            // Player hits with paper
            else if (playerAttack == Shoot.paper)
            {
                switch (enemyAttack)
                {
                    case Shoot.rock:
                        return Outcome.lose;
                    case Shoot.paper:
                        return Outcome.draw;
                    case Shoot.scissors:
                        return Outcome.win;
                }
            }

            // Player hits with scissors
            else if (playerAttack == Shoot.scissors)
            {
                switch (enemyAttack)
                {
                    case Shoot.rock:
                        return Outcome.win;
                    case Shoot.paper:
                        return Outcome.lose;
                    case Shoot.scissors:
                        return Outcome.draw;
                }
            }

            return Outcome.lose;
        }

        public static void WinDefault(GameObject winner)
        {
            
        }

        /// <summary>
        /// Default outcome on a draw: the drawer gets knocked back slightly with no health loss.
        /// </summary>
        /// <param name="drawer">The player or enemy gameObject that drew.</param>
        public static void DrawDefault(GameObject drawer)
        {
            if (drawer.CompareTag("Player")) drawer.transform.Translate(new Vector2(-2, 0));
            else if (drawer.CompareTag("Enemy")) drawer.transform.Translate(new Vector2(2, 0));
        }

        /// <summary>
        /// Default outcome on a loss: the loser loses 1 health and is knocked back greatly.
        /// </summary>
        /// <param name="loser">The player or enemy gameObject that lost.</param>
        public static void LoseDefault(GameObject loser)
        {
            if (loser.CompareTag("Player") || loser.CompareTag("PlayerHurtbox"))
            {
                loser.transform.Translate(new Vector2(-3, 0));
                Player.Instance.ChangeHealthBy(-1);
                HealthDisplay.Instance.UpdateHealthDisplay();
            }
            else if (loser.CompareTag("Enemy")) 
            {
                loser.transform.Translate(new Vector2(3, 0));
                loser.GetComponent<EnemyObstacle>().currentHealth -= 1;
            }
        }
    }
}
