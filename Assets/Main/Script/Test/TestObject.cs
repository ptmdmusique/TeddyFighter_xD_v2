using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestObject : MonoBehaviour
{
    public GeneralObject myTarget;
    private string targetName;
    public float speed;
    public Vector2 direction;
    public float delay = 1;
    public string sceneName = "Test";
    public float damagePercentage = 10;

    private static TestObject _instance;

    private void Awake()
    {
        if (_instance == null) {
            targetName = myTarget.name;
            _instance = GameObject.FindObjectOfType<TestObject>();
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
    
    public void MoveObject()
    {
        Debug.Log("Move!");
        myTarget.Move(direction, speed);
    }
    public void ResetObject()
    {
        Debug.Log("Reset!");
        StartCoroutine(myTarget.ResetObject(delay));
    }
    public void TestDie()
    {
        Debug.Log("Target killed!");
        myTarget.Die();
    }
    public void RestartScene()
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        StartCoroutine(RestartScene(0.1f));
    }
    private IEnumerator RestartScene(float delay)
    {
        yield return new WaitForSeconds(delay);
        myTarget = GameObject.Find(targetName).GetComponent<GeneralObject>();
    }
    public void DamageObjet()
    {
        myTarget.ChangeHealth(-damagePercentage, 1);
    }
}
