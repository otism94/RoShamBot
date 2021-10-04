using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoShamBot
{
    public class Audio : MonoBehaviour
    {
        public static Audio Instance;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this.gameObject);
                return;
            }

            Instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }

        public AudioSource Source => this.GetComponent<AudioSource>();
    }
}