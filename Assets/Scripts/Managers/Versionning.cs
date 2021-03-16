using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Versionning : MonoBehaviour
{
    void Start()
    {
        GetComponent<Text>().text = "Version " +Application.version;
    }
}
