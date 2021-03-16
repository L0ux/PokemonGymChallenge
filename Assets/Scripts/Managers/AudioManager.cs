using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{   

    public static AudioManager instance;

    public AudioSource musicSource;
    public AudioSource playerFxSource;
    public AudioSource menuFxSource;
    public AudioSource worldFxSource;
    public AudioMixer mixer;
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

    public void PlayPlayerFx(AudioClip audio){
        if(!playerFxSource.isPlaying){
            playerFxSource.clip = audio;
            playerFxSource.Play();
        }
    }

    public void PlayMenuFx(AudioClip audio){
        if(!menuFxSource.isPlaying){
            menuFxSource.clip = audio;
            menuFxSource.Play();
        }
    }

    public void PlayWorldFx(AudioClip audio){
        worldFxSource.clip = audio;
        worldFxSource.Play();
    }

    public void PlayMusic(AudioClip audio){
        if(audio == null)
            return;
        if(musicSource.clip != audio){
            musicSource.clip = audio;
            musicSource.Play();
        }
    }

    public void SetVolume(float volume){
        mixer.SetFloat("volume",volume);
    }
}
