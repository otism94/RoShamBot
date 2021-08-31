using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    public GameObject rockBubble;
    public GameObject paperBubble;
    public GameObject scissorsBubble;
    public Rigidbody2D enemyRb;
    private GameObject bubble;
    public enum AttackType { none, rock, paper, scissors }
    public AttackType enemyAttack;

    // Start is called before the first frame update
    void Start()
    {
        enemyRb = gameObject.GetComponent<Rigidbody2D>();
        int randomAttack = Random.Range(1, 4);

        // Enemy picks rock
        if (randomAttack == 1)
        {
            enemyAttack = AttackType.rock;
            bubble = Instantiate(rockBubble, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 2), rockBubble.transform.rotation);
        }
        // Enemy picks paper
        else if (randomAttack == 2)
        {
            enemyAttack = AttackType.paper;
            bubble = Instantiate(paperBubble, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 2), paperBubble.transform.rotation);
        }
        // Enemy picks scissors
        else if (randomAttack == 3)
        {
            enemyAttack = AttackType.scissors;
            bubble = Instantiate(scissorsBubble, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 2), scissorsBubble.transform.rotation);
        }
    }

    // Update is called once per frame
    void Update()
    {
        bubble.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 2);
    }

    private void OnDestroy()
    {
        Destroy(bubble);
    }
}
