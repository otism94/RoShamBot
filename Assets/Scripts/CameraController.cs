using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private int speed;

    // Start is called before the first frame update
    void Start()
    {
        speed = GameObject.Find("Player").GetComponent<PlayerController>().speed;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector2.right);
    }
}
