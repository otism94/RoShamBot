using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoShamBot
{
    public class Lever : EnemyObstacle
    {
        public bool flipped = false;
        [SerializeField] private GameObject unflippedLever;
        [SerializeField] private GameObject flippedLever;
        [SerializeField] private EnemyObstacle obstacle;

        protected override void Update()
        {
            if (flipped && unflippedLever.activeInHierarchy) ClearObstacle();
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (flipped) return;
            if (collision.gameObject.CompareTag("PlayerHitbox") && Player.Instance.Attack != RPS.Shoot.none)
            {
                if (RPS.GetOutcome(Player.Instance.Attack, enemyAttack) == RPS.Outcome.lose) ClearObstacle();
            }
        }
        public override void Defeated() { return; }

        public override void ClearObstacle()
        {
            flipped = true;
            unflippedLever.SetActive(false);
            flippedLever.SetActive(true);
            obstacle.ClearObstacle();
        }

        protected override void OnCollisionEnter2D(Collision2D collision) { return; }
    }
}