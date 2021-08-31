using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int speed = 5;
    public GameObject rockBubble;
    public GameObject paperBubble;
    public GameObject scissorsBubble;
    private Rigidbody2D playerRb;
    public GameObject playerHitbox;
    public bool attacking = false;
    public enum AttackType { none, rock, paper, scissors }
    public AttackType currentAttackType;
    

    // Start is called before the first frame update
    void Start()
    {
        currentAttackType = AttackType.none;
        playerRb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Move the player
        transform.Translate(speed * Time.deltaTime * Vector2.right);

        // Player inputs rock
        if (Input.GetKeyDown(KeyCode.Q) && currentAttackType == AttackType.none)
        {
            GameObject bubble = Instantiate(rockBubble, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + 2), rockBubble.transform.rotation);
            bubble.transform.parent = gameObject.transform;
            GameObject hitbox = Instantiate(playerHitbox, new Vector2(gameObject.transform.position.x + gameObject.GetComponent<BoxCollider2D>().size.x, gameObject.transform.position.y), playerHitbox.transform.rotation);
            hitbox.transform.parent = gameObject.transform;
            currentAttackType = AttackType.rock;
        }
        // Player inputs paper
        else if (Input.GetKeyDown(KeyCode.W) && currentAttackType == AttackType.none)
        {
            GameObject bubble = Instantiate(paperBubble, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + 2), paperBubble.transform.rotation);
            bubble.transform.parent = gameObject.transform;
            GameObject hitbox = Instantiate(playerHitbox, new Vector2(gameObject.transform.position.x + gameObject.GetComponent<BoxCollider2D>().size.x, gameObject.transform.position.y), playerHitbox.transform.rotation);
            hitbox.transform.parent = gameObject.transform;
            currentAttackType = AttackType.paper;
        }
        // Player inputs scissors
        else if (Input.GetKeyDown(KeyCode.E) && currentAttackType == AttackType.none)
        {
            GameObject bubble = Instantiate(scissorsBubble, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + 2), scissorsBubble.transform.rotation);
            bubble.transform.parent = gameObject.transform;
            GameObject hitbox = Instantiate(playerHitbox, new Vector2(gameObject.transform.position.x + gameObject.GetComponent<BoxCollider2D>().size.x, gameObject.transform.position.y), playerHitbox.transform.rotation);
            hitbox.transform.parent = gameObject.transform;
            currentAttackType = AttackType.scissors;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("DeathZone"))
        {
            Debug.Log("Game Over! (Hit the death zone)");
            Destroy(gameObject);
        }
    }


}
