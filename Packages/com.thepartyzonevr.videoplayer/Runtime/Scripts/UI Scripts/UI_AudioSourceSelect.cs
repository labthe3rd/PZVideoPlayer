/*Programmer:       Labthe3rd
 * Date:            06/10/22
 * Description:     A simple UI script to enable/disable audio sources for vidoe player
 */

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
using TMPro;

namespace Labthe3rd.UI.UI_AudioSourceSelect
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class UI_AudioSourceSelect : UdonSharpBehaviour
    {
        [Header("Target Text We Will Be Changing When Button Is Pressed")]
        public TextMeshProUGUI targetText;
        [Space]
        [Header("Target Video Player")]
        public UdonBehaviour videoPlayerScript;

        //Grab local player on start
        private bool videoPlayerRealisticDefault;
        private bool toggleState;


        void Start()
        {
            //Delay start the script to prevent any wonkiness from Udon 
            videoPlayerRealisticDefault = (bool)videoPlayerScript.GetProgramVariable("stereoAudioSourcesDefault");
            toggleState = videoPlayerRealisticDefault;
            if (toggleState == true)
            {
                targetText.text = "STEREO";
            }
            else
            {
                targetText.text = "REALISTIC";
            }
        }

        public void SwitchAudioSource()
        {
            toggleState = !toggleState;
            if(toggleState == true)
            {
                videoPlayerScript.SendCustomEvent("ToggleStereoAudioSources");
                targetText.text = "STEREO";
            }
            else
            {
                videoPlayerScript.SendCustomEvent("ToggleRealisticAudioSources");
                targetText.text = "REALISTIC";
            }
        }




    }
}

