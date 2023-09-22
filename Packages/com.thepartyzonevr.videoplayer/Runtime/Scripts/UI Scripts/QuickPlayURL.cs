
/*Programmer:   labthe3rd
 * Date:        08/05/2022
 * Description: A script which stores data to be used on the video player
 */


using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Components;
using VRC.SDKBase;


[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class QuickPlayURL : UdonSharpBehaviour
{
    [Header("Title Text For Save Slot")]
    public Text slotTitleText;
    [Space]
    [Header("Title Text UI Input")]
    public InputField titleTextInput;
    [Header("PC VRCURL Input Field")]
    public VRCUrlInputField pCUrlInput;
    [Header("Quest VRCURL Input Field")]
    public VRCUrlInputField questInput;

    [Space]
    [Header("Video Player Script")]
    public VideoPlayerLABEdition videoPlayerScript;


    //Hidden public variables so other script can interact with this script
    //[HideInInspector,UdonSynced] public Text title;
    //[HideInInspector,UdonSynced]public VRCUrl pCURL;
    //[HideInInspector,UdonSynced]public VRCUrl questURL;

    [UdonSynced, FieldChangeCallback(nameof(QuickQuestURL))]
    private VRCUrl _quickQuestURL;

    [UdonSynced, FieldChangeCallback(nameof(QuickPCURL))]
    private VRCUrl _quickPCURL;

    [UdonSynced, FieldChangeCallback(nameof(QuickTitle))]
    private string _quickTitle;

    void Start()
    {

    }


    public void SetQuickPlaySlot()
    {
        if (!Networking.IsOwner(gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }
        if (Networking.IsOwner(gameObject))
        {
            QuickTitle = titleTextInput.text;
            QuickPCURL = pCUrlInput.GetUrl();
            QuickQuestURL = questInput.GetUrl();
            RequestSerialization();
        }
    }

    public void PlayURL()
    {
        Debug.Log(gameObject.name + " was pressed");
        if (Utilities.IsValid(QuickPCURL) && Utilities.IsValid(QuickQuestURL))
        {
            videoPlayerScript.QuickPlay(QuickPCURL, QuickQuestURL);
        }
        else
        {
            Debug.Log("PC Or Quest URL Is Invalid, Cannot Load URL In Video Player");
        }
        
        
    }


    //We will allow ownership to take over for setting the values
    public override bool OnOwnershipRequest(VRCPlayerApi requestingPlayer, VRCPlayerApi requestedOwner)
    {
        return true;
    }


    public VRCUrl QuickQuestURL
    {
        set
        {
            Debug.Log("Setting Quest URL To" + value);
            _quickQuestURL = value;
        }

        get => _quickQuestURL;
    }

    public VRCUrl QuickPCURL
    {
        set
        {
            Debug.Log("Setting PC URL To" + value);
            _quickPCURL = value;
        }

        get => _quickPCURL;
    }

    public string QuickTitle
    {
        set
        {
            Debug.Log("Setting Quick Play Title To" + value);
            _quickTitle = value;
            slotTitleText.text = value;
        }

        get => _quickTitle;
    }

}


