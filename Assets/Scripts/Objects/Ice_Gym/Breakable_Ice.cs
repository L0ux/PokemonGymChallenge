using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Breakable_Ice : MonoBehaviour
{   

    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    private AudioClip crackedAudio;

    private Slide_Wall slideWall;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void  OnTriggerEnter2D(Collider2D collider){
        GameObject isntance = Instantiate(prefab,transform.position,Quaternion.identity);
        isntance.GetComponent<Cracked_Ice>().IgnoreNextTrigger(slideWall);
        AudioManager.instance.PlayWorldFx(crackedAudio);
        isntance.transform.parent = transform.parent;
        gameObject.SetActive(false);
        slideWall.AddFloor();
    }

    public void SetSlideWall(Slide_Wall s){
        slideWall = s;
    }
    
}
