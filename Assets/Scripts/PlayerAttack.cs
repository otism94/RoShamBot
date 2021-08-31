using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private float activeTime = 1.0f;
    private PlayerController playerControllerScript;

    // Start is called before the first frame update
    void Start()
    {
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        Destroy(gameObject, activeTime);
    }

    private void OnDestroy()
    {
        playerControllerScript.currentAttackType = PlayerController.AttackType.none;
    }
}
