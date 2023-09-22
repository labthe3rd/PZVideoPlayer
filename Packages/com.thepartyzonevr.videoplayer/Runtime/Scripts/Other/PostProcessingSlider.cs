
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

namespace Labthe3rd.PostProcessingSlider
{
    public class PostProcessingSlider : UdonSharpBehaviour
    {

        public Slider BloomSlider;
        public Text BloomText;
        public PostProcessVolume BloomVolume;

        void Start()
        {
            BloomSlider.SetValueWithoutNotify(BloomVolume.weight);
            BloomText.text = ((Mathf.Round(BloomSlider.value * 10000) / 100) / BloomSlider.maxValue).ToString(); //This equation gives me two decimal spots
        }

        public void _SetBloom()
        {
            BloomVolume.weight = BloomSlider.value;
        }
    }
}

