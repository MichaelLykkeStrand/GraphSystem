using System;
using UnityEngine;

[System.Serializable]

public class SerializeSceneReferenceAttribute : Attribute
{
    [SerializeField] private string m_ID = Guid.NewGuid().ToString();
    public string ID => m_ID;
}