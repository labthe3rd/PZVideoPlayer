
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Labthe3rd.VideoPlayer.MenuButton
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class MenuButton : UdonSharpBehaviour
    {
        public VideoMenuSelect mainScript;
        public int buttonValue;
        public void OpenMenu()
        {
            mainScript.menuSelect = buttonValue;
            mainScript.ChangeMenu();
        }
    }
}

