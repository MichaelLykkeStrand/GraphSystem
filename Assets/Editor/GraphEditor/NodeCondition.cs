using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NodeCondition : UnityEngine.Object, Completeable
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
