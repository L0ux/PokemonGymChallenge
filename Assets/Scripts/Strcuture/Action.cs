using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MyBox;

[Serializable]
public enum ActionType
{
    Walk,
    Wait,
    Speak,
    DetectePlayer,
    WalkAround,
    None
}

[Serializable]
public enum VDirection
{
    Down,
    Up,
    Left,
    Right
}

[Serializable]
public class Action
{

    public ActionType type = ActionType.None;

    [ConditionalField(nameof(type), false, ActionType.Wait)]
    public float waitTime;

    [SerializeField]
    [ConditionalField(nameof(type), false, ActionType.Walk)]
    private VDirection direction;    

    [ConditionalField(nameof(type), false, ActionType.Speak,ActionType.DetectePlayer)]
    public Dialogue dialogue;

    [ConditionalField(nameof(type), false, ActionType.DetectePlayer)]
    public int radius;

    [ConditionalField(nameof(type), false, ActionType.WalkAround)]
    public int nbSteps;

    private int steps = 0;


    public Vector2 Dir(){
        Vector2 dir = new Vector2(0f,0f);

        if(direction == VDirection.Up)
            dir = new Vector2(0f,1f);

        if(direction == VDirection.Down)
            dir = new Vector2(0f,-1f);

        if(direction == VDirection.Left)
            dir = new Vector2(-1f,0f);

        if(direction == VDirection.Right)
            dir = new Vector2(1f,0f);
        
        return dir;
    }

    public bool Process(NPC npc){
        switch(type){
            case ActionType.Walk:
                return WalkAction(npc);
            case ActionType.Wait:
                return WaitAction(npc);
            case ActionType.Speak:
                return SpeakAction(npc);
            case ActionType.DetectePlayer:
                return DetecteAction(npc);
            case ActionType.WalkAround:
                return WalkAroundAction(npc);
            default:
                Debug.Log("bug switch action");
                break;
        } 
        return true; 
    }
    
    private bool WalkAction(NPC npc){
        return npc.NpcMove(this.Dir());
    }

    private bool WaitAction(NPC npc){
        npc.Wait(this.waitTime);
        return true;
    }

    private bool SpeakAction(NPC npc){
        npc.NpcSpeak(this.dialogue);
        return true;
    }

    private bool DetecteAction(NPC npc){
        if(!GameManager.instance.PlayerAround(npc.GetPosition(),radius))
            return false;
        npc.NpcSpeak(this.dialogue);
        return true;
    }

    private bool WalkAroundAction(NPC npc){
        bool hasWalked = false;
        int alea = (int)(UnityEngine.Random.Range(0f,4f));
        switch (alea)
        {
            case 0:
                direction = VDirection.Up;
                break;
            case 1:
                direction = VDirection.Down;
                break;
            case 2:
                direction = VDirection.Left;
                break;
            case 3:
                direction = VDirection.Right;
                break;
            default:
                Debug.Log("Bug switch WalkAroundAction");
                break;
        }
        hasWalked = npc.NpcMove(this.Dir());

        if( hasWalked )
            steps++;

        if(steps == nbSteps)
            steps = 0;
        else
            hasWalked = false;
        
        return hasWalked;
    }
    
}


