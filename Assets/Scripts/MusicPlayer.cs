using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    private AudioSource audio;
    private AudioClip intro;
    private AudioClip loop;
    private LevelMusic music;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        audio = gameObject.GetComponent<AudioSource>();
    }

    private void Update()
    {
        music = GameObject.Find("Level Music").GetComponent<LevelMusic>();
        if (intro != music.intro)
        {
            intro = music.intro;
            loop = music.loop;
            audio.clip = intro;
            audio.loop = false;
            audio.Play();
        }
        if (audio.clip == intro && audio.isPlaying == false)
        {
            audio.clip = loop;
            audio.loop = true;
            audio.Play();
        }
    }
}
