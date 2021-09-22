using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoShamBot
{
    public class HealthDisplay : MonoBehaviour
    {
        public static HealthDisplay Instance;
        [SerializeField] private GameObject playerHeart;
        [SerializeField] private Vector2 position;
        private List<GameObject> hearts;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this.gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        // Start is called before the first frame update
        void Start()
        {
            hearts = new List<GameObject>();
            for (int i = 0; i < Player.Instance.InitialHealth; i++) AddHeart();
        }

        /// <summary>
        /// Adds a heart to the player health display.
        /// </summary>
        private void AddHeart()
        {
            GameObject heart = Instantiate(playerHeart, transform);
            hearts.Add(heart);
            heart.transform.Translate(new Vector2(position.x + (hearts.Count * .75f), position.y));
        }

        /// <summary>
        /// Updates the player health display based on the player's current health.
        /// </summary>
        public void UpdateHealthDisplay()
        {
            // Player lost health
            if (hearts.Count > Player.Instance.Health)
            {
                Destroy(hearts[hearts.Count - 1]);
                hearts.RemoveAt(hearts.Count - 1);
            }
            // Player gained health
            else if (hearts.Count < Player.Instance.Health) AddHeart();
        }

        /// <summary>
        /// Removes all hearts from the player display (used for instant kills).
        /// </summary>
        public void RemoveAllHearts()
        {
            foreach (GameObject heart in hearts)
                Destroy(heart);
        }
    }
}

