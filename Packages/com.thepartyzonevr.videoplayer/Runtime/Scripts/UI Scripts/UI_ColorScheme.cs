using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;
using UdonSharp;
using UnityEngine.UI;
using TMPro;

#if !COMPILER_UDONSHARP && UNITY_EDITOR // These using statements must be wrapped in this check to prevent issues on builds
using UnityEditor;
using UdonSharpEditor;
#endif


/// <summary>
/// Example behaviour that has a custom inspector
/// </summary>
[ExecuteInEditMode]
public class UI_ColorScheme : UdonSharpBehaviour
{

    public string stringVal;
    public Color primaryColor;
    public Color secondaryColor;
    public Color backgroundColor;
    public Color textColor;
    private Button[] uIButtons;
    private Image[] uIImage;
    private Text[] uIText;
    //private TextMesh[] uITextMesh;
    private TextMeshProUGUI[] uITextMeshPro;

    /* private void Start()
     {
         //uIButtons = GetComponentsInChildren<Button>();
         //Debug.Log("Received Buttons");
         //Debug.Log(uIButtons);
     }*/


    public void GetColors()
    {
        uIButtons = GetComponentsInChildren<Button>();
        uIImage = GetComponentsInChildren<Image>();
        uITextMeshPro = GetComponentsInChildren<TextMeshProUGUI>();
        uIText = GetComponentsInChildren<Text>();
        for(int i = 0; i < uIImage.Length; i++)
        {

            switch (uIImage[i].gameObject.name)
            {
                case "Panel":
                    backgroundColor = uIImage[i].color;
                    break;

                case "Scroll View":
                    secondaryColor = uIImage[i].color;
                    break;

                default:
                    primaryColor = uIImage[i].color;
                    break;
            }
        }
        textColor = uITextMeshPro[0].color;


    }

    public void ChangeColor()
    {
        for (int i = 0; i < uIImage.Length; i++)
        {
            switch (uIImage[i].gameObject.name)
            {
                case "Panel":
                    uIImage[i].color = new Color(backgroundColor.r,backgroundColor.g,backgroundColor.b,uIImage[i].color.a);
                    break;

                case "Scroll View":
                    uIImage[i].color = new Color(secondaryColor.r,secondaryColor.g,secondaryColor.b,uIImage[i].color.a);
                    break;

                default:
                    if (uIImage[i].gameObject.name.Contains("Logo") == false) //We don't want the logo colors to change
                    {
                        uIImage[i].color = new Color(primaryColor.r, primaryColor.g, primaryColor.b, uIImage[i].color.a);
                    }
                    
                    break;
            }
        }

        for(int i = 0; i < uIText.Length; i++)
        {
            uIText[i].color = new Color(textColor.r, textColor.g, textColor.b, uIText[i].color.a);
        }

        for(int i = 0; i < uITextMeshPro.Length; i++)
        {
            uITextMeshPro[i].color = new Color(textColor.r, textColor.g, textColor.b, uITextMeshPro[i].color.a);
        }
    }
}

// Editor scripts must be wrapped in a UNITY_EDITOR check to prevent issues while uploading worlds. The !COMPILER_UDONSHARP check prevents UdonSharp from throwing errors about unsupported code here.
#if !COMPILER_UDONSHARP && UNITY_EDITOR
[CustomEditor(typeof(UI_ColorScheme))]
public class CustomInspectorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draws the default convert to UdonBehaviour button, program asset field, sync settings, etc.
        if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;

        UI_ColorScheme inspectorBehaviour = (UI_ColorScheme)target;
        inspectorBehaviour.GetColors();
        EditorApplication.QueuePlayerLoopUpdate(); //force the editor to update so you can see in real time the changes you make to the objects
        EditorGUI.BeginChangeCheck();



        Color newBackgroundColor = EditorGUILayout.ColorField("Background Color", inspectorBehaviour.backgroundColor);
        Color newPrimaryColor = EditorGUILayout.ColorField("Primary Color", inspectorBehaviour.primaryColor);
        Color newSecondaryColor = EditorGUILayout.ColorField("Secondary Color", inspectorBehaviour.secondaryColor);
        Color newTextColor = EditorGUILayout.ColorField("Text Color", inspectorBehaviour.textColor);




        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(inspectorBehaviour, "Modify string val");


            inspectorBehaviour.primaryColor = newPrimaryColor;
            inspectorBehaviour.secondaryColor = newSecondaryColor;
            inspectorBehaviour.backgroundColor = newBackgroundColor;
            inspectorBehaviour.textColor = newTextColor;
            inspectorBehaviour.ChangeColor();

            //Debug.Log(newStrVal);
            //Debug.Log("New Color is " + newPrimaryColor.ToString());
        }
    }
}
#endif

