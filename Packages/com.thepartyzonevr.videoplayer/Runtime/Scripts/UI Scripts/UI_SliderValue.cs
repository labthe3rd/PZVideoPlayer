
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

public class UI_SliderValue : UdonSharpBehaviour
{
    public Text targetText;

    private Slider targetSlider;
    private float sliderValue;
    private float sliderValueScaled;
    void Start()
    {
        targetSlider = (Slider)this.gameObject.GetComponent<Slider>();
    }

    public void _ValueChanged()
    {
        sliderValue = targetSlider.value;
        sliderValueScaled = (Mathf.Round(sliderValue*10000)/100) / targetSlider.maxValue; //This equation gives me two decimal spots

        targetText.text = sliderValueScaled.ToString() + "%";
        
    }

}
