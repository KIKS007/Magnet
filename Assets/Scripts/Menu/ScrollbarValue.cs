using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ScrollbarValue : MonoBehaviour 
{
	private Text text;
	private Scrollbar scrollbar;
	private Slider slider;

	// Use this for initialization
	void Start () 
	{
		text = GetComponent<Text> ();

		if(transform.GetComponentInParent <Scrollbar> ())
		{
			scrollbar = transform.GetComponentInParent <Scrollbar> ();
			scrollbar.onValueChanged.AddListener((float arg0) => ValueChange(arg0));
			text.text = scrollbar.value.ToString ();
		}
		else if(transform.GetComponentInParent <Slider> ())
		{
			slider = transform.GetComponentInParent <Slider> ();
			slider.onValueChanged.AddListener((float arg0) => ValueChange(arg0));
			text.text = slider.value.ToString ();
		}

	}

	void ValueChange (float arg)
	{
		int count = BitConverter.GetBytes(decimal.GetBits((decimal)arg)[3])[2];

		if (count == 0)
			text.text = arg.ToString ();
		else
			text.text = arg.ToString ("F");
	}

}
