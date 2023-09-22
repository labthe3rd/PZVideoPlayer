
/*
 * Programmer:      Labthe3rd
 * Date:            06/12/22
 * Description:     Easy script to disable audio link. If you hide the object you'll hear a grinding noise....
 */

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

namespace Labthe3rd.UI.UI_DisableAudioLink
{
    //This is a local only script
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class UI_DisableAudioLink : UdonSharpBehaviour
    {
        [Header("Place Audio Link Here")]
        public GameObject audioLink;
        [Header("Toggle For Audio Link On/Off")]
        public Toggle audioLinkToggle;
        private AudioSource audioLinkAudioSource;
        [HideInInspector]public bool audioLinkState;
        void Start()
        {
            if (audioLink != null)
            {
                audioLinkAudioSource = audioLink.GetComponentInChildren<AudioSource>();
                audioLinkState = !audioLinkAudioSource.mute;
                Debug.Log("Audio link mute started at " + audioLinkState);
            }
            
        }

        public void ToggleAudioLink()
        {

            

            if(audioLinkAudioSource != null)
            {
                audioLinkState = audioLinkToggle.isOn;
                audioLinkAudioSource.mute = !audioLinkState;
                Debug.Log("Setting audio link mute to " + audioLinkState);
            }
            
        }

    }
}

