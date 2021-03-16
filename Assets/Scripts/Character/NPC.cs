using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MyBox;

public class NPC : MovingObject
{   

    [Separator("NPC")]
    [Space(10)]
    public Dialogue dialogue;
    public Action[] actions;
    public VDirection orientation;
    public bool newDialogue = false;

    [ConditionalField(nameof(newDialogue), false)]
    public Dialogue secondeDialogue;
    [ConditionalField(nameof(newDialogue), false)]
    public bool stopAction = false;


    private float actionDelay = 1f;
    private Queue<Action> actionsQueue;
    private Action nextAction;
    private float time;
    private bool hasMoved = true;
    private bool stop = false;
    
    // Start is called before the first frame update
    public override void Start()
    {    
        base.Start();
        time = 0f;
        dialogue.name = name;
        secondeDialogue.name = name;
        StartQueue();
        if(GetComponent<Animator>() != null)
            SetDirection();
    }

    // Update is called once per frame
    new void Update()
    {   

        if(actionsQueue.Count == 0 || (stopAction && stop) )
            return;

        time += Time.deltaTime;

        if(!isMoving)
            StopWalking();

        if(cantMove || isTalking)
            return;

        if(time > actionDelay ){
            time = 0f;

            if(hasMoved)
                nextAction = actionsQueue.Dequeue();

            hasMoved =  nextAction.Process(this.GetComponent<NPC>());
        
        }

        if( actionsQueue.Count == 0 )
            RestartQeue();
    }

    public virtual void Talk(Vector3 lookAt){
        if(GetComponent<Animator>() != null)
            animator.SetFloat("x",lookAt.x);
            animator.SetFloat("y",lookAt.y);
        isTalking = true;
        DialogueManager.instance.StartDialogue(dialogue,this.GetComponent<NPC>());
    }

    public virtual void StopTalking(){
        isTalking = false;
        SetDirection();
        if(newDialogue)
            dialogue = secondeDialogue;
            stop = stopAction;
    }
    
    private void StartQueue(){
        actionsQueue = new Queue<Action>();
        foreach(Action action in actions){
            actionsQueue.Enqueue(action);
        }
    }

    private void RestartQeue(){
        actionsQueue.Clear();
        foreach(Action action in actions){
            actionsQueue.Enqueue(action);
        }
    }

    private void SetDirection(){
        Vector2 dir = new Vector2(0f,0f);

        if(orientation == VDirection.Up)
            dir = new Vector2(0f,1f);

        if(orientation == VDirection.Down)
            dir = new Vector2(0f,-1f);

        if(orientation == VDirection.Left)
            dir = new Vector2(-1f,0f);

        if(orientation == VDirection.Right)
            dir = new Vector2(1f,0f);
        
        animator.SetFloat("x",dir.x);
        animator.SetFloat("y",dir.y);
    }

    public void NpcSpeak(Dialogue dialogue){
        Speak(dialogue);
    }

    public bool NpcMove(Vector2 dir){
        return AttemptMove(dir);
    }
}







