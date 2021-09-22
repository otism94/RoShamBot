using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoShamBot
{
    public class DeathZone : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("PlayerHurtbox"))
            {
                Debug.Log("Game Over! (Hit the death zone)");
                HealthDisplay.Instance.RemoveAllHearts();
                Player.Instance.SetHealthTo(0);
            }
            else if (collision.gameObject.CompareTag("Enemy")) collision.gameObject.GetComponent<EnemyObstacle>().Defeated();
        }
    }
}

