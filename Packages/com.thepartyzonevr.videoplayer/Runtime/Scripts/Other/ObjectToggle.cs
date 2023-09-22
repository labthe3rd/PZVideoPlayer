
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;


namespace Labthe3rd.VideoPlayer.ObjectToggle
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)] //This is a local toggle script
    public class ObjectToggle : UdonSharpBehaviour
    {
        [Header("Put target item to toggle here")]
        public GameObject[] targetObject;

        [Header("Put toggle that controls object state here")]
        public Toggle controlToggle;

        private bool _toggleState;
        void Start()
        {
            if (targetObject.Length > 0)
            {
                _toggleState = targetObject[0].activeSelf; //set default state
            }
            
        }

        public void Toggle()
        {
            for(int i = 0; i < targetObject.Length; i++)
            {
                targetObject[i].SetActive(controlToggle.isOn); //Bada Bing Bada Boom object is toggled
            }
            
        }
    }
}

