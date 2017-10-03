using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using DG.Tweening;
using Sirenix.OdinInspector;

public class MenuButtonAnimationsAndSounds : MenuShaderElement, IPointerClickHandler, ISelectHandler, IPointerEnterHandler, IPointerExitHandler, IDeselectHandler, ISubmitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Settings")]
    public bool scaleChange = true;
    public bool colorChange = false;
    public bool vibration = true;
    public bool textColorChange = true;

    [Header("Debug")]
    public bool selected;

    private Text text;
    private RectTransform buttonRect;
    private EventSystem eventSys;
    private bool mainButton;
    private bool pointerDown = false;

    private float scaleOnSelected = 1.1f;
    private float scaleOnDuration = 0.25f;
    [HideInInspector]
    public Selectable selectableComponent;
    [HideInInspector]
    public bool selectableSelection = false;
    [HideInInspector]
    public bool forcedHighlight = false;


    protected override void Awake()
    {
        base.Awake();

        eventSys = GameObject.FindGameObjectWithTag("EventSystem").GetComponent<EventSystem>();
        buttonRect = GetComponent<RectTransform>();
        selectableComponent = GetComponent<Selectable>();

        EventSystemScript.OnNewSelectedGameObject += OnNewSelectedGameObject;

        if (textColorChange || colorChange)
            text = transform.GetChild(0).GetComponent<Text>();

        if (colorChange)
            ColorChangeSetup();
    }

    void OnNewSelectedGameObject(GameObject arg)
    {
        if (arg != gameObject && selected)
            OnDeselect();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        EventSystemScript.OnNewSelectedGameObject -= OnNewSelectedGameObject;
    }

    void ColorChangeSetup()
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

        if (selectableComponent && selectableComponent.interactable == false)
            text.color = mainButton ? GlobalVariables.Instance.mainButtonHighlightedColorText : GlobalVariables.Instance.secondaryButtonIdleColorText;
    }

    void Update()
    {
        if (!Application.isPlaying)
            return;

        if (colorChange)
            ColorChangeUpdate();

        if (scaleChange)
        {
            if (selectableComponent && selectableComponent.interactable == false)
                return;
			
            if (eventSys.currentSelectedGameObject != gameObject && buttonRect.localScale != Vector3.one && !DOTween.IsTweening("ResetScale" + GetInstanceID()))
            {
                if (!useShaderOnChildren)
                    buttonRect.DOScale(1, scaleOnDuration).SetId("ResetScale" + GetInstanceID());
                else
                {
                    foreach (var i in imagesComponent)
                        i.rectTransform.DOScale(1, scaleOnDuration).SetId("ResetScale" + GetInstanceID());
                }
            }
        }
    }

    void ColorChangeUpdate()
    {
        if (selectableComponent && selectableComponent.interactable == true)
        {
            if (!pointerDown)
            {
                if (eventSys.currentSelectedGameObject == gameObject)
                    text.color = mainButton ? GlobalVariables.Instance.mainButtonHighlightedColorText : GlobalVariables.Instance.secondaryButtonHighlightedColorText;
                else
                    text.color = mainButton ? GlobalVariables.Instance.mainButtonIdleColorText : GlobalVariables.Instance.secondaryButtonIdleColorText;
            }
        }
        else
            text.color = mainButton ? GlobalVariables.Instance.mainButtonIdleColorText : GlobalVariables.Instance.secondaryButtonIdleColorText;
    }

    void ColorChange()
    {
        if (colorChange)
            text.color = mainButton ? GlobalVariables.Instance.mainButtonClickedColorText : GlobalVariables.Instance.secondaryClickedColorText;
    }

    public override void ShaderColorOnStart()
    {
        if (highlightedOnStart)
            OnSelect();

        if (clickedOnStart)
            ShaderClick(true);
    }

    public override void ShaderColorChange()
    {
        base.ShaderColorChange();

        if (!useUIShader)
            return;

        if (!useEnvironementChroma)
            return;

        ShaderHighlight();

        TextColorChange();
    }

    public void ShaderHighlight()
    {
        if (!useUIShader)
            return;

        ShaderClick(false);

        if (forcedHighlight)
        {
            if (!useShaderOnChildren)
                material.SetInt(highlightToggle, 1);
            else
                foreach (var m in materials)
                    if (selected)
                        m.SetInt(highlightToggle, 1);
        }
        else
        {
            if (!useShaderOnChildren)
            {
                if (selected)
                    material.SetInt(highlightToggle, 1);
                else
                    material.SetInt(highlightToggle, 0);
            }
            else
            {
                foreach (var m in materials)
                    if (selected)
                        m.SetInt(highlightToggle, 1);
                    else
                        m.SetInt(highlightToggle, 0);
            }
        }

        TextColorChange();
    }

    public void ShaderClick(bool clickEnable)
    {
        if (!useUIShader)
            return;

        if (!useShaderOnChildren)
        {
            if (clickEnable)
                material.SetInt(clickToggle, 1);
            else
                material.SetInt(clickToggle, 0);
        }
        else
        {
            foreach (var m in materials)
                if (clickEnable)
                    m.SetInt(clickToggle, 1);
                else
                    m.SetInt(clickToggle, 0);
        }
		
        TextColorChange();
    }

    public void ShaderClickDuration()
    {
        if (!useUIShader)
            return;

        if (!useShaderOnChildren)
            material.SetInt(clickToggle, 1);
        else
            foreach (var m in materials)
                m.SetInt(clickToggle, 1);
		
        TextColorChange();
		
        DOVirtual.DelayedCall(clickDuration, () =>
            {
                if (!useShaderOnChildren)
                    material.SetInt(clickToggle, 0);
                else
                    foreach (var m in materials)
                        m.SetInt(clickToggle, 0);
				
                TextColorChange();
            });
		
    }

    public void ShaderHighlightClick()
    {
        forcedHighlight = true;

        OnSelect();

        DOVirtual.DelayedCall(0.2f, () =>
            {
                forcedHighlight = false;
                OnDeselect();
            });
    }

    void TextColorChange()
    {
        string color = chromas[(int)globalVariables.environementChroma];

        if (!useShaderOnChildren && material && material.GetInt(clickToggle) == 1 || useShaderOnChildren && materials[0] && materials[0].GetInt(clickToggle) == 1)
            color += click;
        else if (!useShaderOnChildren && material.GetInt(highlightToggle) == 1 || useShaderOnChildren && materials[0] && materials[0].GetInt(highlightToggle) == 1)
            color += highlight;
        else if (text && textColorChange)
        {
            text.color = Color.white;
            return;
        }

        if (text && textColorChange)
        {
            if (!useShaderOnChildren)
                text.color = material.GetColor(color);
            else
                text.color = materials[0].GetColor(color);
        }
    }

    public void OnSelect()
    {
        selected = true;
			
        if (scaleChange && !DOTween.IsTweening("Select" + GetInstanceID()))
        {
            if (!useShaderOnChildren)
                buttonRect.DOScale(scaleOnSelected, scaleOnDuration).SetId("Select" + GetInstanceID());
            else
            {
                foreach (var i in imagesComponent)
                    i.rectTransform.DOScale(scaleOnSelected, scaleOnDuration).SetId("Select" + GetInstanceID());
            }
        }

        ShaderHighlight();
    }

    public void OnDeselect()
    {
        selected = false;

        if (scaleChange && !DOTween.IsTweening("Deselect" + GetInstanceID()))
        {
            if (selectableComponent && selectableComponent.interactable == false)
                return;

            if (!useShaderOnChildren)
                buttonRect.DOScale(1, scaleOnDuration).SetId("Deselect" + GetInstanceID());
            else
            {
                foreach (var i in imagesComponent)
                    i.rectTransform.DOScale(1, scaleOnDuration).SetId("Deselect" + GetInstanceID());
            }
        }

        ShaderHighlight();
    }

    //OnSelect Methods
    public void OnPointerClick(PointerEventData data)
    {
        if (!Application.isPlaying)
            return;
		
        if (selectableComponent && selectableComponent.interactable == false)
            return;
		
        if (vibration)
            VibrationManager.Instance.Vibrate(1, FeedbackType.ButtonClick);
		
        SoundsManager.Instance.MenuSubmit();
		
        OnSelect();
		
        ColorChange();
    }

    public void OnSelect(BaseEventData data)
    {
        if (!Application.isPlaying)
            return;

        OnSelect();

        if (!selectableSelection)
            SoundsManager.Instance.MenuNavigation();
        else
            selectableSelection = false;
    }


    public void OnPointerEnter(PointerEventData data)
    {
        if (!Application.isPlaying)
            return;

        if (selectableComponent && selectableComponent.interactable == false)
            return;


        eventSys.SetSelectedGameObject(null);
        eventSys.SetSelectedGameObject(gameObject);
			
        selected = true;			

        if (scaleChange && !DOTween.IsTweening("Select" + GetInstanceID()))
        {
            if (selectableComponent && selectableComponent.interactable == false)
                return;
			
            if (!useShaderOnChildren)
                buttonRect.DOScale(scaleOnSelected, scaleOnDuration).SetId("Select" + GetInstanceID());
            else
            {
                foreach (var i in imagesComponent)
                    i.rectTransform.DOScale(scaleOnSelected, scaleOnDuration).SetId("Select" + GetInstanceID());
            }
        }
		
        SoundsManager.Instance.MenuNavigation();
    }

    public void OnPointerExit(PointerEventData data)
    {
        if (!Application.isPlaying)
            return;


        pointerDown = false;

        //eventSys.SetSelectedGameObject (null);

        OnDeselect();
    }


    public void OnDeselect(BaseEventData data)
    {
        if (!Application.isPlaying)
            return;

        OnDeselect();
    }

    public void OnSubmit(BaseEventData data)
    {
        if (!Application.isPlaying)
            return;

		
        if (selectableComponent && selectableComponent.interactable == false)
            return;
		
        if (vibration)
            VibrationManager.Instance.Vibrate(1, FeedbackType.ButtonClick);
		
        SoundsManager.Instance.MenuSubmit();
		
        ColorChange();
		
        if (colorChange)
            text.color = mainButton ? GlobalVariables.Instance.mainButtonClickedColorText : GlobalVariables.Instance.secondaryClickedColorText;
		
        ShaderClickDuration();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!Application.isPlaying)
            return;

        if (selectableComponent && selectableComponent.interactable == false)
            return;
		
        pointerDown = true;
		
        ShaderClick(true);
		
        ColorChange();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!Application.isPlaying)
            return;

        if (selectableComponent && selectableComponent.interactable == false)
            return;
		
        pointerDown = false;
		
        ShaderClick(false);
    }
}
