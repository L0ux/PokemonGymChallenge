using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Threading;


public class GameManager : MonoBehaviour
{   

    public static GameManager instance;
    
    [HideInInspector]
    public float fallTime = 1f;
    private float fadeTime = 0.3f;
    private GameObject canvas;
    private Player player;
    private List<Vector2> movements;

    void Awake(){
        MakeSingleton();
    }

    // Start is called before the first frame update
    void Start()
    {   //plein de chose a faire ici pour gérer la sauvegarde / placement du joueur 
        movements = new List<Vector2>();
        player = GameObject.Find("Player").GetComponent<Player>();
        canvas = GameObject.Find("Canvas");                                                
        AudioManager.instance.PlayMusic(player.currentMapData.levelMusic);
        if(SceneManager.GetActiveScene().buildIndex != 0 )
            Camera.main.GetComponent<CameraController>().RefreshBounds(player.currentMapData.bounds);   
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void MakeSingleton(){
        if( instance != null ){
            Destroy(gameObject);
        }else{
            instance  = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void MapChanged(MapData map){
        player.currentMapData = map;                           
        AudioManager.instance.PlayMusic(player.currentMapData.levelMusic);
        Camera.main.GetComponent<CameraController>().RefreshBounds(player.currentMapData.bounds);
    }

    public bool AllowRun(){
        return player.currentMapData.allowRun;
    }


    private IEnumerator FadeOut(float fadeSpeed){
        Image image = canvas.transform.GetChild(0).GetComponent<Image>();
        Color color = Color.black;
        image.color = color;

        while(image.color.a > 0){
            color.a = color.a - (fadeSpeed * Time.deltaTime);
            image.color = color;
            yield return null;
        }
    }

    private IEnumerator WaitColor(float time){
        Camera.main.GetComponent<CameraController>().cameraSpeed = 100f;
        Image image = canvas.transform.GetChild(0).GetComponent<Image>();
        image.color = Color.black;
        Color transparant = Color.black;
        transparant.a = 0;
        yield return new WaitForSeconds(time);
        Camera.main.GetComponent<CameraController>().cameraSpeed = 10f;
        image.color = transparant;
    }

    public void ChangePlayerPosition(MapData map,Vector3 position){
        MapChanged(map);
        MovePlayer(position);
    }

    public void ChangePlayerPosition(Vector3 position){
        MovePlayer(position);
    }


    private void MovePlayer(Vector3 position){
        player.ChangePosition(position);
        StartCoroutine(WaitColor(fadeTime));
    }  

    public void FallPlayer(){
        player.Fall(fallTime);
        StartCoroutine(GoToUnderMapPosition());
    }

    private IEnumerator GoToUnderMapPosition(){
        yield return new WaitForSeconds(fallTime);
        if(player.currentMapData.underMap == null )
            MovePlayer((Vector3)player.currentMapData.spawnPosition);
        else
            MovePlayer((Vector3)player.currentMapData.underMap.spawnPosition);
        player.currentMapData = player.currentMapData.underMap;
        AudioManager.instance.PlayMusic(player.currentMapData.levelMusic);
        Camera.main.GetComponent<CameraController>().RefreshBounds(player.currentMapData.bounds);
    }

    public void AddMovement(Vector2 m){
        movements.Add(m);
    }

    public void RemoveMovement(Vector2 m){
        movements.Remove(m);
    }

    public bool ContainsMovement(Vector2 m){
        return movements.Contains(m);
    }

    public void PlayerInputs(bool b){
        player.SetInputs(b);
    }

    public bool PlayerAround(Vector2 position,int radius){
        if(Vector2.Distance(position,player.GetGridPosition()) > radius)
            return false;
        else 
            return true;
    }

    private void ReloadScene(){         
        Time.timeScale = 1;                                 
        canvas = GameObject.Find("Canvas"); 
        player = GameObject.Find("Player").GetComponent<Player>();
        AudioManager.instance.PlayMusic(player.currentMapData.levelMusic);
        movements.Clear();
        if(SceneManager.GetActiveScene().buildIndex != 0 )
            Camera.main.GetComponent<CameraController>().RefreshBounds(player.currentMapData.bounds);     
    }

}
