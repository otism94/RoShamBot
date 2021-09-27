using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoShamBot
{
    public class PaperAeroplane : Obstacle
    {
        public override void ClearObstacle() => Destroy(this.gameObject);

        public override void Draw() => ClearObstacle();

        public override void Win() => ClearObstacle();
    }
}
