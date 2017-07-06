using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using DG.Tweening;
using Sirenix.OdinInspector;

[ExecuteInEditMode]
public class MenuButtonAnimationsAndSounds : MonoBehaviour, IPointerClickHandler, ISelectHandler, IPointerEnterHandler, IPointerExitHandler, IDeselectHandler, ISubmitHandler, IPointerDownHandler, IPointerUpHandler
{
	[PropertyOrder(-1)]
	[ButtonAttribute ()]
	public void UpdateShaderAll ()
	{
		foreach (var s in FindObjectsOfType<MenuButtonAnimationsAndSounds> ())
			s.SetupShader ();
	}

	[Header ("Settings")]
	public bool scaleChange = true;
	public bool colorChange = true;
	public bool vibration = true;
	public bool useUIShader = false;
	public bool textColorChange = false;

	[Header ("Debug")]
	public bool selected;

	[Header ("Shader")]
	public Texture neonTexture;

	private Text text;
	private RectTransform buttonRect;
	private EventSystem eventSys;
	private bool mainButton;
	private bool pointerDown = false;

	private float scaleOnSelected = 1.1f;
	private float scaleOnDuration = 0.25f;
	private Button buttonComponent;

	private Material material;

	private string[] chromasToggles = new string[] {"_BlueChroma", "_GreenChroma", "_OrangeChroma" };
	private string highlightToggle = "_Highlighting";
	private string clickToggle = "_Selection";
	private float clickDuration = 0.2f;

	private string [] chromas = new string[] {"_PURPLECHROMA", "_BLUECHROMA", "_GREENCHROMA", "_ORANGECHROMA" };
	private string idle = "Idle";
	private string highlight = "Highlight";
	private string click = "Selection";

	void Awake	 ()
	{
		if (!Application.isPlaying)
			return;

		eventSys = GameObject.FindGameObjectWithTag ("EventSystem").GetComponent<EventSystem> ();
		buttonRect = GetComponent<RectTransform> ();
		buttonComponent = GetComponent<Button> ();
		text = transform.GetChild (0).GetComponent<Text> ();

		SetupShader ();

		GlobalVariables.Instance.OnEnvironementChromaChange += ShaderColorChange;

		if(useUIShader)
			ShaderColorChange ();

		if (colorChange)
			ColorChangeSetup ();

	}
		
	void OnEnable ()
	{
		if (!Application.isPlaying)
			SetupShader ();
	}
		
	void SetupShader ()
	{
		if (!useUIShader)
			return;

		GlobalVariables gv = FindObjectOfType<GlobalVariables> ();

		Image image = GetComponent<Image> ();

		image.material = new Material (gv.uiMaterial);
		material = image.material;

		material.SetTexture ("_T_Button", image.mainTexture);

		if(neonTexture != null)
			material.SetTexture ("_T_Neon", neonTexture);
		else
			material.SetTexture ("_T_Neon", image.mainTexture);
			
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

		if(scaleChange && buttonComponent.interactable == true)
		{
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

	void ShaderColorChange ()
	{
		if (!useUIShader)
			return;

		foreach(string s in chromasToggles)
			material.SetInt (s, 0);

		if(GlobalVariables.Instance.environementChroma != EnvironementChroma.Purple)
			material.SetInt (chromasToggles [(int)GlobalVariables.Instance.environementChroma - 1], 1);

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
		string color = chromas [(int)GlobalVariables.Instance.environementChroma];

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

		if(buttonComponent.interactable == true && scaleChange && !DOTween.IsTweening ("Deselect" + GetInstanceID ()))
			buttonRect.DOScale(1, scaleOnDuration).SetId ("Deselect" + GetInstanceID ());

		ShaderHighlight ();
	}

	//OnSelect Methods
	public void OnPointerClick( PointerEventData data )
	{
		if (!Application.isPlaying)
			return;


		if(buttonComponent.interactable == true)
		{
			if(vibration)
				VibrationManager.Instance.Vibrate (1, FeedbackType.ButtonClick);
			
			OnSelect ();

			ColorChange ();
		}
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


		if(buttonComponent.interactable == true)
		{
			eventSys.SetSelectedGameObject (null);
			eventSys.SetSelectedGameObject (gameObject);
			
			selected = true;			
		}

		if(buttonComponent.interactable == true && scaleChange && !DOTween.IsTweening ("Select" + GetInstanceID ()))
			buttonRect.DOScale(scaleOnSelected, scaleOnDuration).SetId ("Select" + GetInstanceID ());
		
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


		if(buttonComponent.interactable == true)
		{
			if(vibration)
				VibrationManager.Instance.Vibrate (1, FeedbackType.ButtonClick);
			
			OnDeselect ();

			SoundsManager.Instance.MenuSubmit ();

			ColorChange ();

			if (colorChange)
				text.color = mainButton ? GlobalVariables.Instance.mainButtonClickedColorText : GlobalVariables.Instance.secondaryClickedColorText;

			ShaderClick (true);
		}
	}

	public void OnPointerDown (PointerEventData eventData)
	{
		if (!Application.isPlaying)
			return;


		if(buttonComponent.interactable == true)
		{
			pointerDown = true;

			ShaderClick ();

			ColorChange ();
		}
	}

	public void OnPointerUp (PointerEventData eventData)
	{
		if (!Application.isPlaying)
			return;


		if(buttonComponent.interactable == true)
		{
			pointerDown = false;

			ShaderClick ();
		}
	}
}
