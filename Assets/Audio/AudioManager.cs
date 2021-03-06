using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ProcessControl.Tools;
using UnityEditor;


namespace ProcessControl.Audio
{
    
    public class AudioManager : Service
    {
        //- LOCAL VARIABLES
        [SerializeField] private SFX[] soundEffects;
        [SerializeField] private AudioSource[] sources;

        //- EVENT HOOKS
        public static Action<string> PlaySFX;
        public static Action<SFX, int, bool> PlayTrack;
        public static Action<string, int, bool> FindAndPlayTrack;

        private bool cancelPlaylist;
        
        // TODO audio initialization
        public override void Initialize()
        {
            var assets = AssetDatabase.LoadAllAssetsAtPath("Assets/Audio/Media");
            // soundEffects = assets.Cast<SFX>().ToList();
        }

        //> INITIALIZATION
        protected override void Awake()
        {
            base.Awake();
            AudioListener.volume = PlayerPrefs.GetFloat("GlobalVolume", 1f);

            PlaySFX += OnPlaySFX;
            PlayTrack += OnPlayTrack;
            FindAndPlayTrack += OnPlayTrack;

            sources = GetComponents<AudioSource>();

            EditorApplication.playModeStateChanged += (state) =>
            {
                cancelPlaylist = state switch
                {
                    PlayModeStateChange.EnteredPlayMode => false,
                    PlayModeStateChange.ExitingPlayMode => true,
                    _ => true,
                };
            };
        }

        public void Start()
        {
            var musicPlaylist = new List<SFX>
            {
                soundEffects.First(s => s.name == "Lonely_Machines_Intro"),
                soundEffects.First(s => s.name == "Lonely_Machines"),
            };
            
            StartPlaylist(musicPlaylist, 0, loop: true);
        }

        //> PLAY A PLAYLIST OF MUSIC
        private async void StartPlaylist(List<SFX> tracks, int channel, bool shuffle = false, bool loop = false)
        {
            if (tracks.Count <= 0) Debug.LogWarning("Provided <colors=yellow>NO SONGS</color> in Playlist!");
            if (shuffle) tracks = tracks.Shuffle();
            
            foreach (var track in tracks)
            {
                if (cancelPlaylist) return;
                
                OnPlayTrack(track, channel);
                await Alerp.Delay(track.clip.length);
            }
            
            // if (loop) StartPlaylist(tracks, channel, shuffle, loop);
        }

        //> PLAY ONE SHOT SOUND CLIP
        private void OnPlaySFX(string name)
        {
            SFX sfx = soundEffects.FirstOrDefault(s => s.name == name);
            if (sfx is { }) sources[1].PlayOneShot(sfx.clip, sfx.volume);
            else Debug.LogWarning($"Unable to find sfx: <color=yellow>\"{name}\"</color>");
        }

        //> REPLACE STREAM SOUND CLIP
        private void OnPlayTrack(string track, int channel , bool loop = false)
        {
            SFX sfx = soundEffects.FirstOrDefault(s => s.name == track);
            OnPlayTrack(sfx, channel, loop);
        }

        //> PLAY SOUND EFFECT
        private void OnPlayTrack(SFX sfx, int channel, bool loop = false)
        {
            if (sfx is { })
            {
                sources[channel].clip = sfx.clip;
                sources[channel].pitch = sfx.pitch;
                sources[channel].volume = sfx.volume;
                sources[channel].loop = loop;
                sources[channel].Play();
            }
            else Debug.LogWarning($"Unable to find track: <color=yellow>\"{name}\"</color>");
        }

        //> STOP STREAM SOUND CLIP
        private void Stop(int stream) => sources[stream].Stop();

        //> SET THE VOLUME IN PLAYER PREFERENCES
        private static void SetVolume(float volume)
        {
            AudioListener.volume = volume;
            PlayerPrefs.SetFloat("GlobalVolume", volume);
        }

    }
}