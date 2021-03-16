using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using MyBox;

public class MenuManager : MonoBehaviour
{   

    [Separator("Main Menu")]
    [Space(10)]
    public GameObject[] menuButtons;
    public AudioClip mainMenu;
    public Toggle fullscreen;
    public GameObject fullScreenDisable;

    [Separator("In Game")]
    [Space(10)]
    public GameObject menu;
    public GameObject pauseMenu;
    public GameObject optionMenu;
    public GameObject commandeMenu;
    public AudioClip openMenu;
    public AudioClip songMenu;

    public List<InputActionReference> buttonA;
    public Text aText;
    public List<InputActionReference> buttonB;
    public Text bText;
    public InputActionReference buttonStart;
    public Text startText;
    public InputActionReference buttonMove;
    public Text upText;
    public Text downText;
    public Text leftText;
    public Text rightText;

    public PlayerInput playerInput;
    public PlayerInput managerInput;
    private const string playerKey = "playerKey";
    private const string managerKey = "managerKey";


    private Player player;
    private bool pauseButton = false;
    private EventSystem eventSystem;


    // Start is called before the first frame update
    void Start()
    {   
        LoadKey();
        DisplayRebindText();
        player = GameObject.Find("Player").GetComponent<Player>();
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        GameObject.Find("Managers").BroadcastMessage("ReloadScene");
        if(Application.platform == RuntimePlatform.WebGLPlayer || Application.platform == RuntimePlatform.OSXEditor)
            fullscreen.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnStart(){
        if(SceneManager.GetActiveScene().buildIndex == 0 )
            return;
        pauseButton = !pauseButton;
        player.SetInputs(!pauseButton);
        if(pauseButton){
            AudioManager.instance.PlayMenuFx(openMenu);
            Time.timeScale = 0;
            menu.SetActive(true);
            eventSystem.SetSelectedGameObject(null);
            eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject);
        }else{
            AudioManager.instance.PlayMenuFx(songMenu);
            Time.timeScale = 1;
            commandeMenu.SetActive(false);
            optionMenu.SetActive(false);
            pauseMenu.SetActive(true);
            menu.SetActive(false);
        }
    }

    public void OnOption(){
        pauseMenu.SetActive(false);
        optionMenu.SetActive(true);
        SelectedButton(optionMenu);
        AudioManager.instance.PlayMenuFx(songMenu);
    }

    public void OnRetour(){
        pauseMenu.SetActive(true);
        optionMenu.SetActive(false);
        commandeMenu.SetActive(false);
        SelectedButton(pauseMenu);
        AudioManager.instance.PlayMenuFx(songMenu);
    }

    public void OnCommande(){
        optionMenu.SetActive(false);
        commandeMenu.SetActive(true);
        AudioManager.instance.PlayMenuFx(songMenu);
    }


    private void SelectedButton(GameObject obj){
        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(obj.transform.GetChild(0).gameObject);
    }

    public void SetFullScreen(bool b){
        if( b ){
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            Screen.fullScreen = b;
        }
        else 
            Screen.fullScreenMode = FullScreenMode.Windowed;

    }

    public void Volume(float v){
        AudioManager.instance.SetVolume(v);
    }

    private void LoadKey(){
        string rebinds = PlayerPrefs.GetString(managerKey, string.Empty);
        if (!string.IsNullOrEmpty(rebinds)) {
            managerInput.actions.LoadBindingOverridesFromJson(rebinds);
        }

        rebinds = PlayerPrefs.GetString(playerKey, string.Empty);
        if (!string.IsNullOrEmpty(rebinds)) {
            playerInput.actions.LoadBindingOverridesFromJson(rebinds);
        }
    }

    private void SaveKey(){
        string bind = managerInput.actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString(managerKey, bind);

        bind = playerInput.actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString(playerKey, bind);
    }

    private void DisplayRebindText(){
        aText.text = InputControlPath.ToHumanReadableString(
            buttonA[0].action.bindings[0].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);
        
        bText.text = InputControlPath.ToHumanReadableString(
            buttonB[0].action.bindings[0].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);
        
        startText.text = InputControlPath.ToHumanReadableString(
            buttonStart.action.bindings[0].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);

        upText.text = InputControlPath.ToHumanReadableString(
            buttonMove.action.bindings[1].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice); 

        downText.text = InputControlPath.ToHumanReadableString(
            buttonMove.action.bindings[2].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);

        leftText.text = InputControlPath.ToHumanReadableString(
            buttonMove.action.bindings[3].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice); 
        
        rightText.text = InputControlPath.ToHumanReadableString(
            buttonMove.action.bindings[4].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice); 
    }

    

