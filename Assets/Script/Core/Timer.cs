using System;
using UnityEngine;

public class Timer
{

    //If the Timer is running
    private bool isRunning;

    //If the timer will be stopped on trigger
    private bool stopOnTrigger;

    private float onceTime;
    private float sumTime;
    private float[] triggerTimes;

    public float SumTime { get { return sumTime; } }
    public int Level { get; private set; }

    public Action onStart;
    public Action<float, float> onStop;
    public event Action<int> Tick;

    public Timer(float[] triggerTimes, bool stopOnTrigger)
    {
        sumTime = 0.0f;
        onceTime = 0;
        this.triggerTimes = triggerTimes;
    }

    ~Timer()
    {
        onStart = null;
        onStop = null;
        Tick = null;
    }

    public void Start()
    {
        if (isRunning)
            return;

        //if (sumTime > 0)
        //    Debug.LogWarning("The Timer has been used before calling 'Start' method, do you mean 'Continue' ?");

        onceTime = 0;
        isRunning = true;
        if (onStart != null)
            onStart();
    }

    public void Update(float deltaTime)
    {
        if (isRunning)
        {
            sumTime += deltaTime;
            onceTime += deltaTime;

            if (sumTime > triggerTimes[Level])
            {
                isRunning = !stopOnTrigger;
                Level++;

                if (Tick != null)
                    Tick(Level);
            }
        }
    }

    public void Stop()
    {
        if (!isRunning)
            return;

        isRunning = false;
        if (onStop != null)
        {
            onStop(onceTime, sumTime);
            Debug.Log("onTimerStop");
        }
    }

    //public void Continue()
    //{
    //    isRunning = true;
    //}

    public void Restart()
    {
        isRunning = true;
        sumTime = 0.0f;
        onceTime = 0;

        if (onStart != null)
            onStart();
    }

    public void ResetTriggerTime(float[] triggerTimes)
    {
        this.triggerTimes = triggerTimes;
    }
}