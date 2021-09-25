using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoShamBot
{
    public class PaperBridge : MonoBehaviour
    {
        [SerializeField] private GameObject closedBridge;
        [SerializeField] private GameObject openBridge;
        [SerializeField] private GameObject attachedEnemy;

        public void OpenBridge()
        {
            if (attachedEnemy != null && attachedEnemy.activeInHierarchy) return;
            else
            {
                closedBridge.SetActive(false);
                openBridge.SetActive(true);
                openBridge.GetComponent<BoxCollider2D>().enabled = true;
            }
        }
    }
}