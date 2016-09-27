using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MovableTestColor : MonoBehaviour 
{
	public float tweenDuration = 0.5f;
	public float timeBeforeColor;
	public float timeBeforeNeutral;
	private Material cubeMaterial;
	private MeshFilter cubeMeshFilter;

	// Use this for initialization
	void Start () 
	{
		GameObject.FindGameObjectWithTag ("MainCamera").transform.position = new Vector3 (0, 0, -5.5f);
		GameObject.FindGameObjectWithTag ("MainCamera").transform.rotation = Quaternion.Euler(Vector3.zero);

		cubeMaterial = transform.GetChild (0).GetComponent<Renderer> ().material;
		cubeMeshFilter = transform.GetChild (1).GetComponent<MeshFilter> ();
	}

	void OnEnable ()
	{
		cubeMaterial = transform.GetChild (0).GetComponent<Renderer> ().material;
		cubeMeshFilter = transform.GetChild (1).GetComponent<MeshFilter> ();cubeMaterial.SetFloat ("_Lerp", 0);

		cubeMaterial.SetColor ("_Color", GlobalVariables.Instance.cubeNeutralColor);

		//cubeMeshFilter.mesh = GlobalVariables.Instance.cubesStripes [Random.Range (0, GlobalVariables.Instance.cubesStripes.Length)];

		StopCoroutine (ColorTest ());
		StartCoroutine (ColorTest ());
	}

	IEnumerator ColorTest ()
	{
		Color cubeCorrectColor = new Color ();

		cubeCorrectColor = GlobalVariables.Instance.cubeColorplayer1;

		Color cubeColorTemp = cubeMaterial.GetColor("_Color");
		float cubeLerpTemp = cubeMaterial.GetFloat ("_Lerp");

		DOTween.To(()=> cubeColorTemp, x=> cubeColorTemp =x, cubeCorrectColor, tweenDuration).OnUpdate(()=> cubeMaterial.SetColor("_Color", cubeColorTemp));
		DOTween.To(()=> cubeLerpTemp, x=> cubeLerpTemp =x, 1, tweenDuration).OnUpdate(()=> cubeMaterial.SetFloat("_Lerp", cubeLerpTemp));


		yield return new WaitForSeconds (timeBeforeNeutral);


		cubeColorTemp = cubeMaterial.GetColor("_Color");
		cubeLerpTemp = cubeMaterial.GetFloat ("_Lerp");

		DOTween.To(()=> cubeColorTemp, x=> cubeColorTemp =x, GlobalVariables.Instance.cubeNeutralColor, tweenDuration).OnUpdate(()=> cubeMaterial.SetColor("_Color", cubeColorTemp));
		DOTween.To(()=> cubeLerpTemp, x=> cubeLerpTemp =x, 0, tweenDuration).OnUpdate(()=> cubeMaterial.SetFloat("_Lerp", cubeLerpTemp));


		yield return new WaitForSeconds (timeBeforeColor);

		cubeCorrectColor = GlobalVariables.Instance.cubeColorplayer3;

		cubeColorTemp = cubeMaterial.GetColor("_Color");
		cubeLerpTemp = cubeMaterial.GetFloat ("_Lerp");

		DOTween.To(()=> cubeColorTemp, x=> cubeColorTemp =x, cubeCorrectColor, tweenDuration).OnUpdate(()=> cubeMaterial.SetColor("_Color", cubeColorTemp));
		DOTween.To(()=> cubeLerpTemp, x=> cubeLerpTemp =x, 1, tweenDuration).OnUpdate(()=> cubeMaterial.SetFloat("_Lerp", cubeLerpTemp));


		yield return new WaitForSeconds (timeBeforeNeutral);


		cubeColorTemp = cubeMaterial.GetColor("_Color");
		cubeLerpTemp = cubeMaterial.GetFloat ("_Lerp");

		DOTween.To(()=> cubeColorTemp, x=> cubeColorTemp =x, GlobalVariables.Instance.cubeNeutralColor, tweenDuration).OnUpdate(()=> cubeMaterial.SetColor("_Color", cubeColorTemp));
		DOTween.To(()=> cubeLerpTemp, x=> cubeLerpTemp =x, 0, tweenDuration).OnUpdate(()=> cubeMaterial.SetFloat("_Lerp", cubeLerpTemp));


		yield return new WaitForSeconds (timeBeforeColor);

		cubeCorrectColor = GlobalVariables.Instance.cubeColorplayer4;

		cubeColorTemp = cubeMaterial.GetColor("_Color");
		cubeLerpTemp = cubeMaterial.GetFloat ("_Lerp");

		DOTween.To(()=> cubeColorTemp, x=> cubeColorTemp =x, cubeCorrectColor, tweenDuration).OnUpdate(()=> cubeMaterial.SetColor("_Color", cubeColorTemp));
		DOTween.To(()=> cubeLerpTemp, x=> cubeLerpTemp =x, 1, tweenDuration).OnUpdate(()=> cubeMaterial.SetFloat("_Lerp", cubeLerpTemp));


		yield return new WaitForSeconds (timeBeforeNeutral);


		cubeColorTemp = cubeMaterial.GetColor("_Color");
		cubeLerpTemp = cubeMaterial.GetFloat ("_Lerp");

		DOTween.To(()=> cubeColorTemp, x=> cubeColorTemp =x, GlobalVariables.Instance.cubeNeutralColor, tweenDuration).OnUpdate(()=> cubeMaterial.SetColor("_Color", cubeColorTemp));
		DOTween.To(()=> cubeLerpTemp, x=> cubeLerpTemp =x, 0, tweenDuration).OnUpdate(()=> cubeMaterial.SetFloat("_Lerp", cubeLerpTemp));


		yield return new WaitForSeconds (timeBeforeColor);

		cubeCorrectColor = GlobalVariables.Instance.cubeColorplayer2;

		cubeColorTemp = cubeMaterial.GetColor("_Color");
		cubeLerpTemp = cubeMaterial.GetFloat ("_Lerp");

		DOTween.To(()=> cubeColorTemp, x=> cubeColorTemp =x, cubeCorrectColor, tweenDuration).OnUpdate(()=> cubeMaterial.SetColor("_Color", cubeColorTemp));
		DOTween.To(()=> cubeLerpTemp, x=> cubeLerpTemp =x, 1, tweenDuration).OnUpdate(()=> cubeMaterial.SetFloat("_Lerp", cubeLerpTemp));


		yield return new WaitForSeconds (timeBeforeNeutral);


		cubeColorTemp = cubeMaterial.GetColor("_Color");
		cubeLerpTemp = cubeMaterial.GetFloat ("_Lerp");

		DOTween.To(()=> cubeColorTemp, x=> cubeColorTemp =x, GlobalVariables.Instance.cubeNeutralColor, tweenDuration).OnUpdate(()=> cubeMaterial.SetColor("_Color", cubeColorTemp));
		DOTween.To(()=> cubeLerpTemp, x=> cubeLerpTemp =x, 0, tweenDuration).OnUpdate(()=> cubeMaterial.SetFloat("_Lerp", cubeLerpTemp));


		yield return new WaitForSeconds (timeBeforeColor);

		StartCoroutine (ColorTest ());
	}
}
