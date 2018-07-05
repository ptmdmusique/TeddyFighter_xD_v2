using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpawn : MonoBehaviour {

    [Header("Spawn position info")]
    public Vector2 enemySpawnPos;
    public Vector2 allySpawnPos;
    [Header("Object list info")]
    public int spawnIndx = -1;
    public List<Transform> objectList;
    public Tag spawnTag = Tag.Enemy;
    public int numberOfObject = 1;

    [Header("Collector")]
    private Collector objectMaster;

    private void Awake()
    {
        objectMaster = GameObject.Find("Object_Collector").GetComponent<Collector>();
    }

    public void SpawnObject(int indx = -1)
    {
        Vector2 spawnPos = enemySpawnPos;
        if (spawnTag == Tag.Ally) {
            spawnPos = allySpawnPos;
        }

        float spawnLength = StaticGlobal.GetCameraWidth(Camera.main) * 0.9f;
        Vector2 curLocation = new Vector2(spawnPos.x - spawnLength / 2, spawnPos.y);
        spawnLength /= (float)(numberOfObject + 1);
        curLocation.x += spawnLength;   //Location of the first object   

        if (indx == -1) {
            int curIndx = 0;
            for(int indx2 = 0; indx2 < numberOfObject; indx2++) {
                GeneralObject spawnObject = Instantiate(objectList[curIndx++], curLocation, Quaternion.identity).GetComponent<GeneralObject>();
                curLocation.x += spawnLength;
                spawnObject.SetTag(spawnTag == Tag.Ally ? "Ally" : "Enemy");
                spawnObject.SetLayer(spawnTag == Tag.Ally ? "Ally" : "Enemy");

                objectMaster.AddChild(spawnObject.transform);
                if (curIndx >= objectList.Count) {
                    curIndx = 0;
                }
            }
        } else {
            for(int spawnIndx = 0; spawnIndx < numberOfObject; spawnIndx++) {
                GeneralObject spawnObject = Instantiate(objectList[indx], curLocation, Quaternion.identity).GetComponent<GeneralObject>();
                curLocation.x += spawnLength;
                spawnObject.SetTag(spawnTag == Tag.Ally ? "Ally" : "Enemy");
                spawnObject.SetLayer(spawnTag == Tag.Ally ? "Ally" : "Enemy");

                objectMaster.AddChild(spawnObject.transform);
            }
        }
    }
}
