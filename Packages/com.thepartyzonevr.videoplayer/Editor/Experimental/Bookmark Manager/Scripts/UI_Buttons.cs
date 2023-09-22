#if !COMPILER_UDONSHARP && UNITY_EDITOR // These using statements must be wrapped in this check to prevent issues on builds
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;
using UdonSharp;
using UnityEngine.UI;
using UnityEditor;
using UdonSharpEditor;
using TMPro;



/// <summary>
/// Example behaviour that has a custom inspector
/// </summary>
[ExecuteInEditMode]
public class UI_Buttons : MonoBehaviour
{

    
    public string buttonText;
    public int buttonFontSize;


    //Private Variables
    private Text[] targetText;
    private TMP_Text[] targetTMPText;
    


    public void RetrieveText()
    {
        targetText = this.gameObject.GetComponentsInChildren<Text>();
        targetTMPText = this.gameObject.GetComponentsInChildren<TMP_Text>();
        if(targetText.Length != 0)
        {
            buttonText = targetText[0].text;
            buttonFontSize = (int)targetText[0].fontSize;
        }
        else
        {
            Debug.Log("No text components found");
        }
        if(targetTMPText.Length != 0)
        {
            buttonText = targetTMPText[0].text;
            buttonFontSize = (int)targetTMPText[0].fontSize;
        }
        else
        {
            Debug.Log("No text mesh pro components found");
        }
    }

    public void ChangeText()
    {
        if( targetText.Length == 0 && targetTMPText.Length == 0)
        {
            Debug.Log("There is no text or TMP components found. Is this script attached to a parent object with children text?");
        }

        if(targetText.Length > 0)
        {
            for (int i = 0; i < targetText.Length; i++)
            {
                targetText[i].text = buttonText;
                targetText[i].fontSize = buttonFontSize;
            }
        }

        if(targetTMPText.Length > 0)
        {
            for (int i = 0; i < targetTMPText.Length; i++)
            {
                targetTMPText[i].text = buttonText;
                targetTMPText[i].fontSize = buttonFontSize;
            }
        }

    }



}

[CustomEditor(typeof(UI_Buttons))]
public class UI_ButtonsEditor : Editor
{

    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector(); We do not want to see the public object above since we will be controlling that via the field below

        UI_Buttons inspectorBehaviour = (UI_Buttons)target;
        inspectorBehaviour.RetrieveText(); //Grab all child components and get ready
        EditorApplication.QueuePlayerLoopUpdate(); //force the editor to update so you can see in real time the changes you make to the objects
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.LabelField("Enter Desired Text To Appear On The Button");
        string newButtonText = EditorGUILayout.TextField("Button Text", inspectorBehaviour.buttonText);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Enter Desired Font Size To Appear on Button");
        int newButtonFontSize = EditorGUILayout.IntField("Button Font Size", inspectorBehaviour.buttonFontSize);


        if (EditorGUI.EndChangeCheck())
        {
            inspectorBehaviour.buttonText = newButtonText;
            inspectorBehaviour.buttonFontSize = newButtonFontSize;
            inspectorBehaviour.ChangeText();
        }



    }
}
#endif


