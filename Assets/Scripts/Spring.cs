using UnityEngine;

public class Spring
{
    public float state;
    public float targetState;
    public float vel;
    public float strength;
    public float damping;

    public Spring(float state, float targetState, float strength, float damping)
    {
        Set(state, targetState, strength, damping);
    }

    public void Set(float state, float targetState, float strength, float damping)
    {
        this.state = state;
        this.targetState = targetState;
        this.strength = strength;
        this.damping = damping;
        vel = 0.0f;
    }

    public void Update()
    {
        bool linearSprings = false;
        if (linearSprings)
        {
            state = Mathf.MoveTowards(state, targetState, strength * Time.deltaTime * 0.05f);
        }
        else
        {
            vel += (targetState - state) * strength * Time.deltaTime;
            vel *= Mathf.Pow(damping, Time.deltaTime);
            state += vel * Time.deltaTime;
        }
    }
}