using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoShamBot
{
    public class CreateBridgeOnDefeat : OnDestroyEvent
    {
        [SerializeField] private Lever lever;
        [SerializeField] private PaperBridge bridge;

        public override void Event()
        {
            if (lever.flipped) bridge.ClearObstacle();
        }
    }
}