using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class Teleporter : MonoBehaviour
{   

    public AudioClip audioTP;
    public AudioClip audioTPBack;
    public bool toAnotherTeleporter = false;

    [ConditionalField(nameof(toAnotherTeleporter),false)]
    public GameObject destinationTeleporter;

    [ConditionalField(nameof(toAnotherTeleporter),true)]
    public MapData destination;

    private bool ignoreTrigger = false;
    private AudioClip currentAudio;

    // Start is called before the first frame update
    void Start()
    {
        currentAudio = audioTP;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collider){
        if(collider.tag == "Player" && !ignoreTrigger ){
            collider.GetComponent<Player>().Wait(0.3f);
            StartCoroutine(TeleporterAnimation());
        }

    }

    IEnumerator TeleporterAnimation(){
        yield return new WaitForSeconds(0.3f);
        if(toAnotherTeleporter){
            destinationTeleporter.GetComponent<Teleporter>().IgnoreNextTrigger();
            destinationTeleporter.GetComponent<Teleporter>().TpBack();
            GameManager.instance.ChangePlayerPosition(destinationTeleporter.transform.position);
        }else
            GameManager.instance.ChangePlayerPosition(destination,(Vector3) destination.teleporterPosition);
        AudioManager.instance.PlayWorldFx(currentAudio);
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        ignoreTrigger = false;
    }

    public void IgnoreNextTrigger(){
        this.ignoreTrigger = true;
    }

    public void TpBack(){
        if( currentAudio == audioTP )
            currentAudio = audioTPBack;
        else   
            currentAudio = audioTP;
        
    }
}
