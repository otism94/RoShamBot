using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace RoShamBot
{
    public class BattleMode : MonoBehaviour
    {
        public static BattleMode Instance;

        public bool active;
        public Enemy enemy;
        [SerializeField] private TextMeshProUGUI countdown;
        [SerializeField] private AudioClip countdownClip;
        [SerializeField] private AudioClip lightActivateClip;
        [SerializeField] private GameObject resultDisplay;
        [SerializeField] private Sprite winSprite;
        [SerializeField] private Sprite drawSprite;
        [SerializeField] private Sprite loseSprite;
        [SerializeField] private GameObject playerSpotlight;
        [SerializeField] private GameObject enemySpotlight;
        private bool roundStarted = false;
        private bool playerCanInput = false;

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

        private void Start() 
        { 
            RenderSettings.ambientLight = Color.grey;
            Audio.Instance.Source.PlayOneShot(lightActivateClip, .6f);
            playerSpotlight.transform.position = new Vector3(Player.Instance.transform.position.x, playerSpotlight.transform.position.y, -1.5f);
            enemySpotlight.transform.position = new Vector3(enemy.transform.position.x, enemySpotlight.transform.position.y, -1.5f);
        }

        // Update is called once per frame
        void Update()
        {
            if (enemy == null) EndBattleMode();
            else
            {
                if (enemy != null && !enemy.Defeated && !roundStarted && enemy.Stationary) StartCoroutine(RoundStart());

                if (playerCanInput) Player.Instance.HandleInput();

                playerSpotlight.transform.position = new Vector3(Player.Instance.transform.position.x, playerSpotlight.transform.position.y, -1.5f);
                enemySpotlight.transform.position = new Vector3(enemy.transform.position.x, enemySpotlight.transform.position.y, -1.5f);
            }
        }

        public IEnumerator RoundStart(float precognition = 0.2f, float postcognition = 0.3f)
        {
            roundStarted = true;
            playerCanInput = false;
            Audio.Instance.Source.PlayOneShot(countdownClip);
            countdown.text = "3";
            yield return new WaitForSeconds(0.5f);
            countdown.text = "2";
            yield return new WaitForSeconds(0.5f);
            countdown.text = "1";
            yield return new WaitForSeconds(precognition);
            enemy.DisplayIntent();
            yield return new WaitForSeconds(postcognition);
            countdown.text = "GO!";
            playerCanInput = true;
            yield return new WaitForSeconds(0.5f);
            countdown.text = "";
            playerCanInput = false;
            DetermineWinner();
            yield return new WaitForSeconds(1);

            if (Player.Instance.Health > 0 && enemy.Health > 0) roundStarted = false;
            else EndBattleMode();
        }

        private void EndBattleMode()
        {
            StopAllCoroutines();
            enemySpotlight.transform.position = new Vector3(Player.Instance.transform.position.x + 4f, enemySpotlight.transform.position.y, -1.5f);
            active = false;
            playerCanInput = true;
            roundStarted = false;
            Player.Instance.ExitBattleMode(this.gameObject);
        }

        private void DetermineWinner()
        {
            if (Player.Instance.currentAttackType != RPS.Shoot.none)
            {
                RPS.Outcome outcome = RPS.GetOutcome(Player.Instance.Attack, enemy.Attack);
                
                if (outcome == RPS.Outcome.lose)
                {
                    Debug.Log("Player wins");
                    DisplayResult(winSprite);

                    Player.Instance.Win();
                    enemy.Lose();
                    
                    return;
                }
                else if (outcome == RPS.Outcome.draw)
                {
                    Debug.Log("Draw");
                    DisplayResult(drawSprite);

                    Player.Instance.Draw();
                    enemy.Draw();
                    
                    return;
                }
            }

            Debug.Log("Player loses");
            DisplayResult(loseSprite);

            Player.Instance.Lose();
            enemy.Win();
        }

        private void DisplayResult (Sprite result)
        {
            SpriteRenderer resultSprite = resultDisplay.GetComponent<SpriteRenderer>();
            resultSprite.sprite = result;
            GameObject resultInstance = Instantiate(resultDisplay, transform);
            resultInstance.transform.position = new Vector3(transform.position.x, transform.position.y, 5);
            Destroy(resultInstance, 1f);
        }

        private void OnDestroy() => RenderSettings.ambientLight = Color.white;
    }
}