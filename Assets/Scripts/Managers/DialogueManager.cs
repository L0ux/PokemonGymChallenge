using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{

    public static DialogueManager instance;

    private GameObject textBox;
    private Tilemap uiTilemap;
    public TileBase winowsTile;
    public Font font;
    public AudioClip textAudio;
    
    private float timeAfterSentence = 1f;
    private float charDelay = 0.05f;
    private float dialogueSpeed = 1f;
    private int charPerLigne = 25;
    private Queue<string> dialogues;
    private List<Queue<string>> npcSpeak;
    private GameObject speakCanvas;
    private GameObject arrow;
    private Player player;
    private bool isTalking = false;
    private bool textPassButton = false;
    private bool speedTextButton = false;
    private bool endOfLine = false;
    
    // Start is called before the first frame update
    void Start()
    {   
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Awake(){
        MakeSingleton();
    }

    private void MakeSingleton(){
        if( instance != null ){
            Destroy(gameObject);
        }else{
            instance  = this;
            DontDestroyOnLoad(gameObject);
        }
    }


    public void StartDialogue(Dialogue dialogue,NPC npc){
        player.SetInputs(false);
        textBox.SetActive(true);
        StartCoroutine(Talking(dialogue,npc));
        AudioManager.instance.PlayMenuFx(textAudio);
        isTalking = true;
    }

    IEnumerator Talking(Dialogue dialogue,NPC npc){
        textBox.transform.GetChild(0).transform.GetComponent<Text>().text = dialogue.name;
        yield return StartCoroutine(TypeSentence(dialogue.sentences,textBox.transform.GetChild(1).transform.GetComponent<Text>(),true));
        isTalking = false;
        speedTextButton = false;
        npc.StopTalking();
        textBox.SetActive(false);
        player.SetInputs(true);
    }

    public void OnTextPass(){
        if(endOfLine){
            textPassButton = true;
        }
    }


    public void OnSpeedText(){
        if(isTalking){
            speedTextButton = !speedTextButton;
            if(speedTextButton){
                dialogueSpeed = 10f;
            }
            else{
                dialogueSpeed = 1f;
            }
        }
    }

    public float StartSpeak(Dialogue dialogue,Vector2 pos){
        Vector3Int position = uiTilemap.WorldToCell(new Vector3(pos.x,pos.y,0));
        int nbSentences = dialogue.sentences.Length;
        int sizeMax = 0;
        int nbChars = 0;
        foreach(string sentence in dialogue.sentences){
            nbChars += sentence.Length;
            if( sentence.Length > sizeMax )
                sizeMax = sentence.Length;
        }
        int nbLignes = (sizeMax / charPerLigne) + 1;
        Bounds area = new Bounds();
        area.min.x = position.x - 2;
        area.min.y = position.y + 5;
        if( nbLignes == 1 ){
            if( sizeMax < 6)
                area.max.x = area.min.x + sizeMax + 2;
            else if( sizeMax > 13 )
                area.max.x = area.min.x + (int)(sizeMax*0.8f);
            else
                area.max.x = area.min.x + (int)(sizeMax*0.9);
            area.max.y =  area.min.y + 4;
        }else{
            area.max.x = area.min.x + (int)(charPerLigne*0.7f);
            area.max.y =  area.min.y + (int)((nbLignes*2) + 2);
        }
        for( int i = area.min.x; i < area.max.x; i++){
            for( int j = area.min.y; j < area.max.y; j++){
                uiTilemap.SetTile(new Vector3Int(i,j,0),winowsTile);
            }
        }
        GameObject textObject = CreateText(area,dialogue.name);
        StartCoroutine(Speaking(dialogue,textObject,area));
        return (nbChars * charDelay + nbSentences * timeAfterSentence + timeAfterSentence);
    }

    IEnumerator Speaking(Dialogue dialogue,GameObject obj,Bounds area){
        yield return StartCoroutine(TypeSentence(dialogue.sentences,obj.GetComponent<Text>(),false));
        Destroy(obj);
        for( int i = area.min.x; i < area.max.x; i++){
            for( int j = area.min.y; j < area.max.y; j++){
                uiTilemap.SetTile(new Vector3Int(i,j,0),null);
            }
        }
        yield return new WaitForSeconds(timeAfterSentence);
    }

    IEnumerator TypeSentence(string[] sentences,Text text,bool wait){
        foreach(string sentence in sentences ){
            text.text = "";
            foreach(char letter in sentence.ToCharArray()){
                text.text = text.text + letter;
                yield return new WaitForSeconds(charDelay/dialogueSpeed);
            }
            if(wait){
                endOfLine = true;
                arrow.SetActive(true);
                int charLignes = 60;
                TextGenerator textGenerator = text.cachedTextGenerator;
                IList<UICharInfo> characters = textGenerator.characters;
                IList<UILineInfo> lines = textGenerator.lines;
                int nbLignes = (characters.Count/charLignes)+1;
                int nbChars = 0;
                float multiplicateur = 0f;
                if(lines.Count == 2 )
                    nbChars = characters.Count - lines[1].startCharIdx;
                else
                    nbChars = characters.Count % charLignes;
                
                if( nbChars < 20)
                    multiplicateur = 16f;
                else if(nbChars < 35)
                    multiplicateur = 16f;
                else
                    multiplicateur = 15f;

                int xPosition = -525 + (int)(nbChars*multiplicateur); //des valeurs brut de partout mais bon
                if(textGenerator.lineCount > 1 || nbLignes > 1 ){
                    arrow.transform.localPosition = new Vector3(xPosition,-35f,0f);
                }else{
                    arrow.transform.localPosition = new Vector3(xPosition,8,0f);
                }
                while(!textPassButton){
                    yield return null;
                }
                arrow.SetActive(false);
                AudioManager.instance.PlayMenuFx(textAudio);
                dialogueSpeed = 1f;
                textPassButton = false;
                endOfLine = false;
            }else{
                yield return new WaitForSeconds(timeAfterSentence);  
            } 
        }
    }

    private GameObject CreateText(Bounds bounds,String name){
        Vector2 leftDownCorner = uiTilemap.CellToWorld(new Vector3Int(bounds.min.x+1,bounds.min.y+1,0));
        Vector2 leftUpCorner = uiTilemap.CellToWorld(new Vector3Int(bounds.min.x+1,bounds.max.y-1,0));
        Vector2 rightDownCorner = uiTilemap.CellToWorld(new Vector3Int(bounds.max.x,bounds.min.y+1,0));

        GameObject textObject = new GameObject(name+" speak");
        textObject.transform.SetParent(speakCanvas.transform);

        RectTransform rectTransform = textObject.AddComponent<RectTransform>();
        rectTransform.anchoredPosition = leftDownCorner;

        rectTransform.localScale = new Vector3(0.4f,0.4f,0.4f);
        rectTransform.pivot = new Vector2(0f,0f);

        float width = Vector2.Distance(rightDownCorner,leftDownCorner)*2.5f;
        float height = Vector2.Distance(leftDownCorner,leftUpCorner)*2.5f;
        rectTransform.sizeDelta = new Vector2(width,height);

        Text text = textObject.AddComponent<Text>();
        text.text = "";
        text.font = font;
        text.fontSize = 1;
        text.color = Color.black;
        text.alignment = TextAnchor.MiddleLeft;

        return textObject;
    }

    private void ReloadScene(){
        if(SceneManager.GetActiveScene().buildIndex != 0 ){
            uiTilemap = GameObject.Find("Speak").GetComponent<Tilemap>();
            textBox = GameObject.Find("Text Box");
            arrow = textBox.transform.GetChild(2).gameObject;
            arrow.SetActive(false);
            textBox.SetActive(false);
            speakCanvas = GameObject.Find("SpeakCanvas");
            player = GameObject.Find("Player").GetComponent<Player>();
        }
    }
}
