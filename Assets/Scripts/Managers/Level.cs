using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RoShamBot
{
    public class Level : MonoBehaviour
    {
        public static Level Instance;
        [SerializeField] private GameObject gameOver;

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

        // Start is called before the first frame update
        void Start() => StartCoroutine(StartRunning());
        
        private IEnumerator StartRunning()
        {
            yield return new WaitForSeconds(1.5f);
            Player.Instance.isMoving = true;
        }

        public void GameOver() => gameOver.SetActive(true);

        public void BackToTitle() 
        {
            gameOver.SetActive(false);
            SceneManager.LoadScene("Title");
        }

        public void RestartLevel() 
        {
            gameOver.SetActive(false);
            SceneManager.LoadScene("Level");
        }

    // Update is called once per frame
    void Update()
        {
        
        }
    }
}
