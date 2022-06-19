using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

public class GraphEditor : EditorWindow
{
    private Graph graph;

    private ConnectionPoint selectedInPoint;
    private ConnectionPoint selectedOutPoint;

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
        graph.connections = new List<Connection>();
        graph.nodes = new List<Node>();
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
        if (graph.nodes != null)
        {
            for (int i = 0; i < graph.nodes.Count; i++)
            {
                graph.nodes[i].Draw();
            }
        }
    }

    private void DrawConnections()
    {
        if (graph.edges != null)
        {
            for (int i = 0; i < graph.edges.Count; i++)
            {
                graph.edges[i].Draw();
            }
        }
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
        if (graph.nodes != null)
        {
            for (int i = graph.nodes.Count - 1; i >= 0; i--)
            {
                bool guiChanged = graph.nodes[i].ProcessEvents(e);

                if (guiChanged)
                {
                    GUI.changed = true;
                }
            }
        }
    }

    private void DrawConnectionLine(Event e)
    {
        if (selectedInPoint != null && selectedOutPoint == null)
        {
            Handles.DrawBezier(
                selectedInPoint.rect.center,
                e.mousePosition,
                selectedInPoint.rect.center + Vector2.left * 50f,
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
                selectedOutPoint.rect.center,
                e.mousePosition,
                selectedOutPoint.rect.center - Vector2.left * 50f,
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

        if (graph.nodes != null)
        {
            for (int i = 0; i < graph.nodes.Count; i++)
            {
                graph.nodes[i].Drag(delta);
            }
        }

        GUI.changed = true;
    }

    private void OnClickAddNode(Vector2 mousePosition)
    {
        graph.nodes.Add(new NodeController(mousePosition, 180, 100, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode));
    }

    private void OnClickInPoint(ConnectionPoint inPoint)
    {
        selectedInPoint = inPoint;

        if (selectedOutPoint != null)
        {
            if (selectedOutPoint.node != selectedInPoint.node)
            {
                CreateConnection();
                ClearConnectionSelection();
            }
            else
            {
                ClearConnectionSelection();
            }
        }
    }

    private void OnClickOutPoint(ConnectionPoint outPoint)
    {
        selectedOutPoint = outPoint;

        if (selectedInPoint != null)
        {
            if (selectedOutPoint.node != selectedInPoint.node)
            {
                CreateConnection();
                ClearConnectionSelection();
            }
            else
            {
                ClearConnectionSelection();
            }
        }
    }

    private void OnClickRemoveNode(NodeController node)
    {
        if (graph.edges != null)
        {
            List<Connection> connectionsToRemove = new List<Connection>();

            for (int i = 0; i < graph.edges.Count; i++)
            {
                if (graph.edges[i].fromNode == node.inPoint || graph.edges[i].toNode == node.outPoint)
                {
                    connectionsToRemove.Add(graph.edges[i]);
                }
            }

            for (int i = 0; i < connectionsToRemove.Count; i++)
            {
                graph.edges.Remove(connectionsToRemove[i]);
            }

            connectionsToRemove = null;
        }

        graph.nodes.Remove(node);
    }

    private void OnClickRemoveConnection(Connection connection)
    {
        graph.edges.Remove(connection);
    }

    private void CreateConnection()
    {
        graph.edges.Add(new Connection(selectedInPoint, selectedOutPoint, OnClickRemoveConnection));
    }

    private void ClearConnectionSelection()
    {
        selectedInPoint = null;
        selectedOutPoint = null;
    }
}