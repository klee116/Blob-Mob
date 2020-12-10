using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playBomb : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioSource audioSource;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void playSound()
    {
        audioSource.Play();
        //play sound function
    }
    
}
