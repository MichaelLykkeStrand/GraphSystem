using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node
{
    [SerializeField]
    Vector2 position;
    NodeData data;

    public Node(Vector2 position)
    {
        this.position = position;
    }

    public NodeData Data { get => data; set => data = value; }
}
