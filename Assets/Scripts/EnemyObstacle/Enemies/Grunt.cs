using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoShamBot
{
    public class Grunt : Enemy
    {
        [SerializeField] private AudioClip deathScream;

        protected override void Defeat() => StartCoroutine(ScreamAndDie());

        private IEnumerator ScreamAndDie()
        {
            RemoveIntentBubble();
            sprite.color = Color.red;
            Audio.Instance.Source.PlayOneShot(deathScream);
            yield return new WaitForSeconds(deathScream.length);
            Destroy(this.gameObject);
        }
    }
}