using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class GymLeader : NPC
{
    [Separator("Gym Leader")]
    [Space(10)]
    public Sprite portrait;
    public GameObject exitTeleporter;
    public AudioClip teleporterAppearAudio;
    public GameObject spritePortait;
    public AudioClip winGym;


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        exitTeleporter.SetActive(false);
    }

    // Update is called once per frame
    new void Update()
    {
        
    }

    public override void Talk(Vector3 lookAt){
        AudioManager.instance.PlayMusic(winGym);
        base.Talk(lookAt);
        spritePortait.SetActive(true);
        spritePortait.GetComponent<SpriteRenderer>().sprite = portrait;
    }

    public override void StopTalking(){
        spritePortait.SetActive(false);
        exitTeleporter.SetActive(true);
        AudioManager.instance.PlayWorldFx(teleporterAppearAudio);
    }
}
