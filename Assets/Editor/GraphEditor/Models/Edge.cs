using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Edge
{
    [SerializeReference]
    public NodeTransition condition;
    [SerializeReference]
    private Node fromNode;
    [SerializeReference]
    private Node toNode;

    public Node FromNode { get => fromNode; set => fromNode = value; }
    public Node ToNode { get => toNode; set => toNode = value; }

    public Edge(Node inPoint, Node outPoint)
    {
        this.FromNode = inPoint;
        this.ToNode = outPoint;
    }
}
