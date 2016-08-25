using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

public class Utils {

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

    public class Timer
    {

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

        public void Start()
        {
            isStarted = true;
        }

        public void Update()
        {
            if(isStarted)
                timer -= Time.deltaTime;
            if (timer < 0)
            {
                function();
                isStarted = false;
                timer = timer_max;
            }
        }
    }
}
