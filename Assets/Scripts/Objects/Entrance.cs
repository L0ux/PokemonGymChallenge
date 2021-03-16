using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MyBox;

public class Entrance : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioClip entranceAudio;
    public MapData mapData;

    public bool isExit = false;
    [ConditionalField(nameof(isExit), true)]
    public Entrance nextEntrance;
    [ConditionalField(nameof(isExit), false)]
    public string nextScene;

    private BoxCollider2D boxCollider;
    private bool ignoreTrigger = false;

    void Start()
    {
        boxCollider = this.GetComponent<BoxCollider2D>();
    }

    void  OnTriggerEnter2D(Collider2D collider){
        if(collider.tag == "Player" && !ignoreTrigger ){
            collider.GetComponent<Player>().Wait(0.3f);
            StartCoroutine(EntranceAnimation());
        }
    }

    private IEnumerator EntranceAnimation(){
        yield return new WaitForSeconds(0.3f);
        if(nextEntrance != null && !isExit ){
                nextEntrance.IgnoreNextTrigger();
                AudioManager.instance.PlayWorldFx(entranceAudio);
                GameManager.instance.ChangePlayerPosition(nextEntrance.mapData,nextEntrance.transform.position-new Vector3(0.5f,0.5f,0f));
        }

        if( isExit ){
            SceneManager.LoadScene(nextScene);
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        ignoreTrigger = false;
    }

    public void IgnoreNextTrigger(){
        this.ignoreTrigger = true;
    }


}

