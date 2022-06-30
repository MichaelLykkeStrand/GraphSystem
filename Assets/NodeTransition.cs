using System;
using UnityEngine;

[System.Serializable]
public class NodeTransition : MonoBehaviour, ICompleteable
{
    public Action onComplete;

    public void Complete()
    {
        onComplete?.Invoke();
    }
}

public interface ICompleteable{
    public void Complete();
}