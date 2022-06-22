using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : ScriptableObject
{
    private List<Node> nodes;
    private List<Edge> edges;

    public List<Edge> Edges { get => edges; set => edges = value; }
    public List<Node> Nodes { get => nodes; set => nodes = value; }

    public void Connect(Node fromNode, Node toNode)
    {

    }

    public void Disconnect(Node fromNode, Node toNode)
    {

    }

    public void Add(Node node)
    {
        this.nodes.Add(node);
    }
}
