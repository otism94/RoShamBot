using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoShamBot
{
    public class PaperBridge : EnemyObstacle
    {
        [SerializeField] private GameObject closedBridge;
        [SerializeField] private GameObject openBridge;
        [SerializeField] private GameObject attachedEnemy;

        public override void ClearObstacle()
        {
            if (attachedEnemy != null && attachedEnemy.activeInHierarchy) return;
            else
            {
                closedBridge.SetActive(false);
                openBridge.SetActive(true);
                openBridge.GetComponent<BoxCollider2D>().enabled = true;
            }
        }

        public override void Defeated() { return; }

        protected override void OnTriggerEnter2D(Collider2D collision) { return; }

        protected override void OnCollisionEnter2D(Collision2D collision) { return; }
    }
}