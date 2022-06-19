using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Edge
{
    [SerializeReference]
    public NodeTransition condition;
    [SerializeReference]
    public Node fromNode;
    [SerializeReference]
    public Node toNode;
    public Edge(Node inPoint, Node outPoint)
    {
        this.fromNode = inPoint;
        this.toNode = outPoint;
    }
}
