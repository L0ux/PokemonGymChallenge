using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraController : MonoBehaviour
{   

    public float cameraSpeed = 10f;
    public Vector2 leftCorner =  new Vector2(0f,0f);
    public Vector2 rightCorner = new Vector2(0f,0f);

    private Bounds bounds;
    private float camVertExtent;
    private float camHorzExtent;

    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {   
        player = GameObject.Find("Player");
        camVertExtent = Camera.main.orthographicSize;
        camHorzExtent = (float)Math.Round((double)(Camera.main.aspect * camVertExtent));
    }

    // Update is called once per frame
    void Update()
    {      
        transform.position = Vector3.MoveTowards(transform.position,CalculatePosition(),cameraSpeed*Time.deltaTime);
    }

    public void RefreshBounds(Bounds b){
        if(player == null ){
            player = GameObject.Find("Player");
        }
        bounds = b;
        transform.position = CalculatePosition();
    }

    private Vector3 CalculatePosition(){
        float leftBound = bounds.min.x + camHorzExtent+1;
        float rightBound = bounds.max.x - camHorzExtent;
        float bottomBound = bounds.min.y + camVertExtent +1;
        float topBound = bounds.max.y - camVertExtent;
        float camY = Mathf.Clamp(player.transform.position.y, bottomBound, topBound);
        float camX;
        if(leftBound > rightBound ){ //Si la map est trop petite , ca centre au milieu
            float diff = bounds.min.x + (bounds.max.x - bounds.min.x) / 2.0f;
            camX = Mathf.Clamp(player.transform.position.x, diff, diff);
        }else{
            camX = Mathf.Clamp(player.transform.position.x, leftBound, rightBound);
        }
        return (new Vector3(camX,camY,-10f));
    }
}
