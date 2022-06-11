using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GraphEditor : EditorWindow
{
    [MenuItem ("Window/GraphEditor")]
    public static void  ShowWindow () {
        EditorWindow.GetWindow(typeof(GraphEditor));
    }
    
    private void OnGUI () {
        // The actual window code goes here
    }

    private void DrawNodes()
    {
    }

    private void ProcessEvents(Event e)
    {
    }
}
