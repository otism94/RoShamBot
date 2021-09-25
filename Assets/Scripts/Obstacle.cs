using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoShamBot
{
    public abstract class Obstacle : EnemyObstacle
    {
        public abstract void ClearObstacle();

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("PlayerHitbox") && Player.Instance.Attack != RPS.Shoot.none)
            {
                switch (RPS.GetOutcome(Player.Instance.Attack, enemyAttack))
                {
                    case RPS.Outcome.lose:
                        Debug.Log("Player wins");
                        Player.Instance.Win();
                        this.Lose();
                        break;
                    case RPS.Outcome.draw:
                        Debug.Log("Draw");
                        Player.Instance.Draw();
                        this.Draw();
                        break;
                    case RPS.Outcome.win:
                        Debug.Log("Player loses");
                        Player.Instance.Lose();
                        this.Win();
                        break;
                }
            }
        }

        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("PlayerHurtbox"))
            {
                Player.Instance.Lose();
                if (fixedShoot != RPS.Shoot.none) SetAttack(fixedShoot);
                else if (includeShoots.Count != 0) SetAttack(includeShoots);
                else SetAttack();
            }
        }

        public override void Lose()
        {
            currentHealth -= 1;
        }

        protected override void Defeat() => ClearObstacle();
    }
}
