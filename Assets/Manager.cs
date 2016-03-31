using UnityEngine;
using System.Collections;

public class Manager : MonoBehaviour {

    public GameObject foodPrefab;
    public GameObject agentPrefab;
    public GameObject eggPrefab;

    const float X_RADIUS = 40f;
    const float Y_RADIUS = 20f;

    Timer food_timer;
	// Use this for initialization
	void Start () {
        food_timer = new Timer(0.1f);
        food_timer.function = GenerateFood;

        for (int i = 0; i < 300; i++)
        {
            GameObject agent = Instantiate(agentPrefab) as GameObject;
            agent.transform.position = new Vector2(Random.Range(-X_RADIUS, X_RADIUS), Random.Range(-Y_RADIUS, Y_RADIUS));
            double[] buffer = new double[3 * 2 + 2 * 3];
            for (int d = 0; d < buffer.Length;d++) buffer[d] = Random.Range(-1f, 1f);
            agent.GetComponent<Agent>().chromosome = buffer;
        }
	}
	
	// Update is called once per frame
	void Update () {
        food_timer.Update();
	}


    private void GenerateFood()
    {
        GameObject newObject = Instantiate(foodPrefab) as GameObject;
        newObject.transform.position = new Vector2(Random.Range(-X_RADIUS, X_RADIUS), Random.Range(-Y_RADIUS, Y_RADIUS));
    }
}


public class Timer {

    float timer = 0;
    float timer_max;

    public delegate void Function();
    public Function function;

    public Timer(float interval)
    {
        timer_max = interval;
        timer = timer_max;
    }

    public void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            function();
            timer = timer_max;
        }
    }
}
