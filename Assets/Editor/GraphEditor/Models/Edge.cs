using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[System.Serializable]
public class Edge //TODO maybe make a renderable object to make drawing efficient and allow the camera to chose if an object should be rendered?
{
    [SerializeField]
    public string transitionID;

    [NonSerialized]
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
