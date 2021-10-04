using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoShamBot
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController Instance;
        private float speed;
        public bool isMoving = true;

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
        void Start() 
        {
            speed = Player.Instance.InitialSpeed;
            if (Player.Instance.isMoving)
            {
                isMoving = true;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (isMoving && Player.Instance.isMoving) transform.Translate(speed * Time.deltaTime * Vector2.right);
        }
    }
}