    public void StartRebindingA(){
        AudioManager.instance.PlayMenuFx(songMenu);
        aText.text = "?";
        playerInput.SwitchCurrentActionMap("WaitMap");
        managerInput.SwitchCurrentActionMap("WaitMap");
        foreach(InputActionReference button in buttonA){
            InputActionRebindingExtensions.RebindingOperation rebinding = new InputActionRebindingExtensions.RebindingOperation(); 
            rebinding = button.action.PerformInteractiveRebinding()
                .WithControlsExcluding("Mouse")
                .WithCancelingThrough("<Keyboard>/escape")
                .OnComplete(operation => RebindComplete(aText,button,rebinding))
                .Start();
        }
    }

    public void StartRebindingB(){
        AudioManager.instance.PlayMenuFx(songMenu);
        bText.text = "?";
        playerInput.SwitchCurrentActionMap("WaitMap");
        managerInput.SwitchCurrentActionMap("WaitMap");
        foreach(InputActionReference button in buttonB){
            InputActionRebindingExtensions.RebindingOperation rebinding = new InputActionRebindingExtensions.RebindingOperation(); 
            rebinding = button.action.PerformInteractiveRebinding()
                .WithControlsExcluding("Mouse")
                .WithCancelingThrough("<Keyboard>/escape")
                .OnComplete(operation => RebindComplete(bText,button,rebinding))
                .Start();
        }
    }

    public void StartRebindingStart(){
        AudioManager.instance.PlayMenuFx(songMenu);
        startText.text = "?";
        managerInput.SwitchCurrentActionMap("WaitMap");
        InputActionRebindingExtensions.RebindingOperation rebinding = new InputActionRebindingExtensions.RebindingOperation(); 
        rebinding = buttonStart.action.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")
            .WithCancelingThrough("<Keyboard>/escape")
            .OnComplete(operation => RebindComplete(startText,buttonStart,rebinding))
            .Start();
    }

    public void StartRebindingMove(int index){
        AudioManager.instance.PlayMenuFx(songMenu);
        Text text = upText;
        switch(index)
        {
            case 1:
                text = upText;
                break;
            case 2:
                text = downText;
                break;
            case 3:
                text = leftText;
                break;
            case 4:
                text = rightText;
                break;
            default :
                Debug.Log("bug switch rebind move");
                break;
        }
        text.text = "?";
        playerInput.SwitchCurrentActionMap("WaitMap");
        InputActionRebindingExtensions.RebindingOperation rebinding = new InputActionRebindingExtensions.RebindingOperation(); 
        rebinding = buttonMove.action.PerformInteractiveRebinding(index)
            .WithControlsExcluding("Mouse")
            .WithCancelingThrough("<Keyboard>/escape")
            .OnComplete(operation => RebindComplete(text,buttonMove,rebinding,index))
            .Start();
    }


    private void RebindComplete(Text text,InputActionReference inputAction,
                                InputActionRebindingExtensions.RebindingOperation rebinding,int index = 0)
    {

        text.text = InputControlPath.ToHumanReadableString(
            inputAction.action.bindings[index].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);

        rebinding.Dispose();
        playerInput.SwitchCurrentActionMap("Gameplay");
        managerInput.SwitchCurrentActionMap("Managers");

        SaveKey();
    }

    public void OnContinuer(){
        //Load le niveau de la sauvegarde
        SceneManager.LoadScene(1);
        AudioManager.instance.PlayMenuFx(songMenu);
    }

    public void OnNouvellePartie(){
        //load une sauvegarde fraiche
        SceneManager.LoadScene(1);
        AudioManager.instance.PlayMenuFx(songMenu);
    }


    public void OnOptionMenu(){
        AudioManager.instance.PlayMenuFx(songMenu);
        foreach (GameObject button in menuButtons)
        {
            button.GetComponent<Button>().interactable = false;
        }
    }

    public void OnRetourMenu(){
        AudioManager.instance.PlayMenuFx(songMenu);
        foreach (GameObject button in menuButtons)
        {
            button.GetComponent<Button>().interactable = true;
        }
    }

    public void OnMenuPrincipale(){
        AudioManager.instance.PlayMenuFx(songMenu);
        SceneManager.LoadScene(0);
    }

    public void OnToogleDisable(){
        if(Application.platform == RuntimePlatform.WebGLPlayer || Application.platform == RuntimePlatform.OSXEditor)
            fullScreenDisable.SetActive(fullScreenDisable.activeSelf);
    }


}
