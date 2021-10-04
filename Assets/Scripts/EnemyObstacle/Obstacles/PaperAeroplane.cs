using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoShamBot
{
    public class PaperAeroplane : Obstacle
    {
        [SerializeField] private AudioClip planeSFX;
        [SerializeField] private Vector3 velocity = new Vector3(-5, 0, 0);
        [SerializeField] private float gravity = 1.2f;

        protected override void Start()
        {
            base.Start();
            transform.position = Camera.main.ViewportToWorldPoint(new Vector3(1.2f, .75f, 5));
            Audio.Instance.Source.PlayOneShot(planeSFX, 0.3f);
        }

        protected override void Update()
        {
            base.Update();

            velocity.y -= gravity * Time.deltaTime;
            transform.position += velocity * Time.deltaTime;
        }

        public override void ClearObstacle() => Destroy(this.gameObject);

        public override void Draw() => ClearObstacle();

        public override void Win() => ClearObstacle();

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            base.OnTriggerEnter2D(collision);

            if (collision.gameObject.CompareTag("PlayerHurtbox"))
            {
                Player.Instance.Lose(-5f);
                ClearObstacle();
            }
            else if (collision.gameObject.CompareTag("Ground")) ClearObstacle();
        }
    }
}
