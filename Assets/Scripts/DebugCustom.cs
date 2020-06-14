using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugEx
{
    public static void drawRect(float up,float bottom,float left,float right,float duration = 0.1f)
    {
        var leftUp = new Vector3(left, up, 0.0f);
        var leftBottom = new Vector3(left, bottom, 0.0f);
        var rightBottom = new Vector3(right, bottom, 0.0f);
        var rightUp = new Vector3(right, up, 0.0f);

        Debug.DrawLine(leftUp, leftBottom,Color.red,duration);
        Debug.DrawLine(leftBottom, rightBottom,Color.red,duration);
        Debug.DrawLine(rightBottom, rightUp,Color.red, duration);
        Debug.DrawLine(rightUp, leftUp,Color.red, duration);
    }
}
