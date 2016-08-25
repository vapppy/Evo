using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

public class Manager : MonoBehaviour {

	void Start ()
    {
        for (int i = 0; i < 30; i++)
        {
            genomes.Add(new double[GENOME_LENGTH]);
        }

        foreach(double[] genome in genomes)
            for (int d = 0; d < GENOME_LENGTH - 1; d++) genome[d] = Random.Range(-1f, 1f);

        InitializeEnvironment();
	}

	void Update ()
    {
        if(timer != null)
            timer.Update();
	}

    public GameObject FoodPrefab;
    public GameObject AgentPrefab;

    const float X_RADIUS = 20f;
    const float Y_RADIUS = 10f;

    public GameObject TextBlock;

    private int _generation = 0;
    List<double[]> genomes = new List<double[]>();
    Utils.Timer timer;
    const int GENOME_LENGTH = 6 * 3 + 3 * 1;

    const float GENERATION_LIFETIME = 30;
    const int FOOD_COUNT = 60;
    const int FOOD_TO_SURVIVE = 2;

    private void InitializeEnvironment()
    {
        _generation += 1;
        TextBlock.GetComponent<Text>().text = "Generation: " + _generation + " Count: " + genomes.Count;
        GenerateFood();
        GeneratePopulation();
        timer = new Utils.Timer(GENERATION_LIFETIME);
        timer.function = DestroyEnvironment;
        timer.Start();
    }

    private void DestroyEnvironment()
    {
        Reproduce();
        DestroyFood();
        DestroyPopulation();
        InitializeEnvironment();
    }


    private void GenerateFood()
    {
        for (int i = 0; i < FOOD_COUNT; i++)
        {
            GameObject newObject = Instantiate(FoodPrefab) as GameObject;
            newObject.transform.position = new Vector2(Random.Range(-X_RADIUS, X_RADIUS), Random.Range(-Y_RADIUS, Y_RADIUS));
        }
    }

    private void GeneratePopulation()
    {
        foreach (double[] genome in genomes)
        {
            GameObject agent = Instantiate(AgentPrefab) as GameObject;
            agent.GetComponent<Agent>().chromosome = genome;
            agent.transform.position = new Vector2(Random.Range(-X_RADIUS, X_RADIUS), Random.Range(-Y_RADIUS, Y_RADIUS));
        }
    }

    private void Reproduce()
    {
        // CUT DEAD AGENTS
        GameObject[] agentsBuffer = GameObject.FindGameObjectsWithTag("Agent");
        List<GameObject> parents = new List<GameObject>();
        foreach(GameObject agent in agentsBuffer)
        {
            if (agent.GetComponent<Agent>().FoodEaten >= FOOD_TO_SURVIVE)
                parents.Add(agent);
        }

        // CROSSOVER
        Utils.Shuffle(ref parents);
        genomes.Clear();
        for (int i = 0; i < parents.Count - (parents.Count % 2); i += 2)
        {
            for (int j = 0; j < 5; j++)
            {
                double[] buffer = new double[3 * 3 + 3 * 1];
                int gap = Random.Range(0, buffer.Length - 2);
                int p1 = j % 2;
                int p2 = (j + 1) % 2;
                System.Array.Copy(parents[i + p1].GetComponent<Agent>().chromosome, 0, parents[i + p2].GetComponent<Agent>().chromosome, 0, gap + 1);
                buffer = parents[i].GetComponent<Agent>().chromosome;
                genomes.Add(buffer);
            }
        }
    }

    private void DestroyFood()
    {
        GameObject[] food = GameObject.FindGameObjectsWithTag("Food");
        foreach (GameObject obj in food)
            Destroy(obj);
    }

    private void DestroyPopulation()
    {
        GameObject[] agents = GameObject.FindGameObjectsWithTag("Agent");
        foreach (GameObject obj in agents)
            Destroy(obj);
    }
}
