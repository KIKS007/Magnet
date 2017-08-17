using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using DG.Tweening;
using Sirenix.OdinInspector;

public class MenuButtonAnimationsAndSounds : MenuShaderElement, IPointerClickHandler, ISelectHandler, IPointerEnterHandler, IPointerExitHandler, IDeselectHandler, ISubmitHandler, IPointerDownHandler, IPointerUpHandler
{
	[Header ("Settings")]
	public bool scaleChange = true;
	public bool colorChange = false;
	public bool vibration = true;
	public bool textColorChange = true;

	[Header ("Debug")]
	public bool selected;

	private Text text;
	private RectTransform buttonRect;
	private EventSystem eventSys;
	private bool mainButton;
	private bool pointerDown = false;

	private float scaleOnSelected = 1.1f;
	private float scaleOnDuration = 0.25f;
	private Button buttonComponent;

	protected override void Awake ()
	{
		base.Awake ();

		eventSys = GameObject.FindGameObjectWithTag ("EventSystem").GetComponent<EventSystem> ();
		buttonRect = GetComponent<RectTransform> ();
		buttonComponent = GetComponent<Button> ();

		EventSystemScript.OnNewSelectedGameObject += (arg) => 
		{
			if(arg != gameObject && selected)
				OnDeselect ();
		};

		if(textColorChange || colorChange)
			text = transform.GetChild (0).GetComponent<Text> ();

		if (colorChange)
			ColorChangeSetup ();
	}

	void ColorChangeSetup ()
	{
		if (tag == "MainButton")
		{
			mainButton = true;
			text.color = GlobalVariables.Instance.mainButtonIdleColorText;
		}
		else
		{
			mainButton = false;
			text.color = GlobalVariables.Instance.secondaryButtonIdleColorText;
		}

		if(buttonComponent.interactable == false)
			text.color = mainButton ? GlobalVariables.Instance.mainButtonHighlightedColorText : GlobalVariables.Instance.secondaryButtonIdleColorText;
	}

	void Update ()
	{
		if (!Application.isPlaying)
			return;


		if (colorChange)
			ColorChangeUpdate ();

		if(scaleChange)
		{
			if (buttonComponent && buttonComponent.interactable == false)
				return;
			
			if(eventSys.currentSelectedGameObject != gameObject && buttonRect.localScale != Vector3.one && !DOTween.IsTweening ("ResetScale" + GetInstanceID ()))
				buttonRect.DOScale(1, scaleOnDuration).SetId ("ResetScale" + GetInstanceID ());
		}
	}

	void ColorChangeUpdate ()
	{
		if(buttonComponent.interactable == true)
		{
			if(!pointerDown)
			{
				if(eventSys.currentSelectedGameObject == gameObject)
					text.color = mainButton ? GlobalVariables.Instance.mainButtonHighlightedColorText : GlobalVariables.Instance.secondaryButtonHighlightedColorText;

				else
					text.color = mainButton ? GlobalVariables.Instance.mainButtonIdleColorText : GlobalVariables.Instance.secondaryButtonIdleColorText;
			}
		}
		else
			text.color = mainButton ? GlobalVariables.Instance.mainButtonIdleColorText : GlobalVariables.Instance.secondaryButtonIdleColorText;
	}

	void ColorChange ()
	{
		if (colorChange)
			text.color = mainButton ? GlobalVariables.Instance.mainButtonClickedColorText : GlobalVariables.Instance.secondaryClickedColorText;
	}

	public override void ShaderColorChange ()
	{
		base.ShaderColorChange ();

		if (!useUIShader)
			return;

		if (!useEnvironementChroma)
			return;

		ShaderHighlight ();

		TextColorChange ();
	}

	void ShaderHighlight ()
	{
		if (!useUIShader)
			return;
		
		if(selected)
			material.SetInt (highlightToggle, 1);
		else
			material.SetInt (highlightToggle, 0);

		TextColorChange ();
	}

