using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slide_Wall : MonoBehaviour
{

    public GameObject floors;
    public int nbRocks;
    public GameObject[] otherWalls;

    [SerializeField]
    private AudioClip openIce;

    private GameObject[] iceFloors;
    private int nbFloor = 0;
    private int floorToUnlock;
    private List<Cracked_Ice> cracked_Ices = new List<Cracked_Ice>();

    // Start is called before the first frame update
    void Start()
    {   
        iceFloors = new GameObject[floors.transform.childCount];
        for( int i = 0; i < floors.transform.childCount; i++){
            iceFloors[i] = floors.transform.GetChild(i).gameObject;
        }
        foreach(GameObject iceFloor in iceFloors){
            iceFloor.GetComponent<Breakable_Ice>().SetSlideWall(this);
        }
        floorToUnlock = iceFloors.Length - nbRocks;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddFloor(){
        nbFloor++;
        if( nbFloor >= floorToUnlock ){
            AudioManager.instance.PlayWorldFx(openIce);
            gameObject.SetActive(false);
        }
    }

    public void ResetFloors(GameObject instance){
        StartCoroutine(Reset(instance));
    }

    private void ResetAll(){
        foreach (GameObject wall in otherWalls)
        {
            wall.GetComponent<Slide_Wall>().ResetWall();
            wall.SetActive(true);
        }
    }

    public void ResetWall(){
        foreach(Cracked_Ice cracked in cracked_Ices){
            cracked.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
        foreach(GameObject iceFloor in iceFloors){
            iceFloor.SetActive(true);
        }
        foreach(Cracked_Ice cracked in cracked_Ices){
            Destroy(cracked.gameObject);
        }
        nbFloor = 0;
        cracked_Ices.Clear();
    }

    public void AddCrackedIces(Cracked_Ice cracked){
        cracked_Ices.Add(cracked);
    }

    private IEnumerator Reset(GameObject instance){
        ResetAll();
        GameManager.instance.FallPlayer();
        foreach(Cracked_Ice cracked in cracked_Ices){
            cracked.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
        yield return new WaitForSeconds(GameManager.instance.fallTime);
        foreach(GameObject iceFloor in iceFloors){
            iceFloor.SetActive(true);
        }
        foreach(Cracked_Ice cracked in cracked_Ices){
            Destroy(cracked.gameObject);
        }
        Destroy(instance);
        nbFloor = 0;
        cracked_Ices.Clear();
    }
}
