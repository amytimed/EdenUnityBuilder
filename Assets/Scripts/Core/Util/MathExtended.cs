using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathExtended
{
    public static Vector2 ClampSquare(Vector2 vector, float maxLength)
    {
        vector.x = Mathf.Clamp(vector.x, -maxLength, maxLength);
        vector.y = Mathf.Clamp(vector.y, -maxLength, maxLength);
        return vector;
    }
}
