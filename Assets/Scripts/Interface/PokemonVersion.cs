using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonVersion : MonoBehaviour
{   

    public GameObject[] versions;
    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject obj in versions){
            obj.SetActive(false);
        }
        versions[(int)Random.Range(0f,3f)].SetActive(true);
    }

}
