using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AForge;
using AForge.Neuro;

public class Agent : MonoBehaviour {

    float health = 1;
    Timer healthTimer;

    int sex;
    bool isReadyToMultiply = false;
    Timer multiplyTimer;
    public double[] chromosome;

    double[] input = { 0, 0 };
    ActivationNetwork network;

    Transform sensor1;
    Transform sensor2;
    List<Collider2D> sensoredFood = new List<Collider2D>();

    void UpdateHealth()
    {
        this.health -= 1;
    }

    void Multiply()
    {
        isReadyToMultiply = true;
        if(sex == 0)
        {
            for(int i = 0;i < 2;i++)
            {
                int gap = Random.Range(2, chromosome.Length - 2);
                GameObject egg = Instantiate(GameObject.Find("Manager").GetComponent<Manager>().eggPrefab) as GameObject;
                egg.GetComponent<Egg>().gapIndex = gap;
                egg.GetComponent<Egg>().chromosome = chromosome;
                egg.transform.position = transform.position;
                egg.transform.Translate(gap / 3, gap / 3, 0);
            }
        }
    }

	// Use this for initialization
	void Start () {
        healthTimer = new Timer(1f * 20);
        healthTimer.function = this.UpdateHealth;
        sex = Random.Range(0, 2);
        if(sex == 1)
        {
            this.GetComponent<SpriteRenderer>().color = new Color32(96, 169, 255, 255);
        }
        if (sex == 0)
        {
            this.GetComponent<SpriteRenderer>().color = new Color32(255, 131, 131, 255);
        }
        multiplyTimer = new Timer(1f * 18);
        multiplyTimer.function = this.Multiply;

        network = new ActivationNetwork(new Signum(), 2, 3, 2); // BipSigmoid, Sin, x^2!

        //chromosome = new double[network.Layers.Length * network.Layers[0].Neurons.Length * network.Layers[0].Neurons[0].Weights.Length];
        int index = 0;
        for (int i = 0; i < network.Layers.Length; i++)
        {
            for (int j = 0; j < network.Layers[i].Neurons.Length; j++)
            {
                for (int k = 0; k < network.Layers[i].Neurons[j].Weights.Length; k++)
                {
                    network.Layers[i].Neurons[j].Weights[k] = chromosome[index];
                    index++;
                }
            }
        }

        sensor1 = transform.FindChild("1");
        sensor2 = transform.FindChild("2");
        //! Debug.Log("Weight " + network.Layers[0].Neurons[0].Weights[0].ToString());
	
	}
	
	// Update is called once per frame
	void Update () {
        healthTimer.Update();
        if (this.health <= 0)
        {
            Destroy(this.gameObject);
            Debug.Log("DEATH!");
        }

        if (!isReadyToMultiply) multiplyTimer.Update();

        sensoredFood.RemoveAll(item => item == null);
        if(sensoredFood.Count > 0)
        {
            Collider2D closestFood = sensoredFood[0];
            for(int i = 1; i < sensoredFood.Count;i++)
                if(Vector2.Distance(transform.position,sensoredFood[i].transform.position) < Vector2.Distance(transform.position,sensoredFood[i-1].transform.position))
                {
                    closestFood = sensoredFood[i];
                }
            input[0] = Vector2.Distance(sensor1.transform.position, closestFood.transform.position);
            input[1] = Vector2.Distance(sensor2.transform.position, closestFood.transform.position);
        }
        if (sensoredFood.Count == 0)
        {
            input[0] = 8;
            input[1] = 8;
        }

        //if (sensoredFood.Count > 0 && sensoredFood[0] != null)
        //{
        //    Vector2 dir = sensoredFood[0].transform.position - transform.position;
        //    dir = sensoredFood[0].transform.InverseTransformDirection(dir);
        //    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        //    input[0] = Mathf.Cos(angle); // Mathf.Sin((angle * Mathf.PI) / 180);
        //}
        network.Compute(input);
        float z = 0;
        if (network.Output[0] > network.Output[1]) z = -1;
        if (network.Output[0] < network.Output[1]) z = 1;
        transform.Rotate(0, 0, z * 3, Space.Self);
        transform.Translate(0, 0.05f, 0);
        //! Debug.Log(network.Output[0] + " " + network.Output[1] + " " + input[0]); //network.Output[0] + " " + input[0]
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Food" || (other.gameObject.tag == "Egg" && sex == 1) && !sensoredFood.Contains(other) && other != null)
        {
            sensoredFood.Add(other);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other != null && other.gameObject.tag == "Food")
        {
            health += 1;
            sensoredFood.Remove(other.collider);
            Destroy(other.gameObject);
        }

        if (isReadyToMultiply && sex == 1 && other.gameObject.tag == "Egg")
        {
            double[] childChromosome = other.gameObject.GetComponent<Egg>().chromosome;
            int gap = other.gameObject.GetComponent<Egg>().gapIndex;
            for(int i = 0;i < gap;i++)
            {
                childChromosome[i] = chromosome[i];
            }
            GameObject child = Instantiate(GameObject.Find("Manager").GetComponent<Manager>().agentPrefab) as GameObject;
            child.transform.position = new Vector2(transform.position.x, transform.position.y);
            child.GetComponent<Agent>().chromosome = childChromosome;
            Destroy(other.gameObject);
            Debug.Log("New Child");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (sensoredFood.Contains(other))
        {
            sensoredFood.Remove(other);
        }
    }

    public class Signum : IActivationFunction
    {
        public double Function(double x)
        {
            return (double)Mathf.Pow((float)x, 2);
        }

        public double Derivative(double x)
        {
            return 0;
        }

        public double Derivative2(double y)
        {
            return 0;
        }
    }
}
