using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Graph : ScriptableObject
{
    [SerializeReference]
    private List<Node> nodes;
    [SerializeReference]
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

    public void Disconnect(Edge edge)
    {
        edges.Remove(edge);
    }

    public void Remove(Node node)
    {
        foreach(Edge edge in edges)
        {
            if(edge.FromNode == node || edge.ToNode == node)
            {
                Disconnect(edge);
            }
        }
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
