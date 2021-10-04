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
            //DontDestroyOnLoad(this.gameObject);
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
    }
}
