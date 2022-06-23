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
        foreach (var edge in edges)
        {
            if(edge.FromNode == fromNode && edge.ToNode == toNode)
            {
                return;
            }
        }
        edges.Add(new Edge(fromNode, toNode));
    }

    public void Disconnect(Node fromNode, Node toNode)
    {
        Edge edgeToRemove = null;
        foreach (var edge in edges)
        {
            if(edge.FromNode == fromNode && edge.ToNode == toNode)
            {
                edgeToRemove = edge;
            }
        }
        edges.Remove(edgeToRemove);
    }

    public Node GetNodeAtPosition(Vector2 position)
    {
        foreach(Node node in nodes)
        {
            if (node.Rect.Contains(position))
            {
                return node;
            }
        }
        throw new NoNodeFoundException();
    }
}
