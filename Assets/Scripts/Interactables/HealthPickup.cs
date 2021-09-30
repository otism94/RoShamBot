using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoShamBot
{
    public class HealthPickup : MonoBehaviour
    {
        [SerializeField] private int healAmount = 1;
        [SerializeField] private AudioClip healSFX;
        private Rigidbody2D RB;

        // Start is called before the first frame update
        void Start() => RB = this.gameObject.GetComponent<Rigidbody2D>();

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("PlayerHurtbox"))
            {
                Player.Instance.ChangeHealthBy(healAmount);
                UI.Instance.healthUI.UpdateHealthDisplay();
                Audio.Instance.Source.PlayOneShot(healSFX);
                Destroy(this.gameObject);
            }
        }
    }
}
