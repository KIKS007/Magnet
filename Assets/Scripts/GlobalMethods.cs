using UnityEngine;
using System.Collections;
using DG.Tweening;

public class GlobalMethods : Singleton<GlobalMethods> 
{
	public IEnumerator RandomPositionMovables (bool instantly)
	{
		GameObject[] allMovables = GameObject.FindGameObjectsWithTag ("Movable");
		Vector3[] allScales = new Vector3[allMovables.Length];
		LayerMask layer = (1 << 9) | (1 << 12) | (1 << 13) | (1 << 14);

		for(int i = 0; i < allMovables.Length; i++)
		{
			allScales [i] = allMovables [i].transform.lossyScale;
			allMovables [i].transform.localScale = new Vector3 (0, 0, 0);
		}
			

		for(int i = 0; i < allMovables.Length; i++)
		{
			Vector3 newPos = new Vector3 ();

			do
			{
				newPos = new Vector3(Random.Range(-24, 24 + 1), 3, Random.Range(-14, 14 + 1));
			}
			while(Physics.CheckSphere(newPos, 3, layer));

			if (!instantly)
				yield return new WaitForSeconds (0.1f);

			allMovables [i].transform.DOScale (allScales[i], 0.8f).SetEase (Ease.OutElastic);

			allMovables [i].transform.position = newPos;

			yield return null;
		}
	}
}
