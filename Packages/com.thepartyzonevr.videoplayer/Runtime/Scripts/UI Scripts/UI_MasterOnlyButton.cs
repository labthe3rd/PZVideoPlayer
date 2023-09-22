
/*Programmer:       Labthe3rd
 * Date:            06/09/22
 * Description:     A simple UI script to lock the video player so random people can't change it
 */

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

namespace Labthe3rd.UI.UI_MasterOnlyButton
{
    public class UI_MasterOnlyButton : UdonSharpBehaviour
    {
        public Sprite lockSprite;
        public Sprite unlockSprite;
        public GameObject targetImageGameobject;
        public VideoPlayerLABEdition videoPlayerScript;

        private VRCPlayerApi localPlayer;
        private bool videoPlayerLockState;
        private Image targetImage;

        void Start()
        {
            //Grab button at start
            targetImage = targetImageGameobject.GetComponent<Image>();
            //Delay start the script to prevent any wonkiness from Udon 
            SendCustomEventDelayedSeconds("DelayStart", .25f);
        }

        void OnEnable()
        {
            if(targetImage != null)
            {
                if (Utilities.IsValid(Networking.LocalPlayer))
                {
                    localPlayer = Networking.LocalPlayer;
                }
                videoPlayerLockState = videoPlayerScript.allowGuestControl;
                if (videoPlayerLockState == true)
                {
                    targetImage.sprite = unlockSprite;
                }
                else
                {
                    targetImage.sprite = lockSprite;
                }
            }

        }

        public void DelayStart()
        {
            if (Utilities.IsValid(Networking.LocalPlayer))
            {
                localPlayer = Networking.LocalPlayer;
            }
            videoPlayerLockState = videoPlayerScript.allowGuestControl;
            if(videoPlayerLockState == true)
            {
                targetImage.sprite = unlockSprite;
            }
            else
            {
                targetImage.sprite = lockSprite;
            }
        }

        public void ToggleState()
        {
            videoPlayerLockState = videoPlayerScript.allowGuestControl;
            if(videoPlayerLockState == true)
            {
                
                videoPlayerScript.ToggleMaster();
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ChangeToLockIcon");
            }
            else
            {
                if (Networking.IsOwner(videoPlayerScript.gameObject) || Networking.IsMaster)
                {
                    videoPlayerScript.ToggleMaster();
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ChangeToUnlockIcon");
                }
            }
            
        }
        //We're just going to send network events because this script will correct itself each time it is set active.
        public void ChangeToLockIcon()
        {
            if (targetImage != null)
            {
                targetImage.sprite = lockSprite;
                //targetImage.rectTransform.ForceUpdateRectTransforms();
            }
            else
            {
                targetImage = targetImageGameobject.GetComponent<Image>();
                targetImage.sprite = unlockSprite;
            }

        }

        public void ChangeToUnlockIcon()
        {
            if (targetImage != null)
            {
                targetImage.sprite = unlockSprite;
                //targetImage.rectTransform.ForceUpdateRectTransforms();
            }
            else
            {
                targetImage = targetImageGameobject.GetComponent<Image>();
                targetImage.sprite = unlockSprite;
            }

        }

    }
}

