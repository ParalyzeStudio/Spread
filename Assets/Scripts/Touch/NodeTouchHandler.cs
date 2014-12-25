using UnityEngine;
using System.Collections;

public class NodeTouchHandler : TouchHandler
{
    protected override bool OnPointerMove(Vector2 pointerLocation, ref Vector2 delta)
    {
        if (base.OnPointerMove(pointerLocation, ref delta))
        {
            Vector3 deltaVec3 = delta;
            this.transform.position += deltaVec3;
        }

        return true;
    }
}
