using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shotAudio : MonoBehaviour
{
    private AudioSource audio;

    // Start is called before the first frame update
    void Awake()
    {
        audio = GetComponent<AudioSource>();
        audio.pitch = Random.Range(0.8f, 1f);
        audio.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
