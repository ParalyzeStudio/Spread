using UnityEngine;
using System.Collections;

public class SceneTouchHandler : TouchHandler
{
    protected override bool OnPointerMove(Vector2 pointerLocation, ref Vector2 delta)
    {
        //TODO make the scene slide
        return true;
    }
}
