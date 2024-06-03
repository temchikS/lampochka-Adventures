using System;
using System.Collections;
using System.Collections.Generic;
using NavMeshPlus.Components;
using UnityEngine;

public class NavBaker : MonoBehaviour
{
    public NavMeshSurface Surface2D;


    public void BakeMap()
    {
        if (Surface2D == null)
        {
            Debug.LogError("Surface2D is not assigned in the inspector.");
            return;
        }
        Surface2D.BuildNavMeshAsync();
    }
}
