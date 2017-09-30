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

	public bool overrideMainColor = false;
	[ShowIf("overrideMainColor")]
	public Color mainColor;

	[ShowIf("useUIShader")]
	public Texture neonTexture;

	[Header ("UI Shader")]
	public bool useShaderOnChildren = false;
	public bool isInScrollRect = false;
	[ShowIf("useShaderOnChildren")]
	public List<Image> imagesComponent = new List<Image> ();
	public bool overrideRenderQueue = false;
	[ShowIf("overrideRenderQueue")]
	public int renderQueue = 3000;


	[Header ("On Start")]
	public bool highlightedOnStart = false;
	public bool clickedOnStart = false;

	[Header ("Emission Power")]
	public bool overrideEmissionPower = false;
	[ShowIf("overrideEmissionPower")]
	public float emissionPower = 1f;

	protected Material material;
	protected List<Material> materials = new List<Material> ();

	protected Image imageComponent;

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
		if (!useShaderOnChildren && GetComponent<Image> ().material == null )
			NewMaterial ();
		else
		{
			foreach(var i in imagesComponent)
				if(i.material == null || !i.material.HasProperty ("_PURPLECHROMAIdle"))
				{
					NewMaterial ();
					break;
				}
		}

		if(!useShaderOnChildren)
		{
			imageComponent = GetComponent<Image> ();
			material = imageComponent.material;
		}
		else
		{
			materials.Clear ();

			foreach(var i in imagesComponent)
				materials.Add (i.material);
		}

		if (useUIShader && useEnvironementChroma && GlobalVariables.Instance)
			GlobalVariables.Instance.OnEnvironementChromaChange += ShaderColorChange;

		if(useUIShader)
			ShaderColorChange ();
	}

	protected virtual void OnDestroy ()
	{
		if (useUIShader && useEnvironementChroma && GlobalVariables.Instance)
			GlobalVariables.Instance.OnEnvironementChromaChange -= ShaderColorChange;
		
	}

	protected virtual void Start ()
	{
		ShaderColorOnStart ();
	}

	public virtual void ShaderColorOnStart ()
	{
		if(!useShaderOnChildren)
		{
			if(highlightedOnStart)
				material.SetInt (highlightToggle, 1);

			if(clickedOnStart)
				material.SetInt (clickToggle, 1);
		}
		else
		{
			foreach(var i in imagesComponent)
			{
				if(highlightedOnStart)
					i.material.SetInt (highlightToggle, 1);

				if(clickedOnStart)
					i.material.SetInt (clickToggle, 1);
			}
		}
	}

	public virtual void ShaderColorChange ()
	{
		if(!useShaderOnChildren)
		{
			imageComponent = GetComponent<Image> ();
			material = imageComponent.material;
		}
		else
		{
			materials.Clear ();

			foreach(var i in imagesComponent)
			{
				materials.Add (i.material);
			}
		}


		if(globalVariables == null)
			globalVariables = Application.isPlaying ? GlobalVariables.Instance : FindObjectOfType<GlobalVariables> ();

		if (!useUIShader)
			return;

		if(!useEnvironementChroma && usePlayerColor)
		{
			if(!useShaderOnChildren)
				material.SetColor ("_PURPLECHROMAIdle", globalVariables.playersColors [(int)playerColor]);
			else
				foreach(var m in materials)
					m.SetColor ("_PURPLECHROMAIdle", globalVariables.playersColors [(int)playerColor]);
		}

		if (!useEnvironementChroma)
			return;

		foreach(string s in chromasToggles)
		{
			if(!useShaderOnChildren)
				material.SetInt (s, 0);
			else
				foreach(var m in materials)
					m.SetInt (s, 0);
		}

		if(globalVariables.environementChroma != EnvironementChroma.Purple)
		{
			if(!useShaderOnChildren)
				material.SetInt (chromasToggles [(int)globalVariables.environementChroma - 1], 1);
			else
				foreach(var m in materials)
					m.SetInt (chromasToggles [(int)globalVariables.environementChroma - 1], 1);
		}
	}

	public void NewMaterial (bool forceReset = false)
	{
		if (!useShaderOnChildren && GetComponent<Image> ().material == null)
		{
			if(isInScrollRect)
				imageComponent.material = new Material (globalVariables.uiMaterialScrollRect);
			else
				imageComponent.material = new Material (globalVariables.uiMaterial);
		}
		else
		{
			foreach(var i in imagesComponent)
				if(i.material == null || !i.material.HasProperty ("_PURPLECHROMAIdle"))
				{
					if(isInScrollRect)
						i.material = new Material (globalVariables.uiMaterialScrollRect);
					else
						i.material = new Material (globalVariables.uiMaterial);
				}
		}

		if(globalVariables == null)
			globalVariables = Application.isPlaying ? GlobalVariables.Instance : FindObjectOfType<GlobalVariables> ();

		if (!useUIShader)
			return;

		if(!useShaderOnChildren)
		{
			imageComponent = GetComponent<Image> ();
			material = imageComponent.material;

			Color mainColor = new Color ();

			if(imageComponent.material && imageComponent.material.HasProperty ("_PURPLECHROMAIdle"))
				mainColor = imageComponent.material.GetColor ("_PURPLECHROMAIdle");

			if(isInScrollRect)
				imageComponent.material = new Material (globalVariables.uiMaterialScrollRect);
			else
				imageComponent.material = new Material (globalVariables.uiMaterial);

			imageComponent.material.SetTexture ("_T_Button", imageComponent.mainTexture);

			if(neonTexture != null)
				imageComponent.material.SetTexture ("_T_Neon", neonTexture);
			else
				imageComponent.material.SetTexture ("_T_Neon", imageComponent.mainTexture);

			if (overrideEmissionPower)
				imageComponent.material.SetFloat ("_EmissionPowerButton", emissionPower);

			if(!forceReset && mainColor != imageComponent.material.GetColor ("_PURPLECHROMAIdle") || mainColor != imageComponent.material.GetColor ("_PURPLECHROMAIdle") && !useEnvironementChroma && !usePlayerColor)
				imageComponent.material.SetColor ("_PURPLECHROMAIdle", mainColor);

			if(overrideMainColor)
			{
				imageComponent.material.SetColor ("_PURPLECHROMAIdle", this.mainColor);
				imageComponent.material.SetColor ("_BLUECHROMAIdle", this.mainColor);
				imageComponent.material.SetColor ("_GREENCHROMAIdle", this.mainColor);
				imageComponent.material.SetColor ("_ORANGECHROMAIdle", this.mainColor);
			}

			if (overrideRenderQueue)
				material.renderQueue = renderQueue;

			material = imageComponent.material;
		}
		else
		{
			materials.Clear ();

			foreach(var i in imagesComponent)
				materials.Add (i.material);

			for(int i = 0; i < imagesComponent.Count; i++)
			{
				Color mainColor = new Color ();

				if(materials [i] && materials [i].HasProperty ("_PURPLECHROMAIdle"))
					mainColor = materials [i].GetColor ("_PURPLECHROMAIdle");

				if(isInScrollRect)
					imagesComponent [i].material = new Material (globalVariables.uiMaterialScrollRect);
				else
					imagesComponent [i].material = new Material (globalVariables.uiMaterial);

				imagesComponent [i].material.SetTexture ("_T_Button", imagesComponent [i].mainTexture);

				imagesComponent [i].material.SetTexture ("_T_Neon", imagesComponent [i].mainTexture);

				if (overrideEmissionPower)
					imagesComponent [i].material.SetFloat ("_EmissionPowerButton", emissionPower);

				if(!forceReset && mainColor != materials [i].GetColor ("_PURPLECHROMAIdle") || mainColor != imagesComponent [i].material.GetColor ("_PURPLECHROMAIdle") && !useEnvironementChroma && !usePlayerColor)
					imagesComponent [i].material.SetColor ("_PURPLECHROMAIdle", mainColor);

				if(overrideMainColor)
				{
					imagesComponent [i].material.SetColor ("_PURPLECHROMAIdle", this.mainColor);
					imagesComponent [i].material.SetColor ("_BLUECHROMAIdle", this.mainColor);
					imagesComponent [i].material.SetColor ("_GREENCHROMAIdle", this.mainColor);
					imagesComponent [i].material.SetColor ("_ORANGECHROMAIdle", this.mainColor);
				}

				if (overrideRenderQueue)
					imagesComponent [i].material.renderQueue = renderQueue;

				materials [i] = imagesComponent [i].material;
			}
		}
	}
}
