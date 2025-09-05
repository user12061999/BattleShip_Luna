using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    public static AudioManager ins;
    public AudioSource sound;
    public AudioSource music;
    public AudioClip rewardSound;
    public AudioClip FireSound;
    public AudioClip loseSound;
    public AudioClip bgSound;
    public AudioClip ExplorSound;
    public AudioClip miningSound;
    public List<AudioClip> lstMoveSound;

    private void Awake()
    {
        ins = this;
    }

    private void Start()
    {
        if (bgSound)
        {
            PlayMusic();
        }
    }

    public void PlaySound(AudioClip audioClip)
    {
        sound.PlayOneShot(audioClip,1);
    }

   
    public void PlaySoundMove()
    {
        sound.PlayOneShot(lstMoveSound[Random.Range(0,lstMoveSound.Count)],1);
    }
    public void PlaySoundReward()
    {
        sound.PlayOneShot(rewardSound,1);
    }
    public void PlaySoundFire()
    {
        sound.PlayOneShot(FireSound,1);
    }
    public void PlaySoundExplor()
    {
        sound.PlayOneShot(ExplorSound,1);
    }
    public void PlayMiningSound()
    {
        sound.PlayOneShot(miningSound,1);
    }
    public void PlayMusicLose()
    {
        music.loop = false;
        music.clip = loseSound;
        music.Play();
    }
    public void PlayMusic()
    {
        music.loop = true;
        music.clip = bgSound;
        music.Play();
    }
}