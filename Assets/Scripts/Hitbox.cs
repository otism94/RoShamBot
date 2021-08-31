using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    private float activeTime = 1.0f;
    private PlayerController playerControllerScript;
    public enum AttackType { none, rock, paper, scissors }
    public AttackType currentAttackType;
    public enum BattleOutcome { win, lose, draw }
    private Rigidbody2D playerRb;
    private Rigidbody2D enemyRb;

    // Start is called before the first frame update
    void Start()
    {
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        playerRb = playerControllerScript.gameObject.GetComponent<Rigidbody2D>();

        currentAttackType = (AttackType)playerControllerScript.currentAttackType;
        Destroy(gameObject, activeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            BasicEnemy basicEnemy = other.gameObject.GetComponent<BasicEnemy>();
            enemyRb = other.gameObject.GetComponent<Rigidbody2D>();
            BattleOutcome outcome = DetermineWinner(playerControllerScript.currentAttackType, basicEnemy.enemyAttack);
            if (outcome == BattleOutcome.win)
            {
                Debug.Log("Win!");
                Destroy(other.gameObject);
            }
            else if (outcome == BattleOutcome.draw)
            {
                Debug.Log("Draw!");
                playerControllerScript.transform.Translate(Vector2.left * 1);
                basicEnemy.transform.Translate(Vector2.right * 1);
                Destroy(gameObject);
            }
            else if (outcome == BattleOutcome.lose)
            {
                Debug.Log("Game Over! (Picked the wrong option)");
                Destroy(playerControllerScript.gameObject);
            }
        }
    }

    private BattleOutcome DetermineWinner(PlayerController.AttackType playerAttack, BasicEnemy.AttackType enemyAttack)
    {
        // Player hits with rock
        if (playerAttack == PlayerController.AttackType.rock)
        {
            if (enemyAttack == BasicEnemy.AttackType.rock)
            {
                return BattleOutcome.draw;
            }
            else if (enemyAttack == BasicEnemy.AttackType.paper)
            {
                return BattleOutcome.lose;
            }
            else if (enemyAttack == BasicEnemy.AttackType.scissors)
            {
                return BattleOutcome.win;
            }
        }
        // Player hits with paper
        else if (playerAttack == PlayerController.AttackType.paper)
        {
            if (enemyAttack == BasicEnemy.AttackType.rock)
            {
                return BattleOutcome.win;
            }
            else if (enemyAttack == BasicEnemy.AttackType.paper)
            {
                return BattleOutcome.draw;
            }
            else if (enemyAttack == BasicEnemy.AttackType.scissors)
            {
                return BattleOutcome.lose;
            }
        }
        // Player hits with scissors
        else if (playerAttack == PlayerController.AttackType.scissors)
        {
            if (enemyAttack == BasicEnemy.AttackType.rock)
            {
                return BattleOutcome.lose;
            }
            else if (enemyAttack == BasicEnemy.AttackType.paper)
            {
                return BattleOutcome.win;
            }
            else if (enemyAttack == BasicEnemy.AttackType.scissors)
            {
                return BattleOutcome.draw;
            }
        }

        return BattleOutcome.lose;
    }
}
