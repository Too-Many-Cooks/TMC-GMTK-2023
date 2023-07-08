using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vector2Ex
{
    public static class Vector2Ex
    {
        public static Vector2 Project(this Vector2 vector, Vector2 normal)
        {
            return new Vector2(((vector.x * normal.x + vector.y * normal.y) / (normal.x * normal.x + normal.y * normal.y)) * normal.x, ((vector.x * normal.x + vector.y * normal.y) / (normal.x * normal.x + normal.y * normal.y)) * normal.y); //mathmathmathmathmath
        }
    }
}
