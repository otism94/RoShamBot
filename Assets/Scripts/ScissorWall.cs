using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoShamBot;

public class ScissorWall : EnemyObstacle
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerHitbox") && Player.Instance.Attack != RPS.Shoot.none)
        {
            switch (RPS.GetOutcome(Player.Instance.Attack, enemyAttack))
            {
                case RPS.Outcome.lose:
                    Debug.Log("Player wins");
                    RPS.WinDefault(Player.Instance.gameObject);
                    ClearObstacle();
                    break;
                case RPS.Outcome.draw:
                    Debug.Log("Draw");
                    RPS.DrawDefault(Player.Instance.gameObject);
                    Player.Instance.ResetAttackType();
                    break;
                case RPS.Outcome.win:
                    Debug.Log("Player loses");
                    RPS.LoseDefault(Player.Instance.gameObject);
                    Player.Instance.ResetAttackType();
                    break;
            }
        }
    }

    public override void ClearObstacle() => Defeated();

    public override void Defeated() => Destroy(this.gameObject);
}
