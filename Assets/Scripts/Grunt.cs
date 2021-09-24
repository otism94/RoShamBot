using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoShamBot
{
    public class Grunt : EnemyObstacle
    {
        [SerializeField] private AudioClip deathScream;

        protected override void Update()
        {
            base.Update();
            if (!Player.Instance.battleMode.active && Vector2.Distance(this.transform.position, Player.Instance.gameObject.transform.position) <= 4f)
            {
                Player.Instance.EnterBattleMode(this);
            }
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("PlayerHitbox") && Player.Instance.Attack != RPS.Shoot.none)
            {
                RPS.Outcome outcome = RPS.GetOutcome(Player.Instance.Attack, enemyAttack);
                
                if (outcome == RPS.Outcome.lose)
                {
                    Debug.Log("Player wins");
                    RPS.WinDefault(Player.Instance.gameObject);
                    RPS.LoseDefault(this.gameObject);
                }
                else if (outcome == RPS.Outcome.draw)
                {
                    Debug.Log("Draw");
                    RPS.DrawDefault(this.gameObject);
                    RPS.DrawDefault(Player.Instance.gameObject);
                    Player.Instance.ResetAttackType();
                }
                else if (outcome == RPS.Outcome.win)
                {
                    Debug.Log("Player loses");
                    RPS.LoseDefault(Player.Instance.gameObject);
                    Player.Instance.ResetAttackType();
                }
                SetAttack();
                DisplayIntent();
            }
        }

        public override void Defeated() 
        {
            defeated = true;
            StartCoroutine(ScreamAndDie());
        }

        private IEnumerator ScreamAndDie()
        {
            RemoveIntentBubble();
            this.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
            Audio.Instance.Source.PlayOneShot(deathScream);
            yield return new WaitForSeconds(deathScream.length);
            Destroy(this.gameObject);
        }

        public override void ClearObstacle() { return; }
    }
}