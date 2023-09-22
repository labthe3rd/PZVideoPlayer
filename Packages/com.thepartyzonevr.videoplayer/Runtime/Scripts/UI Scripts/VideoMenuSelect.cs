
/*
 * Programmer:      Labthe3rd
 * Description:     A menu selection script
 * Date:            04/16/2022
 * Version:         1.0
 * 
 * Like my prefabs? Find me at The Party Zone!
 * https://discord.gg/y5T5sG7mPr
 */

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;




     [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class VideoMenuSelect : UdonSharpBehaviour
    {
   
        [Header("Place Menus In This Game Object Array. These will be turned on/off by the script")]
        public GameObject[] menuObjects; //Link these to the menus

        //The button scripts will control this value
        [HideInInspector] public int menuSelect = 1;


        public void ChangeMenu()
        {
            //First we hide all the menus to reduce size of script
            for(int i = 0; i < menuObjects.Length; i++)
            {
                if (Utilities.IsValid(menuObjects[i]))
                {
                    menuObjects[i].SetActive(false);
                }
                else
                {
                    Debug.Log("Menu Object " + i + " is invalid. Did you set a gameobject to that slot?");
                }
            
            }

            //Menu buttojn sets the menuSelect int to a value, that value is the array index
            if (Utilities.IsValid(menuObjects[menuSelect]))
            {
            menuObjects[menuSelect].SetActive(true);
            }

            else
            {
            Debug.Log("Invalid menu, did you reference hte wrong int in the button's script?");
            }
            


        }
    }


