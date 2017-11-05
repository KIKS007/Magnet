using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ScrollbarValue : MonoBehaviour
{
    public bool percentage = false;
    public bool addPercentageSign = false;

    private Text text;
    private Scrollbar scrollbar;
    private Slider slider;

    // Use this for initialization
    void Start()
    {
        text = GetComponent<Text>();

        if (transform.GetComponentInParent <Scrollbar>())
        {
            scrollbar = transform.GetComponentInParent <Scrollbar>();
            scrollbar.onValueChanged.AddListener((float arg0) => ValueChange(arg0));
            text.text = scrollbar.value.ToString();

            ValueChange(scrollbar.value);
        }
        else if (transform.GetComponentInParent <Slider>())
        {
            slider = transform.GetComponentInParent <Slider>();
            slider.onValueChanged.AddListener((float arg0) => ValueChange(arg0));
            text.text = slider.value.ToString();

            ValueChange(slider.value);
        }
    }

    void ValueChange(float arg)
    {
        int count = BitConverter.GetBytes(decimal.GetBits((decimal)arg)[3])[2];

        if (percentage)
            arg *= 100;

        if (count == 0)
            text.text = arg.ToString();
        else
        {
            if (!percentage)
                text.text = arg.ToString("F");
            else
                text.text = arg.ToString("N0");
        }

        if (addPercentageSign)
            text.text += "%";
    }

}
