using System;
using UnityEngine;

public class Ddol : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}