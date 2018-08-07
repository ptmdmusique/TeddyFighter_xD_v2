using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoonBoss : Boss {

	#region Action
	public override Transform SpawnMinion(int indx, Vector3 position)
	{
		Transform spawned = base.SpawnMinion(indx, position);
		spawned.gameObject.SetActive(true);

		//Scale down to 0 and scale up again
		Vector3 curScale = spawned.localScale;
		spawned.localScale = Vector3.zero;
		spawned.DOScale(curScale * Random.Range(0.5f, 1.5f), 1).SetEase(Ease.InOutBounce).
			OnComplete(() => {
				//Set active and the target of the orbit/follow script 
				OrbitAround script_2 = spawned.GetComponent<OrbitAround>();
				script_2.target = transform;
				script_2.enabled = true;

				minionSpawnedList.Add(spawned);                         //Add to the spawned list
			});
		return spawned;
	}
	#endregion
}
