using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static bool AreAdjacent(Vector2Int pos1, Vector2Int pos2)
    {
        int dx = Mathf.Abs(pos1.x - pos2.x);
        int dy = Mathf.Abs(pos1.y - pos2.y);

        return (dx == 1 && dy == 0) || (dy == 1 && dx == 0) ;
    }

    public static bool AreNeighboring(Vector2Int pos1, Vector2Int pos2)
    {
        int dx = Mathf.Abs(pos1.x - pos2.x);
        int dy = Mathf.Abs(pos1.y - pos2.y);

        return dx <= 1 && dy <= 1;
    }
    
    public static bool isBackwards(Vector2Int pos1, Vector2Int pos2) {
        return pos2.y < pos1.y;
    }

}
