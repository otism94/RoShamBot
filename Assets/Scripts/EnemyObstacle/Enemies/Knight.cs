using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoShamBot
{
    public class Knight : Enemy
    {
        [SerializeField] private float shieldedDrawKnockback = 1;
        [SerializeField] private float shieldedLoseKnockback = 2;
        [SerializeField] private GameObject shield;
        [SerializeField] private AudioClip deathScream;
        private bool shielded = true;
        [SerializeField] private AudioClip shieldCutClip;

        protected override void Defeat() => StartCoroutine(ScreamAndDie());

        private IEnumerator ScreamAndDie()
        {
            RemoveIntentBubble();
            sprite.color = Color.red;
            Audio.Instance.Source.PlayOneShot(deathScream);
            yield return new WaitForSeconds(deathScream.length);
            Destroy(this.gameObject);
        }

        private void BreakShield()
        {
            Audio.Instance.Source.PlayOneShot(shieldCutClip);
            shield.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0, 127.5f);
            Destroy(shield, .4f);
            shielded = false;
            includeShoots.Clear();
            intentChildOffset = 0;
        }

        public override void Draw()
        {
            if (!shielded)
            {
                base.Draw();
                return;
            }

            RB.AddForce(new Vector2(shieldedDrawKnockback, 0), ForceMode2D.Impulse);
            RemoveIntentBubble();
            SetAttack();

            if (Player.Instance.Attack == RPS.Shoot.scissors) BreakShield();
        }

        public override void Lose()
        {
            if (!shielded)
            {
                base.Lose();
                return;
            }

            RB.AddForce(new Vector2(shieldedLoseKnockback, 0), ForceMode2D.Impulse);
            RemoveIntentBubble();
            SetAttack();

            if (Player.Instance.Attack == RPS.Shoot.scissors) BreakShield();
        }
    }
}
