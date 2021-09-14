using System;
using UnityEngine;


namespace ProcessControl.Audio
{
    //> CONTAINER FOR ALL IN GAME SOUND EFFECTS
    [Serializable] public class SFX
    {
        public string name; 
        public AudioClip clip;
        [Range(0, 2f)]public float pitch = 1f;
        [Range(0, 2f)]public float volume = 1f;
    }
}