using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;


public class ManagerObsolete : MonoBehaviour {

    // INTERFACE VARIABLES
    public GameObject foodPrefab;
    public GameObject agentPrefab;
    public GameObject eggPrefab;

    const float X_RADIUS = 20f;
    const float Y_RADIUS = 10f;

    // UI REFERENCES
    public GameObject TextBlock;

    // LOGIC VARIABLES
    int generation = 0;
    public List<GameObject> agents = new List<GameObject>();
    const float generationLifeTime = 30;
    Timer generationLifeTimer;
    int chromosomeLength = 6 * 3 + 3 * 1;

    private void RefreshGeneration()
    {

    }

    void Start ()
    {
        for (int i = 0; i < 20;i++ )
        {
            GameObject firstAgent = Instantiate(agentPrefab) as GameObject;
            Debug.Log(firstAgent.GetComponent<Agent>().chromosome.Length);
            firstAgent.GetComponent<Agent>().chromosome = new double[chromosomeLength];
            for (int d = 0; d < chromosomeLength - 1; d++) firstAgent.GetComponent<Agent>().chromosome[d] = Random.Range(-1f, 1f);
            firstAgent.GetComponent<Agent>().FoodEaten = 4;
            agents.Add(firstAgent);
            
        }
        NewGeneration();
        generationLifeTimer = new Timer(generationLifeTime);
        generationLifeTimer.function = NewGeneration;
    }

	void NewGeneration () {
        foreach (GameObject agent in agents)
        {
            if (agent.GetComponent<Agent>().FoodEaten < 1) { agents.Remove(agent); Destroy(agent); }
        }
        List<double[]> generatedChromosomes = new List<double[]>();

        Shuffle(ref agents);
        for (int i = 0; i < agents.Count - (agents.Count % 2); i+=2)
        {
            // CROSSOVER
            for (int j = 0; j < 5; j++)
            {
                double[] buffer = new double[3 * 3 + 3 * 1];
                int gap = Random.Range(0, buffer.Length - 2);
                int p1 = j % 2;
                int p2 = (j + 1) % 2;
                System.Array.Copy(agents[i + p1].GetComponent<Agent>().chromosome, 0, agents[i + p2].GetComponent<Agent>().chromosome, 0, gap + 1);
                buffer = agents[i].GetComponent<Agent>().chromosome;
                generatedChromosomes.Add(buffer);
            }
        }

        foreach (GameObject agent in agents) Destroy(agent);
        agents = new List<GameObject>();

        int agentsCount = generatedChromosomes.Count;
        for (int i = 0; i < agentsCount; i++)
        {
            GameObject agent = Instantiate(agentPrefab) as GameObject;
            agent.transform.position = new Vector2(Random.Range(-X_RADIUS, X_RADIUS), Random.Range(-Y_RADIUS, Y_RADIUS));

            double[] buffer = new double[chromosomeLength];
            buffer = generatedChromosomes[i];
            agent.GetComponent<Agent>().chromosome = buffer;
            agents.Add(agent);
        }

        for (int i = 0; i < 30; i++)
        {
            GenerateFood();
        }

        generation += 1;
        TextBlock.GetComponent<Text>().text = "Generation: " + generation + " Count: " + agents.Count;
	}
	
	// Update is called once per frame
	void Update () {
        //food_timer.Update();
        generationLifeTimer.Update();
	}


    private void GenerateFood()
    {
        GameObject newObject = Instantiate(foodPrefab) as GameObject;
        newObject.transform.position = new Vector2(Random.Range(-X_RADIUS, X_RADIUS), Random.Range(-Y_RADIUS, Y_RADIUS));
    }


    public static void Shuffle<T>(ref List<T> list)
    {
        RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
        int n = list.Count;
        while (n > 1)
        {
            byte[] box = new byte[1];
            do provider.GetBytes(box);
            while (!(box[0] < n * (byte.MaxValue / n)));
            int k = (box[0] % n);
            n--;
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}


public class Timer {

    float timer = 0;
    float timer_max;
    bool isStarted = false;

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
