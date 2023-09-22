
/*
 * Programmer: LABthe3rd
 * Description: Simple player with live stream mode
 * Date: 11/22/21
 */

using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Video.Components;
using VRC.SDK3.Video.Components.AVPro;
using VRC.SDK3.Components;
using VRC.SDK3.Components.Video;
using VRC.Udon.Common;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class VideoPlayerLABEdition : UdonSharpBehaviour
{
    [Header("User Settings")]
    [Space]
    [Header("Default URLs for PC & Quest")]
    public VRCUrl defaultURL;
    public VRCUrl defaultQuestURL;
    [Space]
    [Header("Video player starts in live stream mode?")]
    [UdonSynced] public bool liveStreamMode;
    [Space]
    [Header("How often do you want the video player to sync to owner when not in live stream mode?")]
    public float syncFrequency;
    [Space]
    [Header("By default would you like the video player to be unlocked?")]
    [UdonSynced]public bool allowGuestControl;
    [Space]
    [Header("Automatically Retry On Error?")]
    public bool automaticReloadOnError = true;
    //Rev 1.31 removes below to prevent overwhelming video services. We want to use an increment instead to help reduce this.
    //public float errorReloadAttemptFrequency = 1;
    [Space]
    [Header("Start with stereo audio sources?")]
    public bool stereoAudioSourcesDefault;
    [Header("Audio Sources For Realistic Mode & Stereo Mode")]
    public AudioSource[] realisticAudioSources;
    public AudioSource[] stereoAudioSources;

    // Public UI Variables Users Do Not Neccessarily Need To Touch
    [Header("Internal And UI Objects")]
    [Space]
    [Header("Target Video Player")]
    public VRCAVProVideoPlayer player;
    [Space]
    [Header("URL Input Fields For PC & Quest Inputs On Video Player")]
    public VRCUrlInputField URLInputField;
    public VRCUrlInputField QuestURLInputField;
    [Header("UI Toggle For Live Stream Mode")]
    public Toggle liveStreamToggle;
    [Header("UI Slider For Volume")]
    public Slider VideoVolume;
    [Header("All UI Debug Text In Menus")]
    public Text[] debugText;
    [Header("UI URL Text For PC & Quest, For Debugging Playback Issues")]
    public Text pCURLText;
    public Text questURLText;
    [Header("UI Owner Sync Status For Debugging Networking Issues")]
    public Text[] syncStatus;

    // new addition per user request to have slider while not in live stream mode. 06/06/22
    [Header("UI Duration Objects")]
    public Slider durationSlider;
    public Image durationImage;
    public Text videoDurationText;
    public Text videoTimeText;



    //Used to determine if player is questie or not
    private bool QuestPlayer = false;

    [UdonSynced, FieldChangeCallback(nameof(timeAndOffset))]
    private Vector2 _timeAndOffset;
    [UdonSynced]
    private VRCUrl uRL;
    [UdonSynced]
    private VRCUrl questURL;

    private VRCUrl currentURL;

    private bool firstLoad = false; //used to keep track of when the current url tag is set so udon don't break

    //We will use these to only use the GET function once of the video player
    private float videoTime = 0f;
    private float videoDuration = 0f;

    //Used for video time math
    private float videoTimeHour = 0;
    private float videoTimeMinute = 0;
    private float videoTimeSecond = 0;
    private string videoTimeHourString = "00";
    private string videoTimeMinuteString = "00";
    private string videoTimeSecondString = "00";


    private float videoDurationHour = 0;
    private float videoDurationMinute = 0;
    private float videoDurationSecond = 0;
    private string videoDurationHourString = "00";
    private string videoDurationMinuteString = "00";
    private string videoDurationSecondString = "00";

    //On 06/11/22 I added this string to keep track of the past error so on reload it will show the error while attempting to replay for better debugs
    private string lastErrorString = "No Errors";

    //Rev 1.31 we will be using an incremental re-sync time to help overload AVPRO
    private float resyncIncrement = 1f;
    //Rev 1.31 adding string to let players know that video player will be retrying
    private string resyncIncrementString;

    //Rev 1.32 It was requested I add a pause video button
    [UdonSynced, FieldChangeCallback(nameof(pauseVideoState))]
    private bool _pauseVideoState;

    public void Start()
    {

        //Automatically detect if we are in Quest OR PC 
#if UNITY_ANDROID
            QuestPlayer = true;
#endif


        Debug.Log("Video player made by labthe3rd, come find me at The Party Zone if you're interested in my scripts");
        //We'll set the default audio to the stereo's level
        if(stereoAudioSourcesDefault == true)
        {

            if (stereoAudioSources.Length > 0)
            {
                VideoVolume.value = stereoAudioSources[0].volume;
                ToggleStereoAudioSources();
                
            }
            else if (realisticAudioSources.Length > 0)
            {
                VideoVolume.value = realisticAudioSources[0].volume;
                ToggleRealisticAudioSources();
            }
            else
            {
                VideoVolume.value = 0;
            }
        }
        else
        {
            if (realisticAudioSources.Length > 0)
            {
                VideoVolume.value = realisticAudioSources[0].volume;
                ToggleRealisticAudioSources();

            }
            else if (stereoAudioSources.Length > 0)
            {
                VideoVolume.value = stereoAudioSources[0].volume;
                ToggleStereoAudioSources();
            }
            else
            {
                VideoVolume.value = 0;
            }
        }

        


        if (Networking.IsOwner(this.gameObject))
        {

            Debug.Log("Network ready, setting default video URLs");
            questURL = defaultQuestURL;
            uRL = defaultURL;
            pCURLText.text = defaultURL.ToString();
            questURLText.text = defaultQuestURL.ToString();

            if (QuestPlayer == true)
            {
                player.PlayURL(questURL);
                LogMessage("Resolving Quest URL: " + questURL,lastErrorString);
                currentURL = questURL;
            }
            else
            {
                player.PlayURL(uRL);
                LogMessage("Resolving PC URL: " + uRL,lastErrorString);
                currentURL = uRL;
            }

            if (liveStreamMode == false)
            {
                Debug.Log("Showing video player duration slider");
                durationSlider.gameObject.SetActive(true);
            }
            else
            {
                Debug.Log("Showing Hiding player duration slider");
                durationSlider.gameObject.SetActive(false);
            }

            SendCustomEventDelayedSeconds("OwnerSerialize", 3);


        }


    }

    /*
    * From VRChat Documentation
    * Fired when the video player on this object is finished playing, either via the end of the video or via player interaction.
    */
    public override void OnVideoEnd()
    {
        Debug.Log("Video ended");

    }

    /*
     * From VRChat Documentation
     * Fired when the video player on this object starts playback, either via the start of a new video in a queue, unpausing, or via player interaction.
     */
    public override void OnVideoPlay()
    {

        //Rev 1.31 Reset increment resync time back to 1 second
        resyncIncrement = 1f;

        if (liveStreamMode == false)
        {
            
            Debug.Log("Showing video player duration slider");
            durationSlider.gameObject.SetActive(true);
            videoTime = 0f;
            

        }
        else
        {
            Debug.Log("Showing Hiding player duration slider");
            durationSlider.gameObject.SetActive(false);
        }


        if (QuestPlayer == true) //check if quest player
        {

            if (currentURL != questURL)
            {
                if (!string.IsNullOrEmpty(questURL.ToString())) //make sure both strings have characters
                {
                    currentURL = questURL;
                    lastErrorString = "Quest URL Sync Failed, Try Resyncing";
                    LogMessage("Not Playing",lastErrorString);
                    player.PlayURL(currentURL);
                }
                else
                {
                    lastErrorString = "Cannot Load A Blank Quest URL";
                    LogMessage("Quest URL is blank",lastErrorString);
                }

            }
            else
            {

                LogMessage("Loading Video With Quest URL",lastErrorString);
            }
        }


        else //PC user
        {

            if (currentURL != uRL)
            {
                if (!string.IsNullOrEmpty(uRL.ToString()))
                {
                    currentURL = uRL;
                    //make sure both strings have characters
                    lastErrorString = "PC URL Sync Failed, Try Resyncing";
                    LogMessage("Not Playing",lastErrorString);
                    player.PlayURL(currentURL);
                }
                else
                {
                    lastErrorString = "Cannot Load A Blank PC URL";
                    LogMessage("PC URL is blank",lastErrorString);
                }
            }
            else
            {

                LogMessage("Loading Video With PC URL",lastErrorString);
            }
        }



    }





    /*
    * From VRChat Documentation
    * Fired when the video player loads a new video.
    */
    public override void OnVideoReady()
    {

        if (QuestPlayer)
        {
            LogMessage("Quest URL Resolved, Attempting To Play: " + questURL, lastErrorString);
        }

        else
        {

            LogMessage("PC URL Resolved, Attempting To Play: " + uRL, lastErrorString);
        }


    }

    /*
    * From VRChat Documentation
    * Fired when the video player on this object starts playback from a stopped state.
    */
    public override void OnVideoStart()
    {
        //Rev 1.31 Reset increment resync time back to 1 second
        resyncIncrement = 1f;
        lastErrorString = "No Errors";
        Debug.Log("On Video Start Triggered");
        if (QuestPlayer == false) //check if quest player
        {

            LogMessage("Video playing with PC URL: " + uRL,lastErrorString);
        }


        else //PC user
        {
            LogMessage("Video playing with Quest URL: " + questURL,lastErrorString);
        }

        if(liveStreamMode == false)
        {
            //possibly unneccessary to set game object as true here.
            durationSlider.gameObject.SetActive(true);
            videoTime = 0f;

                UpdateVideoDuration();
                UpdateTimeAndOffset();

        }

        //v1.34 when video starts check to see if the pause state is true to stop video
        if (pauseVideoState)
        {
            player.Pause();
        }



    }

    public void UpdateVideoDuration()
    {
        videoDuration = player.GetDuration();
        durationSlider.maxValue = videoDuration;
        videoDurationHour = Mathf.Floor(videoDuration / 3600f);
        videoDurationMinute = Mathf.Floor(((videoDuration/3600f) - videoDurationHour)*60f);
        videoDurationSecond = Mathf.Floor(((((videoDuration / 3600f) - videoDurationHour)*60f) - videoDurationMinute)*60);

        if(videoDurationHour > 9)
        {
            videoDurationHourString = videoDurationHour.ToString();
        }
        else
        {
            videoDurationHourString = "0" + videoDurationHour.ToString();
        }
        if (videoDurationMinute > 9)
        {
            videoDurationMinuteString = videoDurationMinute.ToString();
        }
        else
        {
            videoDurationMinuteString = "0" + videoDurationMinute.ToString();
        }
        if (videoDurationSecond > 9)
        {
            videoDurationSecondString = videoDurationSecond.ToString();
        }
        else
        {
            videoDurationSecondString = "0" + videoDurationSecond.ToString();
        }


        videoDurationText.text = videoDurationHourString + ":" + videoDurationMinuteString + ":" + videoDurationSecondString;
            
            
        
        
        UpdateVideoTimeText();
    }

    public void UpdateVideoTimeText()
    {
        if(liveStreamMode == false)
        {
            //Debug.Log("Updating Video Player Time");
            if (Utilities.IsValid(player))
            {
                if (player.IsPlaying)
                {
                    videoTime = player.GetTime();
                    //durationSlider.value = videoTime;
                    durationImage.fillAmount = videoTime/videoDuration;
                    videoTimeHour = Mathf.Floor(videoTime / 3600f);
                    videoTimeMinute = Mathf.Floor(((videoTime / 3600f) - videoTimeHour) * 60f);
                    videoTimeSecond = Mathf.Floor(((((videoTime / 3600f) - videoTimeHour) * 60f) - videoTimeMinute) * 60);

                    if (videoTimeHour > 9)
                    {
                        videoTimeHourString = videoTimeHour.ToString();
                    }
                    else
                    {
                        videoTimeHourString = "0" + videoTimeHour.ToString();
                    }
                    if (videoTimeMinute > 9)
                    {
                        videoTimeMinuteString = videoTimeMinute.ToString();
                    }
                    else
                    {
                        videoTimeMinuteString = "0" + videoTimeMinute.ToString();
                    }
                    if (videoTimeSecond > 9)
                    {
                        videoTimeSecondString = videoTimeSecond.ToString();
                    }
                    else
                    {
                        videoTimeSecondString = "0" + videoTimeSecond.ToString();
                    }


                    videoTimeText.text = videoTimeHourString + ":" + videoTimeMinuteString + ":" + videoTimeSecondString;

                    SendCustomEventDelayedSeconds("UpdateVideoTimeText", 1f);
                }
                else
                {
                    videoTimeText.text = "Not Playing";
                    durationImage.fillAmount = 0;
                    durationSlider.value = 0;
                }

            }

        }

    }

    public void PlayUpdatedVideoTime()
    {
        if (Networking.IsOwner(this.gameObject))
        {

                player.SetTime(durationSlider.value);

        }
        // 6/10/22 added ability for others to change slider if allowguest is active
        else
        {
            if (allowGuestControl)
            {
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
                if (Networking.IsOwner(gameObject))
                {
                    player.SetTime(durationSlider.value);
                }

            }
        }
    }

    public void UpdateTimeAndOffset()
    {

            Debug.Log("Update Time And Offset");
            if (liveStreamMode == false)
            {
                if (Utilities.IsValid(Utilities.IsValid(player)))
                {
                    if (player.IsPlaying)
                    {
                        if (Networking.IsOwner(Networking.LocalPlayer, this.gameObject) == true)
                        {
                            Debug.Log("Video player time is " + player.GetTime());
                            timeAndOffset = new Vector2(player.GetTime(), (float)Networking.GetServerTimeInSeconds());
                            Debug.Log("Time And Offset Set " + timeAndOffset);
                            RequestSerialization();
                            if (syncFrequency >= 0)
                            {
                                SendCustomEventDelayedSeconds("UpdateTimeAndOffset", syncFrequency);
                                Debug.Log("restarting update time and offset");

                            }
                        }
                        else
                        {
                                Resync();

                        }
                    }
                    else
                    {
                        Debug.Log("Video player is not playing, do not keep trying to sync");
                    }

                }
                else
                {
                    Debug.Log("Video player detected as invalid, do not keep trying to sync");
                }


            }
            else
            {
                Debug.Log("Video Player in live stream mode, do not keep syncing time");
            }



        
    }

    public void Resync()
    {
        if (Utilities.IsValid(player) == true)
        {
            if (player.IsPlaying)
            {
                Debug.Log("Resyncing video player");

                player.SetTime(timeAndOffset.x + ((float)Networking.GetServerTimeInSeconds() - timeAndOffset.y));
                UpdateVideoDuration();
                //videoDuration = player.GetDuration();
                //videoTime = timeAndOffset.x;
                //durationSlider.maxValue = videoDuration;
                //durationSlider.value = videoTime;
                //durationImage.fillAmount = videoTime / videoDuration;
                //videoDurationText.text = videoDuration.ToString();
                //videoTimeText.text = videoTime.ToString();
            }

        }
    }

    public void Restart()
    {
        if (Utilities.IsValid(player) == true)
        {
            player.Stop();
            LogMessage("",lastErrorString);
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "OwnerSerialize");
            SendCustomEventDelayedSeconds("Reload", 3);

        }
    }


    public void SetVolume()
    {
        Debug.Log("Setting Volume");
        float volumeSet = VideoVolume.value;
        if (realisticAudioSources.Length > 0)
        {
            for (int i = 0; i < realisticAudioSources.Length; i++)
            {
                realisticAudioSources[i].volume = volumeSet;
            }
        }
        if(stereoAudioSources.Length > 0)
        {
            for (int i = 0; i < stereoAudioSources.Length; i++)
            {
                stereoAudioSources[i].volume = volumeSet;
            }
        }

    }

    public void ReloadButton()
    {
        Debug.Log("Reloading video");
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Reload");
    }

    public void Reload()
    {
        LogMessage("",lastErrorString);
        Debug.Log("Realoading Video");
        if (!string.IsNullOrEmpty(currentURL.ToString()))
        {

            LogMessage("Attempting to replay " + currentURL,lastErrorString);
            player.PlayURL(currentURL);
        }
        else
        {
            LogMessage("No URL Loaded",lastErrorString);
            player.Stop();
        }

    }

    public void ToggleLivestream()
    {

        if (liveStreamMode != liveStreamToggle.isOn)
        {
            if (!Networking.IsOwner(gameObject))
            {
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
            }

            if (Networking.IsOwner(gameObject))
            {


                liveStreamMode = liveStreamToggle.isOn;
                Debug.Log("Live Stream Mode Toggled To " + liveStreamMode);
                if(liveStreamMode == true)
                {
                    Debug.Log("Live Stream Mode True, Hiding Duration Bar");
                    durationSlider.gameObject.SetActive(false);
                }

                RequestSerialization();
                if (Utilities.IsValid(player))
                {
                    if (liveStreamMode == false && player.IsPlaying == true)
                    {
                        Debug.Log("Updating time and offset");
                        durationSlider.gameObject.SetActive(true);
                        UpdateTimeAndOffset();
                        UpdateVideoDuration();
                        
                    }
                }
                else
                {
                    lastErrorString = "CRITICAL! Video Player is invalid";
                    LogMessage("CRITICAL ERROR",lastErrorString);
                }




            }
        }



    }

    public void Play()
    {
        if (Networking.IsOwner(this.gameObject) == false)
        {
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
        }

        if (Networking.IsOwner(this.gameObject) == true)
        {

            uRL = URLInputField.GetUrl();
            questURL = QuestURLInputField.GetUrl();
            pCURLText.text = uRL.ToString();
            questURLText.text = questURL.ToString();


            if (QuestPlayer == true)
            {
                player.PlayURL(questURL);
                LogMessage("Resolving Quest URL: " + questURL,lastErrorString);
                currentURL = questURL;
            }
            else
            {
                player.PlayURL(uRL);
                LogMessage("Resolving PC URL: " + uRL,lastErrorString);
                currentURL = uRL;
            }

            if(!Networking.IsClogged && Networking.IsNetworkSettled)
            {
                //Add delay to prevent bug in local debug and possibly live
                RequestSerialization();
            }
            else
            {
                Debug.Log("Network clogged retrying in 2 seconds");
                SendCustomEventDelayedSeconds("Play", 2);
                
            }
            

        }
    }

    public void QuickPlay(VRCUrl clubPCURL, VRCUrl clubQuestURL)
    {
        
        Debug.Log("allowGuestControl Set To " + allowGuestControl);
        if (Networking.IsOwner(this.gameObject) == false)
        {
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            
        }

        if (Networking.IsOwner(this.gameObject) == true)
        {
            LogMessage("Quick Play Pressed",lastErrorString);
            uRL = clubPCURL;
            questURL = clubQuestURL;
            pCURLText.text = uRL.ToString();
            questURLText.text = questURL.ToString();


            if (QuestPlayer == true)
            {
                player.PlayURL(questURL);
                LogMessage("Resolving Quest URL: " + questURL,lastErrorString);
                currentURL = questURL;
            }
            else
            {
                player.PlayURL(uRL);
                LogMessage("Resolving PC URL: " + uRL,lastErrorString);
                currentURL = uRL;
            }

            RequestSerialization();

        }
    }


    //Handle video errors here
    public override void OnVideoError(VideoError videoError)
    {
        if (automaticReloadOnError)
        {
            resyncIncrementString = "Video Failed, Retrying In " + resyncIncrement + " seconds";
        }

        switch (videoError)
        {
            case VideoError.InvalidURL:
                lastErrorString = "URL is Invalid, verify URL is correct then resync";
                LogMessage(resyncIncrementString,lastErrorString);
                
                break;
            case VideoError.AccessDenied:
                lastErrorString = "Access Denied, verify untrusted URLs is set and stream is accessible in VRChat then resync";
                LogMessage(resyncIncrementString, lastErrorString);
                break;

            case VideoError.PlayerError:
                lastErrorString = "Player Error, verify stream is online then resync";
                LogMessage(resyncIncrementString, lastErrorString);
                break;

            case VideoError.RateLimited:
                lastErrorString = "Video is rate limited, verify connection speed and stream settings then resync";
                LogMessage(resyncIncrementString, lastErrorString);
                break;

            case VideoError.Unknown:
                lastErrorString = "Unknown Error";
                LogMessage(resyncIncrementString, lastErrorString);
                break;

            default:
                lastErrorString = "Invalid error code from AVPro, this is a script or world error";
                LogMessage(resyncIncrementString, lastErrorString);
                break;
        }
        //On 6/11/22 I added the ability to reload on error
        if (automaticReloadOnError)
        {
            ErrorRestart();
        }
        

    }
    //Try to handle video errors, retrying every 3 seconds
    public void ErrorRestart()
    {
        if (Utilities.IsValid(player) == true)
        {
            player.Stop();
            //Removed to not overload network, may add back if tested well
            //SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "OwnerSerialize");
            SendCustomEventDelayedSeconds("ErrorReload", resyncIncrement);
            //Rev 1.31 add 1 to resync increment until the value of 5 is reached so we don't go beyond 5 seconds
            if(resyncIncrement < 5)
            {
                resyncIncrement += 1f;
            }

        }
    }

    //On 06/11/22 Added Error Reload to use the same logic as Reload but use the last error in the message it displays
    public void ErrorReload()
    {
        LogMessage("Retrying",lastErrorString);
        Debug.Log("Realoading Video");
        if (!string.IsNullOrEmpty(currentURL.ToString()))
        {

            LogMessage("Attempting to replay " + currentURL,lastErrorString);
            player.PlayURL(currentURL);
        }
        else
        {
            lastErrorString = "Cannot resolve blank URL, add URL for platform OR have owner resync";
            LogMessage("No URL Loaded",lastErrorString);
            player.Stop();
        }

    }

    public void ToggleMaster()
    {
        if(allowGuestControl == false)
        {
            if (Networking.IsOwner(gameObject) || Networking.LocalPlayer.isMaster)
            {
                if (!Networking.IsOwner(gameObject))
                {
                    Networking.SetOwner(Networking.LocalPlayer, gameObject);
                }
                if (Networking.IsOwner(gameObject))
                {

                    allowGuestControl = true;
                    Debug.Log("Setting allowGuestControl To " + allowGuestControl);
                    RequestSerialization();
                }
                
            }
            else
            {
                Debug.Log("Video Player Locked");
            }
        }
        else
        {
            if (!Networking.IsOwner(gameObject))
            {
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
            }

            if (Networking.IsOwner(gameObject))
            {
                allowGuestControl = false;
                Debug.Log("Setting allowGuestControl To " + allowGuestControl);
                RequestSerialization();
            }
            
        }
    }

    //Added 06/10/22 to allow to switch between Stereo And Realistic Audio Types
    public void ToggleRealisticAudioSources()
    {
        for (int i = 0; i < stereoAudioSources.Length; i++)
        {
            if (stereoAudioSources[i] != null)
            {
                stereoAudioSources[i].mute = true;
            }
        }
        for (int i = 0; i < realisticAudioSources.Length; i++)
        {
            if (realisticAudioSources[i] != null)
            {

                realisticAudioSources[i].mute = false;
            }
        }
    }

    public void ToggleStereoAudioSources()
    {
        for (int i = 0; i < realisticAudioSources.Length; i++)
        {
            if (realisticAudioSources[i] != null)
            {
                realisticAudioSources[i].mute = true;
            }
        }
        for (int i = 0; i < stereoAudioSources.Length; i++)
        {
            if (stereoAudioSources[i] != null)
            {
                stereoAudioSources[i].mute = false;
            }
        }
    }

    public void PauseVideo()
    {


        if (Utilities.IsValid(player))
        {
            if (!Networking.IsOwner(gameObject))
            {
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
            }
            if (Networking.IsOwner(gameObject))
            {
                if(pauseVideoState == false)
                {

                        if (player.IsPlaying)
                        {
                            player.Pause();
                            pauseVideoState = true;
                            RequestSerialization();
                        }
                    

                }
                else
                {
                    if (!player.IsPlaying)
                    {
                        player.Play();

                        pauseVideoState = false;
                        RequestSerialization();
                        
                    }
                }

                
            }
        }
        else
        {
            Debug.Log("Video Player Detected As Invalid");
        }
        
    }

    public void OwnerSerialize()
    {
        if (Networking.IsOwner(this.gameObject))
        {

            RequestSerialization();
            
        }
    }

    public override void OnPreSerialization()
    {
        LogStatus("Sending Data To Players...");
    }



    public override void OnDeserialization()
    {
        if (Utilities.IsValid(player))
        {
            if (QuestPlayer == true)
            {
                if (firstLoad == true)
                {
                    if (currentURL.ToString() != questURL.ToString())
                    {
                        Debug.Log("Different URL detected");
                        player.PlayURL(questURL);
                        LogMessage("Resolving Quest URL: " + questURL,lastErrorString);
                        currentURL = questURL;
                    }

                    else
                    {
                        Debug.Log("Current URL = PC URL");
                    }
                }
                else
                {
                    Debug.Log("Current URL is null");
                    player.PlayURL(questURL);
                    LogMessage("Resolving Quest URL: " + questURL,lastErrorString);
                    currentURL = questURL;
                    firstLoad = true;
                }


            }
            else
            {
                if (firstLoad == true)
                {
                    if (uRL.ToString() != currentURL.ToString())
                    {
                        Debug.Log("Different URL detected");
                        player.PlayURL(uRL);
                        LogMessage("Resolving PC URL: " + uRL,lastErrorString);
                        currentURL = uRL;
                    }
                    else
                    {
                        Debug.Log("Current URL = PC URL");
                    }
                }
                else
                {
                    Debug.Log("Current URL is null");
                    player.PlayURL(uRL);
                    LogMessage("Resolving PC URL: " + uRL,lastErrorString);
                    currentURL = uRL;
                    firstLoad = true;
                }


            }
            pCURLText.text = uRL.ToString();
            questURLText.text = questURL.ToString();
            liveStreamToggle.isOn = liveStreamMode;

            if (liveStreamMode == true)
            {
                Debug.Log("Live Stream Mode True, Hiding Duration Bar");
                durationSlider.gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("Live Stream Mode True, Showing Duration Bar");
                durationSlider.gameObject.SetActive(true);
                
            }
            Debug.Log("Allow Guest Control Changed To " + allowGuestControl);


        }
        else
        {
            LogMessage("NOT PLAYING","Video player is invalid");
        }

    }

    public override void OnPostSerialization(SerializationResult result)
    {
        if (result.success)
        {
            Debug.Log("Serialization was a success! URL updated for all players");
            LogStatus("Serialization Success! Bytes sent: " + result.byteCount);
        }
        else
        {
            LogStatus("Serialization Failed: Not all players received update Bytes Sent: " + result.byteCount);
        }
    }




    public void LogMessage(string message, string errorMessage)
    {
        Debug.Log(message);
        for (int i = 0; i < debugText.Length; i++)
        {
            debugText[i].text = message + "\nERROR MSG: " + errorMessage + "\n" + "\nErrors will clear when video player starts a video";
        }
    }

    public void LogStatus(string message)
    {
        Debug.Log(message);
        for (int i = 0; i < syncStatus.Length; i++)
        {
            syncStatus[i].text = message;
        }
    }


    public Vector2 timeAndOffset
    {
        set
        {
            Debug.Log("Setting new time and offset");
            _timeAndOffset = value;
            if (Networking.IsOwner(this.gameObject) == false)
            {
                Resync();
            }

        }
        get => _timeAndOffset;
    }

    //v1.34 Pause Video Functionality
    public bool pauseVideoState
    {
        set
        {
            Debug.Log("Checking Pause Video State");
            _pauseVideoState = value;
            if (_pauseVideoState)
            {
                if (player.IsPlaying)
                {
                    player.Pause();
                }
            }
            else
            {

                    if (!player.IsPlaying)
                    {
                        player.Play();
                    }
                
            }
        }
        get => _pauseVideoState;
    }

    public override bool OnOwnershipRequest(VRCPlayerApi requestingPlayer, VRCPlayerApi requestedOwner)
    {

        if (requestingPlayer.isMaster)
        {
            Debug.Log("Master is: " + requestingPlayer.displayName +" And is bypassing guest control check");
            return true;
        }
        else
        {
            return allowGuestControl;
        }

        
    }



}


