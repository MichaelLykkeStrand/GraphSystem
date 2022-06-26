using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

public class GraphEditor : EditorWindow
{
    private Graph graph;

    private Node selectedInPoint;
    private Node selectedOutPoint;

    private Vector2 offset;
    private Vector2 drag;

    private int toolbarInt = -1;
    public string[] toolbarStrings = new string[] { "Open", "Save"};

    [MenuItem("Window/Node Based Editor")]
    private static void OpenWindow()
    {
        GraphEditor window = GetWindow<GraphEditor>();
        window.titleContent = new GUIContent("Node Based Editor");
    }

    private void OnEnable()
    {
        graph = ScriptableObject.CreateInstance<Graph>();
        graph.Edges = new List<Edge>();
        graph.Nodes = new List<Node>();
    }

    private void OnGUI()
    {
        DrawTools();

        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);

        DrawNodes();
        DrawConnections();

        DrawConnectionLine(Event.current);

        ProcessNodeEvents(Event.current);
        ProcessEvents(Event.current);
        ProcessToolStrip();

        if (GUI.changed) Repaint();
    }

    // TODO refactor to use enum?
    private void ProcessToolStrip()
    {
        switch (toolbarInt)
        {
            case 0:
                OpenGraph();
                break;

            case 1:
                SaveGraph();
                break;
        }

        toolbarInt = -1;
    }
    private void OpenGraph()
    {
        AssetDatabase.Refresh();
        string absPath = EditorUtility.OpenFilePanel("Select Graph","", "asset");
        string relativePath = absPath.Substring(absPath.IndexOf("Assets/"));
        this.graph = AssetDatabase.LoadAssetAtPath<Graph>(relativePath);

    }
    private void SaveGraph()
    {
        string absPath = EditorUtility.SaveFilePanel("Save Graph", "", "graph","asset");
        string relativePath = absPath.Substring(absPath.IndexOf("Assets/"));
        AssetDatabase.CreateAsset(graph, relativePath);
    }

    private void DrawTools()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        DrawToolStrip();
        GUILayout.EndHorizontal();
    }

    private void DrawToolStrip()
    {
        toolbarInt = GUI.Toolbar(new Rect(25, 25, 250, 30), toolbarInt, toolbarStrings);
    }

    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        offset += drag * 0.5f;
        Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

        for (int i = 0; i < widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
        }

        for (int j = 0; j < heightDivs; j++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }

    private void DrawNodes()
    {
        foreach (Node node in graph.Nodes)
        {
            DrawNode(node);
        }
    }

    private void DrawNode(Node node)
    {
        GUILayout.BeginArea(node.Rect, GraphGUIStyles.DefaultNodeStyle());
        node.Data = EditorGUILayout.ObjectField(node.Data, typeof(NodeData), true) as NodeData;
        GUILayout.EndArea();
    }

    private void DrawConnections()
    {
        if (graph.Edges != null)
        {
            foreach (Edge edge in graph.Edges)
            {
                DrawEdge(edge);
            }
        }
    }

    private void DrawEdge(Edge edge)
    {
        int width = 150;
        int height = 20;
        Vector2 center = (edge.FromNode.Rect.center + edge.ToNode.Rect.center) * 0.5f;
        Rect rect = new Rect(center.x - width / 2, center.y, width, height);
        Handles.DrawBezier(
            edge.FromNode.Rect.center,
            edge.ToNode.Rect.center,
            edge.FromNode.Rect.center + Vector2.left * 50f,
            edge.ToNode.Rect.center - Vector2.left * 50f,
            Color.white,
            null,
            2f
        );

        if (Handles.Button(center, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
        {
            graph.Disconnect(edge);
        }

        GUILayout.BeginArea(rect, GraphGUIStyles.EdgeStyle());
        edge.condition = EditorGUILayout.ObjectField(edge.condition, typeof(NodeTransition), true) as NodeTransition;
        GUILayout.EndArea();
    }

    private void ProcessEvents(Event e)
    {
        drag = Vector2.zero;

        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    ClearConnectionSelection();
                    selectedInPoint = graph.GetNodeAtPosition(e.mousePosition);
                }

                if (e.button == 1)
                {
                    ProcessContextMenu(e.mousePosition);
                }
                break;

            case EventType.MouseDrag:
                if (e.button == 0)
                {
                    OnDrag(e.delta);
                }
                break;
        }
    }

    private void ProcessNodeEvents(Event e)
    {
        if (graph.Nodes != null)
        {
            for (int i = graph.Nodes.Count - 1; i >= 0; i--)
            {
                //bool guiChanged = graph.Nodes[i].ProcessEvents(e);
            }
        }
    }

    private void DrawConnectionLine(Event e)
    {
        if (selectedInPoint != null && selectedOutPoint == null)
        {
            Handles.DrawBezier(
                selectedInPoint.Rect.center,
                e.mousePosition,
                selectedInPoint.Rect.center + Vector2.left * 50f,
                e.mousePosition - Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }

        if (selectedOutPoint != null && selectedInPoint == null)
        {
            Handles.DrawBezier(
                selectedOutPoint.Rect.center,
                e.mousePosition,
                selectedOutPoint.Rect.center - Vector2.left * 50f,
                e.mousePosition + Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }
    }

    private void ProcessContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Add node"), false, () => OnClickAddNode(mousePosition));
        genericMenu.ShowAsContext();
    }

    private void OnDrag(Vector2 delta)
    {
        drag = delta;

        foreach(Node node in graph.Nodes)
        {
            node.Position += delta;
        }

        GUI.changed = true;
    }

    private void OnClickAddNode(Vector2 mousePosition)
    {
        graph.Nodes.Add(new Node(mousePosition));
    }


    private void OnClickRemoveNode(Node node)
    {
        graph.Remove(node);
    }

    private void CreateConnection()
    {
        graph.Edges.Add(new Edge(selectedInPoint, selectedOutPoint));
    }

    private void ClearConnectionSelection()
    {
        selectedInPoint = null;
        selectedOutPoint = null;
    }
}