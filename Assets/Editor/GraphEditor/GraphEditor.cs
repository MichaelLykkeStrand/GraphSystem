using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

public class GraphEditor : EditorWindow
{
    private Graph graph;
    private NodeController nodeController;
    private GridController gridController;
    private EdgeController edgeController;

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
        NewEmptyGraph();
    }

    private void NewEmptyGraph() {
        //Replace with builder pattern
        graph = ScriptableObject.CreateInstance<Graph>();
        graph.Edges = new List<Edge>();
        graph.Nodes = new List<Node>();
        nodeController = new NodeController(graph);
        edgeController = new EdgeController(graph);
        gridController = new GridController(this);
    }

    private void OnGUI()
    {
        DrawTools();

        gridController.DrawGrid(offset,20, 0.2f, Color.gray);
        gridController.DrawGrid(offset,100, 0.4f, Color.gray);

        ProcessEvents(Event.current);
        nodeController.Draw();
        nodeController.ProcessNodeEvents(Event.current);
        DrawEdges();
        DrawEdge(Event.current);
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
    //TODO update this and move the nodeController creation to a new function that includes edgecontroller etc.
    private void OpenGraph()
    {
        try
        {
            AssetDatabase.Refresh();
            string absPath = EditorUtility.OpenFilePanel("Select Graph", "", "asset");
            string relativePath = absPath.Substring(absPath.IndexOf("Assets/"));
            this.graph = AssetDatabase.LoadAssetAtPath<Graph>(relativePath);
            PopulateNodeTransitionReferences();
            nodeController = new NodeController(graph);
        }
        catch (Exception)
        {
        }

    }



    //move to own graph.cs
    private void PopulateNodeTransitionReferences()
    {
        try
        {
        NodeTransition[] nodeTransitons = GameObject.FindObjectsOfType<NodeTransition>();
        Debug.Log("Populating node transitions: "+nodeTransitons.Length);            
            foreach (NodeTransition nodeTransition in nodeTransitons)
            {
                foreach (Edge edge in graph.Edges)
                {
                    if (nodeTransition.ID == edge.transitionID)
                    {
                        edge.condition = nodeTransition;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("Unable to populate node transitions. Are you in the correct scene?");
            NewEmptyGraph();
        }
    }
    
    private void SaveGraph()
    {
        string absPath = EditorUtility.SaveFilePanel("Save Graph", "", "graph","asset");
        string relativePath = absPath.Substring(absPath.IndexOf("Assets/"));
        AssetDatabase.CreateAsset(graph, relativePath);
        AssetDatabase.SaveAssets();
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





    private void DrawEdges()
    {
        foreach (Edge edge in new List<Edge>(graph.Edges))
        {
            DrawEdge(edge);
        }
    }

    private void DrawEdge(Edge edge) { 
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
        if (edge.condition != null)
        {
            if (edge.condition.ID != edge.transitionID)
            {
                edge.transitionID = edge.condition.ID;
            }
        }
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
                    try{
                        graph.GetNodeAtPosition(e.mousePosition);
                        if(selectedInPoint == null && nodeController.HasSelectedNode())
                        {
                            selectedInPoint = graph.GetNodeAtPosition(e.mousePosition);
                        } else if(selectedInPoint != null && selectedOutPoint == null)
                        {
                            selectedOutPoint = graph.GetNodeAtPosition(e.mousePosition);
                            graph.Connect(selectedInPoint, selectedOutPoint);
                            ClearConnectionSelection();
                        }
                    }
                    catch(NoNodeFoundException)
                    {
                        ClearConnectionSelection();
                    }
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

    private void DrawEdge(Event e)
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

        //Remove

        GUI.changed = true;
    }

    private void OnClickAddNode(Vector2 mousePosition)
    {
        graph.AddNode(new Node(mousePosition));
    }


    private void OnClickRemoveNode(Node node)
    {
        graph.RemoveNode(node);
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