using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node
{
    [SerializeField]
    private Rect rect;
    [SerializeReference]
    protected NodeData data;
    [SerializeField]
    private string guid;

    public Node(Vector2 position)
    {
        this.guid = Guid.NewGuid().ToString();
        this.Rect = new Rect(position.x,position.y,100,100);
    }

    public NodeData Data { get => data; set => data = value; }
    public Rect Rect { get => rect; set => rect = value; }

    public Vector2 Position { get => rect.position; set => rect.position = value; }
}
