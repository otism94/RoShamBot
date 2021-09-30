using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoShamBot
{
    public class Lever : Obstacle
    {
        public bool flipped = false;
        [SerializeField] private GameObject unflippedLever;
        [SerializeField] private GameObject flippedLever;
        [SerializeField] private PaperBridge bridge;
        [SerializeField] private AudioClip flipSFX;

        protected override void Update()
        {
            base.Update();
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

        public override void ClearObstacle()
        {
            Audio.Instance.Source.PlayOneShot(flipSFX);
            flipped = true;
            unflippedLever.SetActive(false);
            flippedLever.SetActive(true);
            bridge.OpenBridge();
        }

        protected override void OnCollisionEnter2D(Collision2D collision) { return; }
        public override void Win() { return; }
        public override void Draw() { return; }
    }
}