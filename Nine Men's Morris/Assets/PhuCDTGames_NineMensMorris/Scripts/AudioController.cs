using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAdd_NineMensMorris
{
    public class AudioController : MonoBehaviour
{
    public AudioClip millCreated;
    public AudioClip chessRemoved;
    public AudioClip chessMoved;
    public AudioClip win;
    public AudioClip lose;

    public AudioSource speaker;

    public static AudioController Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void PlayMillCreated()
    {
        speaker.PlayOneShot(millCreated);
    }

    public void PlayChessRemoved()
    {
        speaker.PlayOneShot(chessRemoved);
    }

    public void PlayChessMoved()
    {
        speaker.PlayOneShot(chessMoved);
    }

    public void PlayWinSound()
    {
        speaker.PlayOneShot(win);
    }

    public void PlayLoseSound()
    {
        speaker.PlayOneShot(lose);
    }
}
}
