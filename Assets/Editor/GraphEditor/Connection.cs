using System;
using UnityEditor;
using UnityEngine;

public class Connection
{
    public NodeTransition condition;
    public ConnectionPoint inPoint;
    public ConnectionPoint outPoint;
    public Action<Connection> OnClickRemoveConnection;
    public Rect rect;
    private GUIStyle style;
    private int width = 150;
    private int height = 20;
    public Connection(ConnectionPoint inPoint, ConnectionPoint outPoint, Action<Connection> OnClickRemoveConnection)
    {
        style = new GUIStyle();
        style.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        style.border = new RectOffset(12, 12, 12, 12);
        this.inPoint = inPoint;
        this.outPoint = outPoint;
        this.OnClickRemoveConnection = OnClickRemoveConnection;
    }

    public void Draw()
    {
        Vector2 center = (inPoint.rect.center + outPoint.rect.center) * 0.5f;
        rect = new Rect(center.x-width/2, center.y, width, height);
        Handles.DrawBezier(
            inPoint.rect.center,
            outPoint.rect.center,
            inPoint.rect.center + Vector2.left * 50f,
            outPoint.rect.center - Vector2.left * 50f,
            Color.white,
            null,
            2f
        );

        if (Handles.Button(center, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
        {
            if (OnClickRemoveConnection != null)
            {
                OnClickRemoveConnection(this);
            }
        }

        GUILayout.BeginArea(rect, style);
        condition = EditorGUILayout.ObjectField(condition, typeof(NodeTransition), true) as NodeTransition;
        GUILayout.EndArea();
    }
}