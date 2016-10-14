using UnityEngine;
using System.Collections;
using DG.Tweening;

public enum RepulseTriggerZones {Zone1, Zone2, Zone3, Zone4, None};

public class MovableRepulse : MovableScript 
{
	[Header("Repulse Mode")]
	public RepulseTriggerZones movableZone = RepulseTriggerZones.None;

	public bool inZone1;
	public bool inZone2;
	public bool inZone3;
	public bool inZone4;

	private Material movableMaterial;

	public Color[] zonesColors = new Color[4];

	protected override void Start ()
	{
		base.Start ();

		movableMaterial = GetComponent<Renderer> ().material;
	}

	protected override void Update () 
	{
		if(hold == false)
			currentVelocity = rigidbodyMovable.velocity.magnitude;


		if(hold == false && currentVelocity > 0)
		{
			if(currentVelocity > higherVelocity)
			{
				higherVelocity = currentVelocity;
			}

			if(currentVelocity >= limitVelocity)
			{
				gameObject.tag = "ThrownMovable";
			}
			else if(currentVelocity < limitVelocity && gameObject.tag == "ThrownMovable")
			{
				gameObject.tag = "Movable";
			}
		}

		FindZone ();

		SetColor ();
	}

	void FindZone ()
	{
		if(inZone1 && !inZone2 && !inZone3 && !inZone4)
		{
			movableZone = RepulseTriggerZones.Zone1;
		}
		else if(!inZone1 && inZone2 && !inZone3 && !inZone4)
		{
			movableZone = RepulseTriggerZones.Zone2;
		}
		else if(!inZone1 && !inZone2 && inZone3 && !inZone4)
		{
			movableZone = RepulseTriggerZones.Zone3;
		}
		else if(!inZone1 && !inZone2 && !inZone3 && inZone4)
		{
			movableZone = RepulseTriggerZones.Zone4;
		}
		else
		{
			movableZone = RepulseTriggerZones.None;
		}
	}

	void SetColor ()
	{
		if(movableZone == RepulseTriggerZones.Zone1 && movableMaterial.color != zonesColors[0])
			DOTween.To(()=> movableMaterial.color, x=> movableMaterial.color =x, zonesColors[0], toColorDuration);

		if(movableZone == RepulseTriggerZones.Zone2 && movableMaterial.color != zonesColors[1])
			DOTween.To(()=> movableMaterial.color, x=> movableMaterial.color =x, zonesColors[1], toColorDuration);

		if(movableZone == RepulseTriggerZones.Zone3 && movableMaterial.color != zonesColors[2])
			DOTween.To(()=> movableMaterial.color, x=> movableMaterial.color =x, zonesColors[2], toColorDuration);

		if(movableZone == RepulseTriggerZones.Zone4 && movableMaterial.color != zonesColors[3])
			DOTween.To(()=> movableMaterial.color, x=> movableMaterial.color =x, zonesColors[3], toColorDuration);

		if(movableZone == RepulseTriggerZones.None && movableMaterial.color != Color.white)
			DOTween.To(()=> movableMaterial.color, x=> movableMaterial.color =x, Color.white, toColorDuration);
	}
}
