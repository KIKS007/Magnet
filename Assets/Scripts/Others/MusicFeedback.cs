using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MusicFeedback : MonoBehaviour 
{
	[Header ("Settings")]
	public AudioSpectrum.LevelsType levelsType = AudioSpectrum.LevelsType.Mean;
	public int minFrequency;
	public int maxFrequency;

	[Header ("Feedback")]
	public Vector3 positionFeedback;
	public Vector3 scaleFeedback;

	private Vector3 initialPosition;
	private Vector3 initialScale;

	// Use this for initialization
	void Start () 
	{
		initialPosition = transform.localPosition;
		initialScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () 
	{
		PositionFeedback ();

		ScaleFeedback ();
	}


	void PositionFeedback ()
	{
		Vector3 position = transform.position;

		float percentage = 0;
		float p = 0;
		int frequencyCount = 0;

		switch(levelsType)
		{

		case AudioSpectrum.LevelsType.Basic:

			for(int i = minFrequency; i < maxFrequency; i++)
			{
				p += AudioSpectrum.Instance.LevelsNormalized [i];
				frequencyCount++;
			}

			percentage = p / frequencyCount;

			break;
		
		case AudioSpectrum.LevelsType.Peak:

			for(int i = minFrequency; i < maxFrequency; i++)
			{
				p += AudioSpectrum.Instance.PeakLevelsNormalized [i];
				frequencyCount++;
			}

			percentage = p / frequencyCount;

			break;
		
		case AudioSpectrum.LevelsType.Mean:

			for(int i = minFrequency; i < maxFrequency; i++)
			{
				p += AudioSpectrum.Instance.MeanLevelsNormalized [i];
				frequencyCount++;
			}

			percentage = p / frequencyCount;

			break;
		}

		position = new Vector3 (initialPosition.x + percentage * positionFeedback.x, 
			initialPosition.y + percentage * positionFeedback.y, 
			initialPosition.z + percentage * positionFeedback.z);

		transform.localPosition = position;
	}

	void ScaleFeedback ()
	{
		Vector3 scale = transform.localScale;

		float percentage = 0;
		float p = 0;
		int frequencyCount = 0;

		switch(levelsType)
		{

		case AudioSpectrum.LevelsType.Basic:

			for(int i = minFrequency; i < maxFrequency; i++)
			{
				p += AudioSpectrum.Instance.LevelsNormalized [i];
				frequencyCount++;
			}

			percentage = p / frequencyCount;

			break;

		case AudioSpectrum.LevelsType.Peak:

			for(int i = minFrequency; i < maxFrequency; i++)
			{
				p += AudioSpectrum.Instance.PeakLevelsNormalized [i];
				frequencyCount++;
			}

			percentage = p / frequencyCount;

			break;

		case AudioSpectrum.LevelsType.Mean:

			for(int i = minFrequency; i < maxFrequency; i++)
			{
				p += AudioSpectrum.Instance.MeanLevelsNormalized [i];
				frequencyCount++;
			}

			percentage = p / frequencyCount;

			break;
		}

		scale = new Vector3 (initialScale.x + percentage * scaleFeedback.x, 
			initialScale.y + percentage * scaleFeedback.y, 
			initialScale.z + percentage * scaleFeedback.z);

		transform.localScale = scale;
	}
}
