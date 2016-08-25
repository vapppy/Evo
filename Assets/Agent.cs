using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AForge;
using AForge.Neuro;

public class Agent : MonoBehaviour {

    public double[] chromosome;

    double[] input = { 0, -1, 0, 0, -1, 0 };
    ActivationNetwork network;

    List<Collider2D> sensoredFood = new List<Collider2D>();
    List<Collider2D> sensoredAgents = new List<Collider2D>();
    public int FoodEaten = 0;

	// Use this for initialization
	void Start () {
        this.GetComponent<SpriteRenderer>().color = new Color32(96, 169, 255, 255);

        network = new ActivationNetwork(new BipolarSigmoidFunction(), 6, 3, 1); // BipSigmoid, Sin, x^2!

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

        //! Debug.Log("Weight " + network.Layers[0].Neurons[0].Weights[0].ToString());
	
	}
	
	// Update is called once per frame
   
	void Update() 
    {
        Collider2D closestFood = null;
        sensoredFood.RemoveAll(item => item == null);
        if(sensoredFood.Count > 0)
        {
            closestFood = sensoredFood[0];
            for(int i = 1; i < sensoredFood.Count;i++)
                if(Vector2.Distance(transform.position,sensoredFood[i].transform.position) < Vector2.Distance(transform.position,sensoredFood[i-1].transform.position))
                {
                    closestFood = sensoredFood[i];
                }

            input[2] = 1;
        }
        if (sensoredFood.Count == 0)
        {
            input[0] = 0;
            input[1] = -1;
            input[2] = 0;
        }

        Collider2D closestAgent = null;
        sensoredAgents.RemoveAll(item => item == null);
        if (sensoredAgents.Count > 0)
        {
            closestAgent = sensoredAgents[0];
            for (int i = 1; i < sensoredAgents.Count; i++)
                if (Vector2.Distance(transform.position, sensoredAgents[i].transform.position) < Vector2.Distance(transform.position, sensoredAgents[i - 1].transform.position))
                {
                    closestAgent = sensoredAgents[i];
                }

            input[5] = 1;
        }
        if (sensoredAgents.Count == 0)
        {
            input[3] = 0;
            input[4] = -1;
            input[5] = 0;
        }

        //if (sensoredFood.Count > 0 && sensoredFood[0] != null)
        //{
        //    Vector2 dir = sensoredFood[0].transform.position - transform.position;
        //    dir = sensoredFood[0].transform.InverseTransformDirection(dir);
        //    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        //    input[0] = Mathf.Cos(angle); // Mathf.Sin((angle * Mathf.PI) / 180);
        //}
        float angle = 0;
        float distance = -1;
        if (closestFood != null)
        {
            Vector3 dir = closestFood.transform.position - transform.position;
            dir = closestFood.transform.InverseTransformDirection(dir);
            angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
            distance = Vector2.Distance(transform.position, closestFood.transform.position);
        }
 
        input[0] = angle;
        input[1] = distance;

        angle = 0;
        distance = -1;
        if (closestAgent != null)
        {
            Vector3 dir = closestAgent.transform.position - transform.position;
            dir = closestAgent.transform.InverseTransformDirection(dir);
            angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
            distance = Vector2.Distance(transform.position, closestAgent.transform.position);
        }

        input[3] = angle;
        input[4] = distance;

        network.Compute(input);

        transform.Rotate(0, 0, (float)network.Output[0] * 10);
        transform.Translate(0.05f, 0, 0);
        //! Debug.Log(network.Output[0] + " " + network.Output[1] + " " + input[0]); //network.Output[0] + " " + input[0]
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Food" && (other && !sensoredFood.Contains(other) && other != null))
        {
            sensoredFood.Add(other);
        }

        if (other.gameObject.tag == "Agent" && (other && !sensoredAgents.Contains(other) && other != null))
        {
            //sensoredAgents.Add(other);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other != null && other.gameObject.tag == "Food")
        {
            FoodEaten += 1;
            sensoredFood.Remove(other.collider);
            Destroy(other.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Food" && (other && !sensoredFood.Contains(other) && other != null))
        {
            sensoredFood.Remove(other);
        }

        if (other.gameObject.tag == "Agent" && (other && !sensoredAgents.Contains(other) && other != null))
        {
            //sensoredAgents.Remove(other);
        }
    }

    public class Signum : IActivationFunction
    {
        public double Function(double x)
        {
            return (double)Mathf.Sin((float)x);
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
