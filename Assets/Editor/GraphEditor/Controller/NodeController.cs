using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NodeController : IGridDrawable
{
    private Graph graph;
    private Node selectedNode;

    public NodeController(Graph graph)
    {
        this.Graph = graph;
    }

    public Graph Graph { get => graph; set => graph = value; }
    public Node SelectedNode { get => selectedNode; set => selectedNode = value; }

    private void DrawNodes()
    {
        foreach (Node node in Graph.Nodes)
        {
            DrawNode(node);
        }
    }

    private void DrawNode(Node node)
    {
        if(node == SelectedNode)
        {
            GUILayout.BeginArea(node.Rect, GraphGUIStyles.SelectedNodeStyle());
        }
        else
        {
            GUILayout.BeginArea(node.Rect, GraphGUIStyles.DefaultNodeStyle());
        }
        node.Data = EditorGUILayout.ObjectField(node.Data, typeof(NodeData), true) as NodeData;
        GUILayout.EndArea();
    }

    public void ProcessNodeEvents(Event e)
    {
        bool clickedNode = false;
        foreach (Node node in Graph.Nodes)
        {
            if (node.Rect.Contains(e.mousePosition))
            {
                clickedNode = true;
                if (e.type == EventType.MouseDown && e.button == 0)
                {
                    ProcessNodeClick(node);
                }
                else if (e.type == EventType.MouseDrag && e.button == 0)
                {
                    ProcessNodeDrag(node, e.delta);
                }
            }
        }
        if (!clickedNode)
        {
            ClearNodeSelection();
        }
    }

    public bool HasSelectedNode(){
        if(selectedNode == null) return false;
        return true;
    }

    private void ClearNodeSelection()
    {
        SelectedNode = null;
    }

    private void ProcessNodeDrag(Node node, Vector2 delta)
    {
        if (node != SelectedNode) return;
        node.Position += delta;
    }

    private void ProcessNodeClick(Node node)
    {
        this.SelectedNode = node;
    }

    public void Draw()
    {
        this.DrawNodes();
    }
}
