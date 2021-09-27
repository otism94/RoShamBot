using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoShamBot
{
    public class Target : MonoBehaviour
    {
        [SerializeField] private PaperAeroplane paperAeroplane;
        private BoxCollider2D BC;

        // Start is called before the first frame update
        void Start() => BC = this.gameObject.GetComponent<BoxCollider2D>();

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("PlayerHurtbox")) 
            { 
                Instantiate(paperAeroplane);
                BC.enabled = false;
            }

        }
    }
}
