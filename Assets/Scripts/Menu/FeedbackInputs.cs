using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class FeedbackInputs : MonoBehaviour 
{
	public KeyCode keycode;

	public string axisX;
	public string axisY;

	public float originScale;
	public float modifiedScale;

	public Color modifiedColor;

	public bool keyPressed;

	// Use this for initialization
	void Start () 
	{
		DOTween.Init ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(keycode != KeyCode.None)
		{
			if(Input.GetKey(keycode) || Input.GetKeyDown(keycode))
			{
				Feedback ();
				keyPressed = true;
			}
			
			if(Input.GetKeyUp(keycode))
			{
				Feedback2 ();
				keyPressed = false;
			}
		}

		if(axisX != "" && axisY != "")
		{
			if(Input.GetAxisRaw(axisX) != 0 || Input.GetAxisRaw(axisY) != 0)
			{
				Feedback ();
				keyPressed = true;
			}
			
			if(Input.GetAxisRaw(axisX) == 0 && Input.GetAxisRaw(axisY) == 0)
			{
				Feedback2 ();
				keyPressed = false;
			}
		}
	
		if(axisX != "")
		{
			if(Input.GetAxisRaw(axisX) != 0)
			{
				Feedback ();
				keyPressed = true;
			}
			
			if(Input.GetAxisRaw(axisX) == 0)
			{
				Feedback2 ();
				keyPressed = false;
			}
		}
	}

	void Feedback ()
	{
		transform.DOScale (modifiedScale, 0.2f);

		if(transform.gameObject.GetComponent<SpriteRenderer> () != null)
			transform.gameObject.GetComponent<SpriteRenderer> ().color = modifiedColor;

		if(transform.gameObject.GetComponent<Image> () != null)
			transform.gameObject.GetComponent<Image> ().color = modifiedColor;
	}

	void Feedback2 ()
	{
		transform.DOScale (originScale, 0.2f);

		if(transform.gameObject.GetComponent<SpriteRenderer> () != null)
			transform.gameObject.GetComponent<SpriteRenderer> ().color = Color.white;

		if(transform.gameObject.GetComponent<Image> () != null)
			transform.gameObject.GetComponent<Image> ().color = Color.white;
	}
}
