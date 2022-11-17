using System.Collections.Generic;
using UnityEngine;
using OptimoveSdk;

public class OptimoveInit : MonoBehaviour
{
    void Awake()
    {
        Optimove.Initialize();
    }
}
