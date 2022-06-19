using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node
{
    public Rect rect;
    public ConnectionPoint inPoint;
    public ConnectionPoint outPoint;

    public Node(Vector2 position, float width, float height)
    {
        rect = new Rect(position.x, position.y, width, height);
        inPoint = new ConnectionPoint(ConnectionPointType.In);
        outPoint = new ConnectionPoint(ConnectionPointType.Out);
    }
}
