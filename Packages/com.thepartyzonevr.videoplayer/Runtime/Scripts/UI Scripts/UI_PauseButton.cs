
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

namespace Labthe3rd.UI.UI_PauseButton
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class UI_PauseButton : UdonSharpBehaviour
    {
        public Color enabledColor;
        public GameObject targetImageGameobject;
        public VideoPlayerLABEdition videoPlayerScript;

        private VRCPlayerApi localPlayer;
        private bool videoPlayerPauseState;
        private Image targetImage;
        private Color defaultColor;

        void Start()
        {
            //Grab button at start
            targetImage = targetImageGameobject.GetComponent<Image>();
            defaultColor = targetImage.color;
            //Delay start the script to prevent any wonkiness from Udon 
            SendCustomEventDelayedSeconds("DelayStart", .25f);
        }

        void OnEnable()
        {
            if (targetImage != null)
            {
                if (Utilities.IsValid(Networking.LocalPlayer))
                {
                    localPlayer = Networking.LocalPlayer;
                }
                videoPlayerPauseState = videoPlayerScript.pauseVideoState;
                if (videoPlayerPauseState == true)
                {
                    targetImage.color = enabledColor;
                }
                else
                {
                    targetImage.color = defaultColor;
                }
            }

        }

        public void DelayStart()
        {
            if (Utilities.IsValid(Networking.LocalPlayer))
            {
                localPlayer = Networking.LocalPlayer;
            }
            videoPlayerPauseState = videoPlayerScript.pauseVideoState;
            if (videoPlayerPauseState == true)
            {
                targetImage.color = enabledColor;
            }
            else
            {
                targetImage.color = defaultColor;
            }
        }

        public void ToggleState()
        {
            videoPlayerPauseState = videoPlayerScript.pauseVideoState;
            Debug.Log("Pause Button Pressed Video Pause State Is " + videoPlayerPauseState);
            if (videoPlayerPauseState == false)
            {
                videoPlayerScript.PauseVideo();
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ChangeToEnableColor");
            }
            else
            {
                    videoPlayerScript.PauseVideo();
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ChangeToDefaultColor");
            }

        }
        //We're just going to send network events because this script will correct itself each time it is set active.
        public void ChangeToDefaultColor()
        {
            if (targetImage != null)
            {
                targetImage.color = defaultColor;
                //targetImage.rectTransform.ForceUpdateRectTransforms();
            }
            else
            {
                targetImage = targetImageGameobject.GetComponent<Image>();
                targetImage.color = defaultColor;
            }

        }

        public void ChangeToEnableColor()
        {

                targetImage.color = enabledColor;
        }
    }
}

