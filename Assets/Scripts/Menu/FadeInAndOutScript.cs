using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class FadeInAndOutScript : MonoBehaviour
{
    public float durationFadeIn;
    public float durationFadeOut;

    [Range(0, 255)]
    public float alphaMinimum;


    private Text textComponent;
    private Image imageComponent;
    private Color originalColor;

    // Use this for initialization
    void OnEnable()
    {
        if (!gameObject.activeInHierarchy || !gameObject.activeSelf)
            return;
        
        if (GetComponent<Text>() != null)
        {
            textComponent = GetComponent<Text>();
            originalColor = textComponent.color;
        }

        if (GetComponent<Image>() != null)
        {
            imageComponent = GetComponent<Image>();
            originalColor = imageComponent.color;
        }

        FadeOut();
    }

    void FadeOut()
    {
        if (!gameObject.activeInHierarchy || !gameObject.activeSelf)
            return;
        
        if (GetComponent<Text>() != null)
        {
            textComponent.DOColor(new Color(originalColor.r, originalColor.g, originalColor.b, alphaMinimum / 255), durationFadeOut).OnComplete(FadeIn).SetId("Fade" + gameObject.GetInstanceID());
        }
		
        if (GetComponent<Image>() != null)
        {
            imageComponent.DOColor(new Color(originalColor.r, originalColor.g, originalColor.b, alphaMinimum / 255), durationFadeOut).OnComplete(FadeIn).SetId("Fade" + gameObject.GetInstanceID());
        }			
    }

    void FadeIn()
    {
        if (!gameObject.activeInHierarchy || !gameObject.activeSelf)
            return;
        
        if (GetComponent<Text>() != null)
        {
            textComponent.DOColor(new Color(originalColor.r, originalColor.g, originalColor.b, 255 / 255), durationFadeIn).OnComplete(FadeOut).SetId("Fade" + gameObject.GetInstanceID());
        }
		
        if (GetComponent<Image>() != null)
        {
            imageComponent.DOColor(new Color(originalColor.r, originalColor.g, originalColor.b, 255 / 255), durationFadeIn).OnComplete(FadeOut).SetId("Fade" + gameObject.GetInstanceID());
        }			
    }
}
