
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using UnityEngine.UI;

#if !COMPILER_UDONSHARP && UNITY_EDITOR // These using statements must be wrapped in this check to prevent issues on builds
using UnityEditor;
using UdonSharpEditor;
#endif

[ExecuteInEditMode]
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class UI_ClubURL : UdonSharpBehaviour
{
    public VRCUrl pCURL;
    public VRCUrl questURL;
    public VideoPlayerLABEdition videoPlayerScript;

    
    //Public variables we send to custom editor
    [HideInInspector] public Sprite _bookmarkSprite;
    [HideInInspector] public string _labelString;

    private Image _bookmarkImage;
    private Text _labelText;

    public void PlayURL()
    {
        videoPlayerScript.QuickPlay(pCURL, questURL);
    }


    //Code for the editor, we will not use this in game
    public void GetChildren()
    {
        _bookmarkImage = gameObject.GetComponentInChildren<Image>();
        _bookmarkSprite = _bookmarkImage.sprite;
        _labelText = gameObject.GetComponentInChildren<Text>();
        _labelString = _labelText.text;

    }

    //Change attributes in components
    public void ChangeBookmark()
    {
        _labelText.text = _labelString;
        _bookmarkImage.sprite = _bookmarkSprite;
    }

}


//Custom editor so we change the bookmark attributes from the parent object
#if !COMPILER_UDONSHARP && UNITY_EDITOR // These using statements must be wrapped in this check to prevent issues on builds
[CustomEditor(typeof(UI_ClubURL))]
public class UI_ClubURLEditor : Editor
{
    public Sprite sourceSprite;

    public override void OnInspectorGUI()
    {


        UI_ClubURL inspectorBehaviour = (UI_ClubURL)target;
        inspectorBehaviour.GetChildren(); //Grab all child components and get ready
        sourceSprite = inspectorBehaviour._bookmarkSprite;
        EditorApplication.QueuePlayerLoopUpdate(); //force the editor to update so you can see in real time the changes you make to the objects
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.LabelField("Enter Bookmark Label");
        string newBookmarkLabel = EditorGUILayout.TextField("Bookmark Label", inspectorBehaviour._labelString);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Select Bookmark Image");
        Sprite newBookmarkSprite = (Sprite)EditorGUILayout.ObjectField(sourceSprite, typeof(Sprite), true);
        EditorGUILayout.Space();
        DrawDefaultInspector();
        // Draws the default convert to UdonBehaviour button, program asset field, sync settings, etc.
        if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;

        if (EditorGUI.EndChangeCheck())
        {
            inspectorBehaviour._labelString = newBookmarkLabel;
            inspectorBehaviour._bookmarkSprite = newBookmarkSprite;
            inspectorBehaviour.ChangeBookmark();
        }



    }
}
#endif

