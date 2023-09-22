
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

public class UI_State_Switch : UdonSharpBehaviour
{
    public string state1Name;
    public string state2Name;
    [Header("Game Objects To Toggle Between")]
    public GameObject targetObject1;
    public GameObject targetObject2;
    [Space]
    [Header("Text To Toggle")]
    public Text toggleText;

    [Space]
    [Header("Sync Settings")]
    public bool localOnly;
    public bool ownerOnly;
    [Space]
    [Header("Internal Stuff")]
    public Toggle toggleSwitch;
    public Animator switchAnimator;

    private bool isOn;
    private bool defaultIsOn;


    [UdonSynced] private bool targetSync1; //also used to sync switch
    [UdonSynced] private bool targetSync2;


    void Start()
    {
        //toggleSwitch = this.GetComponent<Toggle>();
        //switchAnimator = this.GetComponent<Animator>();

        if (Networking.IsOwner(this.gameObject))
        {
            targetSync1 = toggleSwitch.isOn;
            targetSync2 = !toggleSwitch.isOn;
            RequestSerialization();
        }

        targetObject1.SetActive(targetSync1);
        targetObject2.SetActive(targetSync2);
        if (targetSync1)
        {
            toggleText.text = state1Name;
        }
        else
        {
            toggleText.text = state2Name;
        }
        Debug.Log("Toggle Set To " + toggleSwitch.isOn);

        // AnimateSwitch();
    }

    private void OnEnable()
    {
        if (toggleSwitch != null)
        {
            AnimateSwitch();
        }


    }

    public void SwitchState()
    {
        if (localOnly)
        {
            targetSync1 = toggleSwitch.isOn;
            targetSync2 = !toggleSwitch.isOn;
            AnimateSwitch();
        }
        else
        {
            if (!Networking.IsOwner(this.gameObject))
            {
                Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
                Debug.Log("Setting ownership of " + this.gameObject);
            }

            if (Networking.IsOwner(this.gameObject))
            {
                Debug.Log("Is now owner of " + this.gameObject);
                targetSync1 = toggleSwitch.isOn;
                targetSync2 = !toggleSwitch.isOn;
                RequestSerialization();

                targetObject1.SetActive(targetSync1);
                targetObject2.SetActive(targetSync2);
                if (targetSync1)
                {
                    toggleText.text = state1Name;
                }
                else
                {
                    toggleText.text = state2Name;
                }
                AnimateSwitch();
            }
            else
            {
                toggleSwitch.isOn = !toggleSwitch.isOn; //undo if ownership transfer didn't happen
            }
        }
    }

    public void AnimateSwitch()
    {
        Debug.Log("Animating Toggle");
        isOn = toggleSwitch.isOn;
        if (isOn == false)
        {
            switchAnimator.Play("Switch Off");
            
        }

        else
        {
            switchAnimator.Play("Switch On");
            
        }
    }

    public override void OnDeserialization()
    {
        Debug.Log("Deserializing " + this.gameObject);
        toggleSwitch.isOn = targetSync1;
        targetObject1.SetActive(targetSync1);
        targetObject2.SetActive(targetSync2);
        if (targetSync1)
        {
            toggleText.text = state1Name;
        }
        else
        {
            toggleText.text = state2Name;
        }
        AnimateSwitch();
    }

    public override bool OnOwnershipRequest(VRCPlayerApi requestingPlayer, VRCPlayerApi requestedOwner)
    {
        Debug.Log("Returning " + !localOnly + " for object named " + this.gameObject);
        return !localOnly;
    }
}
