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
        [SerializeField] private GameObject resultDisplay;
        [SerializeField] private Sprite winSprite;
        [SerializeField] private Sprite drawSprite;
        [SerializeField] private Sprite loseSprite;
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
            DontDestroyOnLoad(this.gameObject);
        }

        // Update is called once per frame
        void Update()
        {
            if (enemy != null && !enemy.Defeated && !roundStarted && enemy.Stationary) StartCoroutine(RoundStart());

            if (playerCanInput) Player.Instance.HandleInput();

            if (enemy == null) EndBattleMode();
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
                    Player.Instance.ResetAttackType();
                    enemy.Lose();
                    enemy.RemoveIntentBubble();
                    enemy.SetAttack();
                    

                    return;
                }
                else if (outcome == RPS.Outcome.draw)
                {
                    Debug.Log("Draw");
                    DisplayResult(drawSprite);

                    Player.Instance.Draw();
                    Player.Instance.ResetAttackType();
                    enemy.Draw();
                    enemy.RemoveIntentBubble();
                    enemy.SetAttack();
                    
                    return;
                }
            }

            Debug.Log("Player loses");
            DisplayResult(loseSprite);

            Player.Instance.Lose();
            Player.Instance.ResetAttackType();
            enemy.Win();
            enemy.RemoveIntentBubble();
            enemy.SetAttack();
        }

        private void DisplayResult (Sprite result)
        {
            SpriteRenderer resultSprite = resultDisplay.GetComponent<SpriteRenderer>();
            resultSprite.sprite = result;
            GameObject resultInstance = Instantiate(resultDisplay, transform);
            resultInstance.transform.position = new Vector3(transform.position.x, transform.position.y, 5);
            Destroy(resultInstance, 1f);
        }
    }
}