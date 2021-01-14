using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chair : MonoBehaviour
{
    bool isFree = true;

    public bool IsFree()
    {
        return isFree;
    }

    public void Reserve()
    {
        isFree = false;
    }

    public void Leave()
    {
        isFree = true;
    }
}
