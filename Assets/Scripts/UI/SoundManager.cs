using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static AudioClip shot, door;
    static AudioSource audioSrc;

    // Start is called before the first frame update
    void Start()
    {
        shot = Resources.Load<AudioClip>("ShotSFX");
        door = Resources.Load<AudioClip>("DoorSFX");

        audioSrc = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void PlaySound(string clip)
    {
        switch (clip)
        {
            case "ShotSFX":
                audioSrc.PlayOneShot(shot);
                break;
            case "DoorSFX":
                audioSrc.PlayOneShot(door);
                break;
        }
    }
}
