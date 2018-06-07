using UnityEngine;
using UnityEditor;

public class TestObject : MonoBehaviour
{
    public GeneralObject myTarget;
    public float speed;
    public Vector2 direction;
    public float delay = 1;

    private Vector2 initialPos;

    private void Awake()
    {
        initialPos = myTarget.transform.position;    
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
}
