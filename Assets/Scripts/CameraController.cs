using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoShamBot
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController Instance;
        private int speed;
        public bool moving = true;

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
        void Start() => speed = Player.Instance.Speed;

        // Update is called once per frame
        void Update()
        {
            if (moving) transform.Translate(speed * Time.deltaTime * Vector2.right);
        }
    }
}

