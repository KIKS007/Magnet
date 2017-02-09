using UnityEngine;
using System.Collections;
using DarkTonic.MasterAudio;
using DG.Tweening;

public class MovableDeadCube : MovableScript
{
	[Header ("DEADCUBE")]
	public float explosionForce = 50;
	public float explosionRadius = 50;
	public LayerMask explosionMask = (1 << 9) | (1 << 12);

	protected override void OnEnable ()
	{
		hold = false;

		rigidbodyMovable = GetComponent<Rigidbody>();
		movableRenderer = GetComponent<Renderer> ();
		cubeMeshFilter = transform.GetChild (2).GetComponent<MeshFilter> ();
		cubeMaterial = transform.GetChild (1).GetComponent<Renderer> ().material;
		slowMoTrigger = transform.GetComponentInChildren<SlowMotionTriggerScript> ();

		//cubeMaterial.SetFloat ("_Lerp", 1);
		//cubeMaterial.SetColor ("_Color", Color.black);

		cubeMeshFilter.mesh = GlobalVariables.Instance.cubesStripes [Random.Range (0, GlobalVariables.Instance.cubesStripes.Length)];
		attracedBy.Clear ();
		repulsedBy.Clear ();
	}

	protected override void Start ()
	{
		SetDeadColor ();
	}

	void SetDeadColor ()
	{
		if (DOTween.IsTweening ("CubeNeutralTween" + gameObject.GetInstanceID ()))
			DOTween.Kill ("CubeNeutralTween" + gameObject.GetInstanceID ());

		Color cubeColorTemp = cubeMaterial.GetColor("_Color");
		float cubeLerpTemp = cubeMaterial.GetFloat ("_Lerp");

		DOTween.To(()=> cubeColorTemp, x=> cubeColorTemp =x, Color.black, toColorDuration).OnUpdate(()=> cubeMaterial.SetColor("_Color", cubeColorTemp)).SetId("CubeColorTween" + gameObject.GetInstanceID ());
		DOTween.To(()=> cubeLerpTemp, x=> cubeLerpTemp =x, 1, toColorDuration).OnUpdate(()=> cubeMaterial.SetFloat("_Lerp", cubeLerpTemp)).SetId("CubeColorTween" + gameObject.GetInstanceID ());
	}

	protected override void Update ()
	{
		if(hold == false && rigidbodyMovable != null)
			currentVelocity = rigidbodyMovable.velocity.magnitude;


		if(hold == false && currentVelocity > 0)
		{
			if(currentVelocity > higherVelocity)
				higherVelocity = currentVelocity;
		}
	}

	protected override void HitPlayer (Collision other)
	{
		if(other.collider.tag == "Player" && other.collider.GetComponent<PlayersGameplay>().playerState != PlayerState.Stunned)
		{
			GlobalMethods.Instance.Explosion (transform.position, explosionForce, explosionRadius, explosionMask);
		}
	}
}
