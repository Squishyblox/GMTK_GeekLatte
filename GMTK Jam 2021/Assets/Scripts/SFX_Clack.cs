using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX_Clack : MonoBehaviour
{
    private AudioSource audioSource;
    private void Start() {
        audioSource = GetComponent<AudioSource>();
    }
    private void OnCollisionEnter2D(Collision2D other) 
    {
        audioSource.Play();
    }
}
