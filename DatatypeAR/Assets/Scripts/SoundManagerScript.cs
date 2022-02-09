using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{
    public static AudioClip correctAnswerSound;
    public static AudioClip incorrectAnswerSound;
    static AudioSource audioSource;

    private void Start()
    {
        correctAnswerSound = Resources.Load<AudioClip>("Sounds/CorrectAnswer");
        incorrectAnswerSound = Resources.Load<AudioClip>("Sounds/IncorrectAnswer");

        audioSource = GetComponent<AudioSource>();
    }

    public static void PlaySound(string clip)
    {
        switch (clip) 
        {
            case "correct":
                audioSource.PlayOneShot(correctAnswerSound);
                break;
            case "incorrect":
                audioSource.PlayOneShot(incorrectAnswerSound);
                break;
        }

    }

}
