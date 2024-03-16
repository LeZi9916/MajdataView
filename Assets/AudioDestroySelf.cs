using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDestroySelf : MonoBehaviour
{
    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnEnable()
    {
        audioSource = GetComponent<AudioSource>();
    }
    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying)
        {
            Destroy(audioSource);
            Destroy(gameObject);
        }
    }
}
