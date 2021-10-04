using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoShamBot
{
    public class FinishLine : MonoBehaviour
    {
        [SerializeField] private AudioClip winSFX;
        [SerializeField] private GameObject winScreen;
        [SerializeField] private GameObject rightDeathZone;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("PlayerHurtbox"))
            {
                CameraController.Instance.isMoving = false;
                winScreen.SetActive(true);
                Audio.Instance.Source.PlayOneShot(winSFX, 0.5f);
                Destroy(rightDeathZone);

            }
        }
    }
}
