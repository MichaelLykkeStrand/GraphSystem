using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GraphGUIStyles
{

    public static GUIStyle DefaultNodeStyle()
    {
        GUIStyle defaultNodeStyle = new GUIStyle();
        defaultNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        defaultNodeStyle.border = new RectOffset(12, 12, 12, 12);

        return defaultNodeStyle;
    }

    public static GUIStyle SelectedNodeStyle()
    {
        GUIStyle selectedNodeStyle = new GUIStyle();
        selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);

        return selectedNodeStyle;
    }
}
