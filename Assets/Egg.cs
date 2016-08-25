using UnityEngine;
using System.Collections;

public class Egg : MonoBehaviour {
   public Transform other;

   public void Start()
    {
        Vector3 dir = other.transform.position - transform.position;
        dir = other.transform.InverseTransformDirection(dir);
        float angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        if (angle < 0) angle += 360;
        Debug.Log("Angle" + angle);
    }
}
