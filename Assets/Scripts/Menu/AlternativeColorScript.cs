using UnityEngine;
using System.Collections;
using Colorful;
using DG.Tweening;

public class AlternativeColorScript : MonoBehaviour 
{
	public float changeDuration = 1f;

	[Header ("Iluminati Gold")]
	public Vector3 channelMixerRed = new Vector3 (-2, 83, 123);
	public Vector3 channelMixerGreen = new Vector3 (61, 5, 98);
	public Vector3 channelMixerBlue = new Vector3 (104, -200, 149);
	public Vector3 channelMixerConstant = new Vector3 (0, 0, 7);

	private GameObject mainCamera;

	// Use this for initialization
	void Start () 
	{
		mainCamera = GameObject.FindGameObjectWithTag ("MainCamera");
	}

	public void IlluminatiGold ()
	{
		if(mainCamera.GetComponent <ChannelMixer>().Red.x != channelMixerRed.x)
		{
			DOTween.To (() => mainCamera.GetComponent <ChannelMixer>().Red.x, x => mainCamera.GetComponent <ChannelMixer>().Red.x = x, channelMixerRed.x, changeDuration).SetEase (Ease.OutQuad);
			DOTween.To (() => mainCamera.GetComponent <ChannelMixer>().Red.y, x => mainCamera.GetComponent <ChannelMixer>().Red.y = x, channelMixerRed.y, changeDuration).SetEase (Ease.OutQuad);
			DOTween.To (() => mainCamera.GetComponent <ChannelMixer>().Red.z, x => mainCamera.GetComponent <ChannelMixer>().Red.z = x, channelMixerRed.z, changeDuration).SetEase (Ease.OutQuad);

			DOTween.To (() => mainCamera.GetComponent <ChannelMixer>().Green.x, x => mainCamera.GetComponent <ChannelMixer>().Green.x = x, channelMixerGreen.x, changeDuration).SetEase (Ease.OutQuad);
			DOTween.To (() => mainCamera.GetComponent <ChannelMixer>().Green.y, x => mainCamera.GetComponent <ChannelMixer>().Green.y = x, channelMixerGreen.y, changeDuration).SetEase (Ease.OutQuad);
			DOTween.To (() => mainCamera.GetComponent <ChannelMixer>().Green.z, x => mainCamera.GetComponent <ChannelMixer>().Green.z = x, channelMixerGreen.z, changeDuration).SetEase (Ease.OutQuad);

			DOTween.To (() => mainCamera.GetComponent <ChannelMixer>().Blue.x, x => mainCamera.GetComponent <ChannelMixer>().Blue.x = x, channelMixerBlue.x, changeDuration).SetEase (Ease.OutQuad);
			DOTween.To (() => mainCamera.GetComponent <ChannelMixer>().Blue.y, x => mainCamera.GetComponent <ChannelMixer>().Blue.y = x, channelMixerBlue.y, changeDuration).SetEase (Ease.OutQuad);
			DOTween.To (() => mainCamera.GetComponent <ChannelMixer>().Blue.z, x => mainCamera.GetComponent <ChannelMixer>().Blue.z = x, channelMixerBlue.z, changeDuration).SetEase (Ease.OutQuad);

			DOTween.To (() => mainCamera.GetComponent <ChannelMixer>().Constant.x, x => mainCamera.GetComponent <ChannelMixer>().Constant.x = x, channelMixerConstant.x, changeDuration).SetEase (Ease.OutQuad);
			DOTween.To (() => mainCamera.GetComponent <ChannelMixer>().Constant.y, x => mainCamera.GetComponent <ChannelMixer>().Constant.y = x, channelMixerConstant.y, changeDuration).SetEase (Ease.OutQuad);
			DOTween.To (() => mainCamera.GetComponent <ChannelMixer>().Constant.z, x => mainCamera.GetComponent <ChannelMixer>().Constant.z = x, channelMixerConstant.z, changeDuration).SetEase (Ease.OutQuad);
		}
		else
		{
			DOTween.To (() => mainCamera.GetComponent <ChannelMixer>().Red.x, x => mainCamera.GetComponent <ChannelMixer>().Red.x = x, 100, changeDuration).SetEase (Ease.OutQuad);
			DOTween.To (() => mainCamera.GetComponent <ChannelMixer>().Red.y, x => mainCamera.GetComponent <ChannelMixer>().Red.y = x, 0, changeDuration).SetEase (Ease.OutQuad);
			DOTween.To (() => mainCamera.GetComponent <ChannelMixer>().Red.z, x => mainCamera.GetComponent <ChannelMixer>().Red.z = x, 0, changeDuration).SetEase (Ease.OutQuad);

			DOTween.To (() => mainCamera.GetComponent <ChannelMixer>().Green.x, x => mainCamera.GetComponent <ChannelMixer>().Green.x = x, 0, changeDuration).SetEase (Ease.OutQuad);
			DOTween.To (() => mainCamera.GetComponent <ChannelMixer>().Green.y, x => mainCamera.GetComponent <ChannelMixer>().Green.y = x, 100, changeDuration).SetEase (Ease.OutQuad);
			DOTween.To (() => mainCamera.GetComponent <ChannelMixer>().Green.z, x => mainCamera.GetComponent <ChannelMixer>().Green.z = x, 0, changeDuration).SetEase (Ease.OutQuad);

			DOTween.To (() => mainCamera.GetComponent <ChannelMixer>().Blue.x, x => mainCamera.GetComponent <ChannelMixer>().Blue.x = x, 0, changeDuration).SetEase (Ease.OutQuad);
			DOTween.To (() => mainCamera.GetComponent <ChannelMixer>().Blue.y, x => mainCamera.GetComponent <ChannelMixer>().Blue.y = x, 0, changeDuration).SetEase (Ease.OutQuad);
			DOTween.To (() => mainCamera.GetComponent <ChannelMixer>().Blue.z, x => mainCamera.GetComponent <ChannelMixer>().Blue.z = x, 100, changeDuration).SetEase (Ease.OutQuad);

			DOTween.To (() => mainCamera.GetComponent <ChannelMixer>().Constant.x, x => mainCamera.GetComponent <ChannelMixer>().Constant.x = x, 0, changeDuration).SetEase (Ease.OutQuad);
			DOTween.To (() => mainCamera.GetComponent <ChannelMixer>().Constant.y, x => mainCamera.GetComponent <ChannelMixer>().Constant.y = x, 0, changeDuration).SetEase (Ease.OutQuad);
			DOTween.To (() => mainCamera.GetComponent <ChannelMixer>().Constant.z, x => mainCamera.GetComponent <ChannelMixer>().Constant.z = x, 0, changeDuration).SetEase (Ease.OutQuad);
		}
	}
}
