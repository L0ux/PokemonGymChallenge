using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoPokemon : MonoBehaviour
{   

    public float maxTime = 10f;

    private Animator animator;
    private float time = 0f;
    private float random = 0f;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if( time > random )
            animated();

    }

    private void animated(){
        animator.SetTrigger("animated");
        random = Random.Range(1.0f,maxTime);
        time = 0f;
    }
}
