using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Threading;
using MyBox;

public class Player : MovingObject
{
    // Start is called before the first frame update

    [Separator("Player")]
    [Space(10)]
    public float runningSpeed = 2f;
    public AudioClip bongFx;
    public MapData currentMapData;

    private bool runButton = false;
    private bool moveButton = false;
    private float baseSpeed;
    private Vector2 lastDir = new Vector2(0f,0f);
    private bool inputEnable = true;

    private Queue<Vector2> buffer;


    new void Start()
    {   
        base.Start();
        baseSpeed = speed;
        buffer = new Queue<Vector2>();
    }

    // Update is called once per frame
    new void Update()
    {    

        if(!isMoving)
            StopWalking();

        if((buffer.Count > 0) && !cantMove && inputEnable && !isMoving){
            Vector2 dir = new Vector2(0f,0f);
            if(buffer.Count == 0){
                dir = lastDir;
            }else{
                dir = buffer.Dequeue();
                lastDir = dir;
            }
            AttemptMove(dir);
        }

        if((buffer.Count == 0) && moveButton)
            AttemptMove(lastDir);

        if(!isMoving)
            StopWalking();
    }

    protected new bool AttemptMove(Vector2 dir){
        bool canMove = base.AttemptMove(dir);

        if((!canMove)){
            //racyast pour voir l'objet qui empeche le déplacement
            AudioManager.instance.PlayPlayerFx(bongFx);
            return false;
        }

        return true;
    }


    void OnMove(InputValue input){ 
        moveButton = !moveButton;
        if(inputEnable){ 
            if(moveButton){
                if(buffer.Count < 1)
                    buffer.Enqueue(input.Get<Vector2>());
                else{
                    buffer.Enqueue(buffer.Dequeue());
                    buffer.Dequeue();
                    buffer.Enqueue(input.Get<Vector2>());
                }
            }
        }
    }

    void OnRun(){
        if(!GameManager.instance.AllowRun()){
            runButton = false;
            speed = baseSpeed;
            animator.SetBool("isRunning",false);
            return;
        }
        
        runButton = !runButton;
        if(runButton){
            if(!animator.GetBool("isRunning"))
                animator.SetBool("isRunning",true);
            speed = runningSpeed;
        }else{
            speed = baseSpeed;
            animator.SetBool("isRunning",false);
        }
    }

    void OnAction(){
        if(inputEnable){
            StopWalking();
            animator.SetTrigger("action");
            int layerMask =~ LayerMask.GetMask("Ignore Raycast");
            Vector2 direction = new Vector2(animator.GetFloat("x"),animator.GetFloat("y"));
            RaycastHit2D[] results = new RaycastHit2D[1];
            if( boxCollider.Raycast(direction,results,1,layerMask)  > 0 ){
                if(results[0].transform.tag == "NPC" ){
                    Vector3 lookAt = (transform.position-results[0].transform.position+new Vector3(0.5f,0.5f,0f));
                    results[0].transform.gameObject.GetComponent<NPC>().Talk(lookAt);   
                }  
            }
        }
    }

    public void ChangePosition(Vector3 position){
        StopCoroutine(activeCoroutine);
        StartCoroutine(StopMovement(0.3f));
        isMoving = false;
        transform.position = position;
        if(!GameManager.instance.AllowRun())
            OnRun();
    }

    public void SetInputs(bool b){
        inputEnable = b;
    }

    public Vector2 GetGridPosition(){
        return ((Vector2)(transform.position) + new Vector2(.5f,.5f));
    }



}
