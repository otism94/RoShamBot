using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoShamBot
{
    public abstract class EnemyObstacle : MonoBehaviour
    {
        #region Data

        [Header("Health")]
        [SerializeField, Tooltip("The enemy/obstacle's starting health. Default: 1")]
        protected int initialHealth = 1;
        protected int currentHealth;
        protected bool defeated;

        [Header("Attacks")]
        [SerializeField, Tooltip("An RPS option the enemy/obstacle is guaranteed to choose.")]
        protected RPS.Shoot fixedShoot;
        [SerializeField, Tooltip("Limit possible attacks to one of these options.")]
        protected List<RPS.Shoot> includeShoots;
        protected RPS.Shoot enemyAttack;

        protected SpriteRenderer sprite;

        #endregion

        #region Setup & Update

        protected virtual void Start()
        {
            sprite = this.gameObject.GetComponent<SpriteRenderer>();
            currentHealth = initialHealth;
            if (fixedShoot != RPS.Shoot.none) SetAttack(fixedShoot);
            else if (includeShoots.Count > 0) SetAttack(includeShoots);
            else SetAttack();
        }

        protected virtual void Update() 
        {
            if (currentHealth == 0 && !defeated)
            {
                defeated = true;
                Defeat();
            }
        }

        #endregion

        #region Getters & Setters

        public bool Defeated => defeated;

        public int Health => currentHealth;

        /// <summary>
        /// Sets the EnemyObstacle's health to the passed in value.
        /// </summary>
        /// <param name="value">The value to set the EnemyObstacle's health to.</param>
        public void SetHealthTo(int value) => currentHealth = value;

        public RPS.Shoot Attack => enemyAttack;

        #endregion

        #region Methods

        /// <summary>
        /// Sets the enemy's attack to a random RPS.Shoot.
        /// </summary>
        public void SetAttack()
        {
            if (fixedShoot != RPS.Shoot.none) 
            {
                SetAttack(fixedShoot);
                return;
            }
            if (includeShoots.Count > 0)
            {
                SetAttack(includeShoots);
                return;
            }
            else enemyAttack = (RPS.Shoot)Random.Range(1, 4);
        }

        /// <summary>
        /// Sets the enemy's attack to the provided RPS.Shoot.
        /// </summary>
        /// <param name="fixedShoot">RPS.Shoot to set.</param>
        protected void SetAttack(RPS.Shoot fixedShoot) => enemyAttack = fixedShoot;

        /// <summary>
        /// Sets the enemy's attack to one RPS.Shoot in the provided collection.
        /// </summary>
        /// <param name="includeShoots">Collection of RPS.Shoot options the enemy can pick from.</param>
        protected void SetAttack(List<RPS.Shoot> includeShoots)
        {
            do enemyAttack = (RPS.Shoot)Random.Range(1, 4);
            while (!includeShoots.Contains(enemyAttack));
        }

        protected void OnDestroy() => this.gameObject.GetComponent<OnDestroyEvent>()?.Event();

        #endregion

        #region Abstract Methods

        public abstract void Win();
        public abstract void Draw();
        public abstract void Lose();
        protected abstract void Defeat();

        #endregion
    }
}