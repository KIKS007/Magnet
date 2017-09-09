using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

public class MenuShaderElement : MonoBehaviour 
{
	[ButtonGroup ("a", -1)]
	public void UpdateShaderAll ()
	{
		foreach (var s in Resources.FindObjectsOfTypeAll<MenuShaderElement> ())
		{
			s.NewMaterial ();
			s.ShaderColorChange ();
		}
	}

	[ButtonGroup ("a", -1)]
	public void ResetShaderAll ()
	{
		foreach (var s in Resources.FindObjectsOfTypeAll<MenuShaderElement> ())
		{
			s.NewMaterial (true);
			s.ShaderColorChange ();
		}
	}

	public bool IsntUsingEnvironementChroma ()
	{
		return !useEnvironementChroma;
	}

	public bool UsePlayerColor ()
	{
		return !useEnvironementChroma && useUIShader;
	}

	public bool PlayerColor ()
	{
		return !useEnvironementChroma && useUIShader && usePlayerColor;
	}

	[Header ("UI Shader")]
	public bool useUIShader = true;

	[ShowIf("useUIShader")]
	public bool useEnvironementChroma = true;

	[ShowIf("UsePlayerColor")]
	public bool usePlayerColor = true;

	[ShowIf("PlayerColor")]
	public PlayerName playerColor = PlayerName.Player1;

	[ShowIf("useUIShader")]
	public Texture neonTexture;

	protected Material material;
	protected Image image;

	protected string[] chromasToggles = new string[] {"_BlueChroma", "_GreenChroma", "_OrangeChroma" };
	protected string highlightToggle = "_Highlighting";
	protected string clickToggle = "_Selection";
	protected float clickDuration = 0.2f;

	protected string [] chromas = new string[] {"_PURPLECHROMA", "_BLUECHROMA", "_GREENCHROMA", "_ORANGECHROMA" };
	protected string highlight = "Highlight";
	protected string click = "Selection";

	protected static GlobalVariables globalVariables;

	// Use this for initialization
	protected virtual void Awake ()
	{
		image = GetComponent<Image> ();
		material = image.material;

		if (useUIShader && useEnvironementChroma && GlobalVariables.Instance)
			GlobalVariables.Instance.OnEnvironementChromaChange += ShaderColorChange;

		if(useUIShader)
			ShaderColorChange ();
		
	}

	public virtual void ShaderColorChange ()
	{
		if (material == null)
		{
			image = GetComponent<Image> ();
			material = image.material;
		}

		if(globalVariables == null)
			globalVariables = Application.isPlaying ? GlobalVariables.Instance : FindObjectOfType<GlobalVariables> ();

		if (!useUIShader)
			return;

		if(!useEnvironementChroma && usePlayerColor)
			material.SetColor ("_PURPLECHROMAIdle", globalVariables.playersColors [(int)playerColor]);

		if (!useEnvironementChroma)
			return;

		foreach(string s in chromasToggles)
			material.SetInt (s, 0);

		if(globalVariables.environementChroma != EnvironementChroma.Purple)
			material.SetInt (chromasToggles [(int)globalVariables.environementChroma - 1], 1);
	}

	public void NewMaterial (bool forceReset = false)
	{
		if(globalVariables == null)
			globalVariables = Application.isPlaying ? GlobalVariables.Instance : FindObjectOfType<GlobalVariables> ();

		if (!useUIShader)
			return;

		image = GetComponent<Image> ();
		Color mainColor = image.material.GetColor ("_PURPLECHROMAIdle");

		image.material = new Material (globalVariables.uiMaterial);

		image.material.SetTexture ("_T_Button", image.mainTexture);

		if(neonTexture != null)
			image.material.SetTexture ("_T_Neon", neonTexture);
		else
			image.material.SetTexture ("_T_Neon", image.mainTexture);

		if(!forceReset && mainColor != image.material.GetColor ("_PURPLECHROMAIdle") || mainColor != image.material.GetColor ("_PURPLECHROMAIdle") && !useEnvironementChroma && !usePlayerColor)
			image.material.SetColor ("_PURPLECHROMAIdle", mainColor);
		
		material = image.material;
	}
}
