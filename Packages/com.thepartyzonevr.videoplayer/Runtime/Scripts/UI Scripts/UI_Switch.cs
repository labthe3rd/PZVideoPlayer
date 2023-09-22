
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class UI_Switch : UdonSharpBehaviour
{
    public UdonBehaviour targetScript;
    public string eventString;
    public string startBoolString;

    private bool isOn;
    private bool defaultIsOn;
    private Toggle toggleSwitch;
    private Animator switchAnimator;
    private bool lastState;
    private bool firstCycle; //Do not run the OnEnable until after delay start

    
    void Start()
    {
        toggleSwitch = this.GetComponent<Toggle>();
        switchAnimator = this.GetComponent<Animator>();
        SendCustomEventDelayedSeconds("DelayStart", .25f);


        // AnimateSwitch();
    }

        void OnEnable()
    {
            if (toggleSwitch != null)
            {
                toggleSwitch.isOn = (bool)targetScript.GetProgramVariable(startBoolString);
                Debug.Log(gameObject + " Toggle Set To " + toggleSwitch.isOn);
                AnimateSwitch();

            }
        
    }

    public void DelayStart()
    {
        if (toggleSwitch != null)
        {
            toggleSwitch.isOn = (bool)targetScript.GetProgramVariable(startBoolString);
        }

        Debug.Log("Toggle Set To " + toggleSwitch.isOn);
        SendCustomEventDelayedSeconds("AnimateSwitch", .25f);
    }

    public void AnimateSwitch()
    {
        if (toggleSwitch != null && switchAnimator != null)
        {
            isOn = toggleSwitch.isOn;
            Debug.Log("Animating Toggle");
            if (isOn == false)
            {
                switchAnimator.Play("Switch Off");
                targetScript.SendCustomEvent(eventString);
            }

            else
            {
                switchAnimator.Play("Switch On");
                targetScript.SendCustomEvent(eventString);
            }
        }



    }
}
