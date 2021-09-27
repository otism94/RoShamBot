using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoShamBot
{
    public class PaperAeroplane : Obstacle
    {
        [SerializeField] private AudioClip planeSFX;
        private Rigidbody2D RB;

        protected override void Start()
        {
            base.Start();
            RB = this.gameObject.GetComponent<Rigidbody2D>();
            this.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(1.2f, .75f, 5));
            RB.AddForce(new Vector2(-50, 3));
            Audio.Instance.Source.PlayOneShot(planeSFX, 0.3f);
        }

        public override void ClearObstacle() => Destroy(this.gameObject);

        public override void Draw() => ClearObstacle();

        public override void Win() => ClearObstacle();

        protected override void OnCollisionEnter2D(Collision2D collision)
        {
            base.OnCollisionEnter2D(collision);
            ClearObstacle();
        }
    }
}
