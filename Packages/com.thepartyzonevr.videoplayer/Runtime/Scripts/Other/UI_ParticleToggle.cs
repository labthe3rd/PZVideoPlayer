
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

namespace Labthe3rd.UI.ConfettiToggle
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)] //This is a local toggle script
    public class UI_ParticleToggle : UdonSharpBehaviour
    {
        [Header("Put parent particle roots here")]
        public GameObject targetObject;
        public GameObject targetObject2;

        [Header("Put toggle that controls object state here")]
        public Toggle controlToggle;

        private ParticleSystem[] childrenParticles;
        private ParticleSystem[] childrenParticles2;
        private bool _toggleState;
        void Start()
        {
            

                childrenParticles = targetObject.GetComponentsInChildren<ParticleSystem>();
            childrenParticles2 = targetObject2.GetComponentsInChildren<ParticleSystem>();
            Debug.Log("retrieve particle systems successfully");

            _toggleState = childrenParticles[0].gameObject.activeSelf;

        }

        public void Toggle()
        {
            for (int i = 0; i < childrenParticles.Length; i++)
            {

                    childrenParticles[i].gameObject.SetActive(controlToggle.isOn);

                
            }
            for (int i = 0; i < childrenParticles2.Length; i++)
            {

                childrenParticles2[i].gameObject.SetActive(controlToggle.isOn);


            }

        }
    }
}

