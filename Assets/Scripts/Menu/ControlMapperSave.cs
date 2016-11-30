using UnityEngine;
using System.Collections;
using Rewired;
using Rewired.UI.ControlMapper;

public class ControlMapperSave : MonoBehaviour 
{
	public ControlMapper controlMapper;

	void Awake ()
	{
		controlMapper.Close (true);
	}

	void OnEnable ()
	{
		controlMapper.Open ();

	}

	void OnDisable ()
	{
		controlMapper.Close (true);

		//ReInput.userDataStore.Save();
	}
}
