using System;

public class Timer
{

    //If the Timer is running
    private bool isRunning;

    //If the timer will be stopped on trigger
    private bool stopOnTrigger;

    private float curTime;
    private float[] triggerTimes;

    public float CurTime { get { return curTime; } }
    public int Level { get; private set; }

    public event Action<int> Tick;

    public Timer(float[] triggerTimes, bool stopOnTrigger)
    {
        curTime = 0.0f;
        this.triggerTimes = triggerTimes;
    }

    ~Timer()
    {
        Tick = null;
    }

    public void Start()
    {
        isRunning = true;
    }

    public void Update(float deltaTime)
    {
        if (isRunning)
        {
            curTime += deltaTime;

            if (curTime > triggerTimes[Level])
            {
                isRunning = !stopOnTrigger;
                Level++;
                Tick(Level);
            }
        }
    }

    public void Stop()
    {
        isRunning = false;
    }

    public void Continue()
    {
        isRunning = true;
    }

    public void Restart()
    {
        isRunning = true;
        curTime = 0.0f;
    }

    public void ResetTriggerTime(float[] triggerTimes)
    {
        this.triggerTimes = triggerTimes;
    }
}