	public void ShaderClick (bool duration = false)
	{
		if (!useUIShader)
			return;

		if(!duration)
		{
			if(material.GetInt (clickToggle) == 0)
				material.SetInt (clickToggle, 1);
			else
				material.SetInt (clickToggle, 0);
			
			TextColorChange ();
		}
		else
		{
			material.SetInt (clickToggle, 1);

			TextColorChange ();

			DOVirtual.DelayedCall (clickDuration, ()=> 
				{
					material.SetInt (clickToggle, 0);
					TextColorChange ();
				});
		}
	}

	void TextColorChange ()
	{
		string color = chromas [(int)globalVariables.environementChroma];

		if(material.GetInt (clickToggle) == 1)
			color += this.click;

		else if(material.GetInt (highlightToggle) == 1)
			color += highlight;

		else if (text && textColorChange)
		{
			text.color = Color.white;
			return;
		}

		if (text && textColorChange)
			text.color = material.GetColor (color);
	}

	public void OnSelect () 
	{
		selected = true;
			
		if(scaleChange && !DOTween.IsTweening ("Select" + GetInstanceID ()))
			buttonRect.DOScale(scaleOnSelected, scaleOnDuration).SetId ("Select" + GetInstanceID ());

		ShaderHighlight ();
	}

	public void OnDeselect () 
	{
		selected = false;

		if(scaleChange && !DOTween.IsTweening ("Deselect" + GetInstanceID ()))
		{
			if (buttonComponent && buttonComponent.interactable == false)
				return;
			
			buttonRect.DOScale(1, scaleOnDuration).SetId ("Deselect" + GetInstanceID ());
		}

		ShaderHighlight ();
	}

	//OnSelect Methods
	public void OnPointerClick( PointerEventData data )
	{
		if (!Application.isPlaying)
			return;

		if (buttonComponent && buttonComponent.interactable == false)
			return;

			if(vibration)
				VibrationManager.Instance.Vibrate (1, FeedbackType.ButtonClick);
			
			OnSelect ();

			ColorChange ();
	}

	public void OnSelect( BaseEventData data )
	{
		if (!Application.isPlaying)
			return;


		OnSelect ();

		SoundsManager.Instance.MenuNavigation ();
	}


	public void OnPointerEnter(PointerEventData data )
	{
		if (!Application.isPlaying)
			return;

		if (buttonComponent && buttonComponent.interactable == false)
			return;


			eventSys.SetSelectedGameObject (null);
			eventSys.SetSelectedGameObject (gameObject);
			
			selected = true;			

		if(scaleChange && !DOTween.IsTweening ("Select" + GetInstanceID ()))
		{
			if (buttonComponent && buttonComponent.interactable == false)
				return;
			
			buttonRect.DOScale(scaleOnSelected, scaleOnDuration).SetId ("Select" + GetInstanceID ());
		}
		
		SoundsManager.Instance.MenuNavigation ();
	}

	public void OnPointerExit( PointerEventData data )
	{
		if (!Application.isPlaying)
			return;


		pointerDown = false;

		//eventSys.SetSelectedGameObject (null);

		OnDeselect ();
	}



	public void OnDeselect( BaseEventData data )
	{
		if (!Application.isPlaying)
			return;

		OnDeselect ();
	}

	public void OnSubmit( BaseEventData data )
	{
		if (!Application.isPlaying)
			return;

		
		if (buttonComponent && buttonComponent.interactable == false)
			return;
		
		if(vibration)
			VibrationManager.Instance.Vibrate (1, FeedbackType.ButtonClick);
		
		SoundsManager.Instance.MenuSubmit ();
		
		ColorChange ();
		
		if (colorChange)
			text.color = mainButton ? GlobalVariables.Instance.mainButtonClickedColorText : GlobalVariables.Instance.secondaryClickedColorText;
		
		ShaderClick (true);
	}

	public void OnPointerDown (PointerEventData eventData)
	{
		if (!Application.isPlaying)
			return;

		if (buttonComponent && buttonComponent.interactable == false)
			return;
		
		pointerDown = true;
		
		ShaderClick ();
		
		ColorChange ();
	}

	public void OnPointerUp (PointerEventData eventData)
	{
		if (!Application.isPlaying)
			return;

		if (buttonComponent && buttonComponent.interactable == false)
			return;
		
		pointerDown = false;
		
		ShaderClick ();
	}
}
