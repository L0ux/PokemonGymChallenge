using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Cracked_Ice : MonoBehaviour
{   

    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    private AudioClip breakedAudio;
    

    private bool ignoreTrigger = false;
    private Slide_Wall slide_Wall;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collider){
        if(!ignoreTrigger){
            GameObject instance = Instantiate(prefab,transform.position,Quaternion.identity);
            AudioManager.instance.PlayWorldFx(breakedAudio);
            slide_Wall.gameObject.SetActive(true);
            slide_Wall.ResetFloors(instance);
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        ignoreTrigger = false;
    }

    public void IgnoreNextTrigger(Slide_Wall slide){
        slide_Wall = slide;
        slide_Wall.AddCrackedIces(this);
        ignoreTrigger = true;
    }

}
