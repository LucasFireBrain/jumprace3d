using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static Vector3 withX(this Vector3 vector, float x) {
        return new Vector3(x, vector.y, vector.z);
    }
    public static Vector3 withY(this Vector3 vector, float y)
    {
        return new Vector3(vector.x, y, vector.z);
    }
}
