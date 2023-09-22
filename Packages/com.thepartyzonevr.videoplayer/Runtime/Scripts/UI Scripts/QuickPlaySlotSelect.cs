
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;


namespace Labthe3rd.VideoPlayer.QuickPlaySlotSelect
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class QuickPlaySlotSelect : UdonSharpBehaviour
    {
        [Header("Text Of Number We We Will Be Changing")]
        public Text slotText;
        [Space]
        [Header("Slots For URLs To Write To")]
        public QuickPlayURL[] quickPlaySlots;


        private int slotIndex = 0;
        private int slotIndexMax;
        private int slotIndexMin = 0;

        void Start()
        {
            //Set slot index max value from size of array
            slotIndexMax = quickPlaySlots.GetUpperBound(0);
            slotIndexMin = quickPlaySlots.GetLowerBound(0);
        }

        public void NextQuickSlot()
        {
            if(quickPlaySlots.Length > 0)
            {
                if(slotIndex < slotIndexMax)
                {
                    slotIndex++;
                }
                else
                {
                    slotIndex = slotIndexMin;
                }
                slotText.text = (slotIndex + 1).ToString();
            }
            else
            {
                Debug.Log("No slots are detected in the script, did you forget to add them?");
            }
        }

        public void PreviousQuickSlot()
        {
            if (quickPlaySlots.Length > 0)
            {
                if (slotIndex > slotIndexMin)
                {
                    slotIndex--;
                }
                else
                {
                    slotIndex = slotIndexMax;
                }
                slotText.text = (slotIndex + 1).ToString();
            }
            else
            {
                Debug.Log("No slots are detected in the script, did you forget to add them?");
            }
        }

        public void SaveQuickSlot()
        {
            if (Utilities.IsValid(quickPlaySlots[slotIndex]))
            {
                quickPlaySlots[slotIndex].SetQuickPlaySlot();
            }
        }

    }
}


