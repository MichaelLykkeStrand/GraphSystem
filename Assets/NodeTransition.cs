using System;
using UnityEngine;

public class NodeTransition : MonoBehaviour, Completeable
{
    public Action onComplete;

    public void Complete()
    {
        onComplete?.Invoke();
    }
}

public interface Completeable{
    public void Complete();
}
