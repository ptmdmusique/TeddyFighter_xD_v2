using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : MonoBehaviour {

	public UnityEvent onEnableEventList;					//Events that will be invoked when enable
	public UnityEvent onDisableEventList;					//Events that will be invoked when disable

	#region Default
	private void OnEnable()
	{
		onEnableEventList.Invoke();
	}
	private void OnDisable()
	{
		onDisableEventList.Invoke();
	}
	#endregion

	#region Preset Functions
	public void DestroyGameObject(GameObject destroyObject)
	{
		if (destroyObject != null) {
			Destroy(destroyObject);
		}
	}
	#endregion
}
