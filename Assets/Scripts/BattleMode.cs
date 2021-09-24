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
        public EnemyObstacle enemy;
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
            if (enemy != null && !enemy.defeated && !roundStarted && enemy.isStationary) StartCoroutine(RoundStart());

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


            if (Player.Instance.Health > 0 && enemy.currentHealth > 0) roundStarted = false;
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
            SpriteRenderer resultSprite = resultDisplay.GetComponent<SpriteRenderer>();

            if (Player.Instance.currentAttackType != RPS.Shoot.none)
            {
                RPS.Outcome outcome = RPS.GetOutcome(Player.Instance.Attack, enemy.enemyAttack);
                
                if (outcome == RPS.Outcome.lose)
                {
                    Debug.Log("Player wins");
                    resultSprite.sprite = winSprite;
                    GameObject resultObj = Instantiate(resultDisplay);
                    resultDisplay.transform.position = Player.Instance.gameObject.transform.position;
                    Destroy(resultObj, 1f);
                    RPS.WinDefault(Player.Instance.playerSprite);
                    RPS.LoseDefault(enemy.gameObject);
                    enemy.RemoveIntentBubble();
                    enemy.SetAttack();
                    Player.Instance.ResetAttackType();
                    return;
                }
                else if (outcome == RPS.Outcome.draw)
                {
                    Debug.Log("Draw");
                    resultSprite.sprite = drawSprite;
                    GameObject resultObj = Instantiate(resultDisplay);
                    resultDisplay.transform.position = Player.Instance.gameObject.transform.position;
                    Destroy(resultObj, 1f);
                    RPS.DrawDefault(enemy.gameObject);
                    RPS.DrawDefault(Player.Instance.playerSprite);
                    enemy.RemoveIntentBubble();
                    enemy.SetAttack();
                    Player.Instance.ResetAttackType();
                    return;
                }
            }
            Debug.Log("Player loses");
            resultSprite.sprite = loseSprite;
            GameObject resultObject = Instantiate(resultDisplay);
            resultDisplay.transform.position = Player.Instance.gameObject.transform.position;
            Destroy(resultObject, 1f);
            RPS.LoseDefault(Player.Instance.playerSprite);
            enemy.RemoveIntentBubble();
            enemy.SetAttack();
            Player.Instance.ResetAttackType();
        }
    }
}