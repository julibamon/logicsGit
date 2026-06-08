using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIsounds : MonoBehaviour
{
    public void Play(string soundName) { 
        SoundEffectManager.Instance.Play(soundName, false);
     } 
}
