using UnityEngine;
using System.Collections;
using Rewired.UI.ControlMapper;
using System.Collections.Generic;
using UnityEngine.UI;

public class ControlMapperForceLayout : MonoBehaviour 
{
	public enum WhichMapperElement {InputGridHeader, ControllerLabel, ControllerNameLabel, AssignedControllerLabel, InputGridLabel};
	public WhichMapperElement whichMapperElement = WhichMapperElement.InputGridHeader;

	public Font stormFaze;

	private Text textComponent;

	private bool inputGridLabel = false;

	private int defaultFontSize = 29;

	void Start ()
	{
		WhichElement ();
	}

	void OnEnable () 
	{
		WhichElement ();
	}


	void WhichElement ()
	{
		switch (whichMapperElement)
		{
		case WhichMapperElement.InputGridHeader:
			InputGridHeader ();
			break;
		case WhichMapperElement.ControllerLabel:
			ControllerLabel ();
			break;
		case WhichMapperElement.ControllerNameLabel:
			ControllerNameLabel ();
			break;
		case WhichMapperElement.AssignedControllerLabel:
			AssignedControllerLabel ();
			break;
		case WhichMapperElement.InputGridLabel:
			InputGridLabel ();
			break;
		}
	}
	
	void InputGridHeader ()
	{
		textComponent = GetComponent <Text> ();

		textComponent.font = stormFaze;
		textComponent.fontStyle = FontStyle.Normal;
		textComponent.resizeTextMaxSize = defaultFontSize;
	}

	void ControllerLabel ()
	{
		textComponent = GetComponent <Text> ();

		textComponent.font = stormFaze;
		textComponent.fontStyle = FontStyle.Normal;
		textComponent.resizeTextMaxSize = defaultFontSize;
		textComponent.fontSize = defaultFontSize;
	}

	void ControllerNameLabel ()
	{
		textComponent = GetComponent <Text> ();

		textComponent.fontStyle = FontStyle.Normal;
		textComponent.fontSize = 20;
		textComponent.resizeTextMaxSize = 20;
	}

	void AssignedControllerLabel ()
	{
		textComponent = GetComponent <Text> ();

		textComponent.font = stormFaze;
		textComponent.fontStyle = FontStyle.Normal;
		textComponent.resizeTextMaxSize = defaultFontSize;
		textComponent.fontSize = defaultFontSize;
	}

	void InputGridLabel ()
	{
		textComponent = GetComponent <Text> ();

		if(textComponent.fontStyle == FontStyle.Bold)
			inputGridLabel = true;

		if(inputGridLabel)		
		{
			textComponent.font = stormFaze;
			textComponent.fontStyle = FontStyle.Normal;
			textComponent.resizeTextMaxSize = 23;
		}
	}


}
