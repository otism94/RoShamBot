using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoShamBot
{
    public class DeathZone : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("PlayerHurtbox"))
            {
                Debug.Log("Game Over! (Hit the death zone)");
                UI.Instance.healthUI.RemoveAllHearts();
                Player.Instance.SetHealthTo(0);
            }
            else if (collision.gameObject.CompareTag("Enemy") && !collision.gameObject.TryGetComponent<PaperAeroplane>(out PaperAeroplane plane) && Player.Instance.battleMode.active) collision.gameObject.GetComponent<EnemyObstacle>().SetHealthTo(0);
        }
    }
}

