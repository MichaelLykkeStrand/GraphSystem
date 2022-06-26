using System;
using UnityEngine;

[System.Serializable]
public class NodeTransition : MonoBehaviour, Completeable
{
    public Action onComplete;
    [SerializeField] private string m_ID = Guid.NewGuid().ToString();
    public string ID => m_ID;

    public void Complete()
    {
        onComplete?.Invoke();
    }
}

public interface Completeable{
    public void Complete();
}
