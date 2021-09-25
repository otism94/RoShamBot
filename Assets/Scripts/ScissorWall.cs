using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoShamBot;

public class ScissorWall : Obstacle
{
    public override void ClearObstacle() => Destroy(this.gameObject);
    public override void Win() { return; }
    public override void Draw() { return; }
}
