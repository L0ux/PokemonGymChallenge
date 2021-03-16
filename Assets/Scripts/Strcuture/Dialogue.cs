using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct Dialogue
{   
    [HideInInspector]
    public string name;
    [TextArea(3,10)]
    public string[] sentences;
}
