using UnityEngine;
using System.Collections;

public class Cam : MonoBehaviour {

	void Start () {
	
	}

    const int ORTHOGRAPHIC_SIZE_MIN = 6;
    const int ORTHOGRAPHIC_SIZE_MAX = 18;
    const float SPEED = 14f;
	
	void Update () {
        // WHEEL UP/DOWN
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            transform.GetComponent<Camera>().orthographicSize--;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            transform.GetComponent<Camera>().orthographicSize++;
        }
        transform.GetComponent<Camera>().orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, ORTHOGRAPHIC_SIZE_MIN, ORTHOGRAPHIC_SIZE_MAX);

        // WASD
        if (Input.GetKey(KeyCode.A))
        {
            transform.position = new Vector3(transform.position.x - SPEED * Time.deltaTime, transform.position.y, -10);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position = new Vector3(transform.position.x + SPEED * Time.deltaTime, transform.position.y, -10);
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + SPEED * Time.deltaTime, -10);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - SPEED * Time.deltaTime, -10);
        }
	}
}
