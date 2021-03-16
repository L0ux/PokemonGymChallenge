using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MyBox;

public class MovingObject : MonoBehaviour
{   
    [Separator("Moving Object")]
    [Space(10)]
    public float speed = 1f;
    public new string name = "";
    [SerializeField]
    private AudioClip fallAudio;

    protected bool isMoving = false;
    protected bool cantMove = false;
    protected BoxCollider2D boxCollider;
    protected Animator animator;
    protected IEnumerator activeCoroutine;
    protected bool recordMovement = false;
    protected bool isTalking = false;

    // Start is called before the first frame update
    public virtual void Start()
    {   
        boxCollider = this.GetComponent<BoxCollider2D>();
        animator = this.GetComponent<Animator>();
    }

    protected void Update()
    {
        
    }

    protected virtual bool AttemptMove(Vector2 dir){ 
        
        Vector3 end = transform.position + (Vector3)dir;

        if(cantMove)
            return false;

        //racyast pour voir l'objet qui empeche le déplacement
        int layerMask =~ LayerMask.GetMask("Ignore Raycast");
        RaycastHit2D[] results = new RaycastHit2D[1];
        if( ( (boxCollider.Raycast(dir,results,1,layerMask)  > 0 ) && !isMoving ) || GameManager.instance.ContainsMovement(end) ){
            animator.SetFloat("x",dir.x);
            animator.SetFloat("y",dir.y);
            Walk(false);
            return false;
        }

        if( !isMoving ){
            if(recordMovement){
                if( this.gameObject.tag == "Player")
                    GameManager.instance.AddMovement(end +new Vector3(.5f,.5f,0f));
                else
                    GameManager.instance.AddMovement(end);
            }
            animator.SetFloat("x",dir.x);
            animator.SetFloat("y",dir.y);
            if(!animator.GetBool("isMoving"))
                Walk(true);
            isMoving = true;
            activeCoroutine = SmoothMovement(end);
            StartCoroutine(activeCoroutine);
            return true;
        }
        return true;
    }

    protected void StopWalking(){
        animator.SetBool("isMoving",false);
        animator.SetBool("isBonging",false);
    }

    protected void Walk(bool b){
        animator.SetBool("isMoving",b);
        animator.SetBool("isBonging",!b);
    }

    private IEnumerator SmoothMovement( Vector3 end )
    {
        float sqrRemainingDistance = ( transform.position - end ).sqrMagnitude; 
        
        while ( sqrRemainingDistance > float.Epsilon && isMoving){
            transform.position = Vector3.MoveTowards(transform.position,end,speed*Time.deltaTime);
            sqrRemainingDistance = ( transform.position - end ).sqrMagnitude;
            yield return null;
        }
        if( this.gameObject.tag == "Player")
            GameManager.instance.RemoveMovement(end +new Vector3(.5f,.5f,0f));
        else
            GameManager.instance.RemoveMovement(end);

        isMoving = false;
    }

    protected IEnumerator StopMovement(float time){
        cantMove = true;
        StopWalking();
        yield return new WaitForSeconds(time);
        cantMove = false;
    }

    public void Fall(float time){
        StartCoroutine(FallAnimation(time));
    }

    private IEnumerator FallAnimation(float fallTime){
        yield return StartCoroutine(StopMovement(fallTime/2));
        StartCoroutine(StopMovement(fallTime/2));
        AudioManager.instance.PlayWorldFx(fallAudio);
        float time = 0f;
        while(time < fallTime/2){
            time = time + Time.deltaTime;
            transform.localScale = new Vector3(1f-time,1f-time,1f-time);
            transform.Translate(new Vector3(Time.deltaTime/2,Time.deltaTime/2,0f));
            yield return null;
        }

        transform.localScale = new Vector3(1f,1f,1f);
    }

    public void Wait(float time){
        StartCoroutine(StopMovement(time));
    }

    protected void Speak(Dialogue dialogue){
        dialogue.name = name;
        float time = DialogueManager.instance.StartSpeak(dialogue,(Vector2)transform.position);
        Wait(time);
    }

    void OnBecameInvisible()
    {  
        recordMovement = false;
    }

    void OnBecameVisible()
    {   
        recordMovement = true;
    }

    public Vector2 GetPosition(){
        return (Vector2) this.transform.position;
    }

    public bool IsMoving(){
        return isMoving;
    }

}
