using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Tag
{
    Player = 0,
    Ally = 1,
    Enemy = 2
}

static class StaticGlobal {
    //Player stuff
    public static Transform GetPlayer()
    {
        return GameObject.Find("Player").transform;
    }

    //Map value from one range to another
    static public float Map(float in_min, float in_max, float out_min, float out_max, float x)
    {
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }

    //Mouse stuff
    static public Vector3 GetMousePosition(Camera targetCamera)
    {
        Vector3 mouseOnScreen = targetCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
            Input.mousePosition.y,
            Input.mousePosition.z - targetCamera.transform.position.z));
        return mouseOnScreen;
    }

    //Camera stuff
    static public float xScreenClamp = 0.8f;
    static public float yScreenClamp = 0.8f;
    static public float GetCameraHeight(Camera targetCamera)
    {
        //Ortho size = height / 2
        return targetCamera.orthographicSize * 2;
    }
    static public float GetCameraWidth(Camera targetCamera)
    {
        //Aspect ratio = width / height
        float prefWidth = 1680;
        float prefHeight = 2000;
        return targetCamera.orthographicSize * 2.0f * prefWidth / prefHeight;
    }
    static public Vector2 GetCameraXBound(Camera targetCamera)
    {
        return new Vector2(targetCamera.transform.position.x - GetCameraWidth(targetCamera) / 2, targetCamera.transform.position.x + GetCameraWidth(targetCamera) / 2);
    }
    static public Vector2 GetCameraYBound(Camera targetCamera)
    {
        return new Vector2(targetCamera.transform.position.y - GetCameraHeight(targetCamera) / 2, targetCamera.transform.position.x + GetCameraHeight(targetCamera) / 2);
    }

    //Misc
    static public void ChangeIfNotEqual<T>(ref T other, T newValue, T constraint)   //Change the ref if newvalue != constraint
    {
        if (newValue.Equals(constraint) == false) {
            other = newValue;
        }
    }
    static public bool IsOutOfBound(Transform curTransform)
    {
        float halfCameraWidth = GetCameraWidth(Camera.main) / 2.0f;
        float halfCameraHeight = GetCameraHeight(Camera.main) / 2.0f;
        bool condition_1 = curTransform.position.x >= -halfCameraWidth;
        bool condition_2 = curTransform.position.x <= halfCameraWidth;
        bool condition_3 = curTransform.position.y >= -halfCameraHeight;
        bool condition_4 = curTransform.position.y <= halfCameraHeight;

        if (condition_1 == true && condition_2 == true && condition_3 == true && condition_4 == true) {
            return false;
        }

        return false;
    }
	static public double Map(double x, double in_min, double in_max, double out_min, double out_max)
	{
		return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
	}

}
