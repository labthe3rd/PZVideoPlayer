#if !COMPILER_UDONSHARP && UNITY_EDITOR // These using statements must be wrapped in this check to prevent issues on builds
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;
using UdonSharp;
using UnityEngine.UI;
using UnityEditor;
using UdonSharpEditor;



/// <summary>
/// Example behaviour that has a custom inspector
/// </summary>
[ExecuteInEditMode]
public class UI_BookmarkGenerator : MonoBehaviour
{

    [Header("Bookmark Menu Content Game Object Goes Here")]
    public GameObject _bookMarkMenu;
    [Space]
    [Header("Bookmark Prefab goes here")]
    public GameObject _bookMarkPrefab;
    [Space]
    [Header("New Bookmark Parameters")]
    public string _bookmarkName;
    public Sprite _bookmarkImage;
    public string _PCURL;
    public string _QuestURL;

    //Private Variables
    private GameObject _newBookmark;
    private GameObject scrollView;
    private GameObject content;

    public void CreateBookmark()
    {
        // Instantiate at position (0, 0, 0) and zero rotation.
        _newBookmark = Instantiate(_bookMarkPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        _newBookmark.transform.SetParent(_bookMarkMenu.transform);
        _newBookmark.transform.localScale = new Vector3(1, 1, 1);
        _newBookmark.transform.position = new Vector3(_newBookmark.transform.position.x, _newBookmark.transform.position.y, 0);
        _newBookmark.gameObject.name = _bookmarkName;
        
        _newBookmark.GetComponentInChildren<Image>().sprite = _bookmarkImage;
        _newBookmark.GetComponentInChildren<Text>().text = _bookmarkName;
        
        
        Debug.Log("Bookmark Created");

        //Clear out data now that bookmark was created
        _bookmarkName = null;
        _bookmarkImage = null;
        _PCURL = null;
        _QuestURL = null;
        _newBookmark = null;
    }



}

[CustomEditor(typeof(UI_BookmarkGenerator))]
public class UI_BookmarkGeneratorEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        // Draws the default convert to UdonBehaviour button, program asset field, sync settings, etc.
        //if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;

        UI_BookmarkGenerator inspectorBehaviour = (UI_BookmarkGenerator)target;
        EditorApplication.QueuePlayerLoopUpdate(); //force the editor to update so you can see in real time the changes you make to the objects
        EditorGUI.BeginChangeCheck();



        if (GUILayout.Button("Create Bookmark"))
        {
            inspectorBehaviour.CreateBookmark();
        }



    }
}
#endif


