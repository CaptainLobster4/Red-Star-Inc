using UnityEngine;

public enum BossType
{
    Comedy,
    Tragedy
}

public class Boss : MonoBehaviour
{
    public BossType bossType;
    //Ask Darish how long this should be
    [Tooltip("Ask Darish how long this should be")]
    public float fightDuration = 30f;

    [HideInInspector]
    public float timeRemaining;

    void Start()
    {
        timeRemaining = fightDuration;
    }
    public void TickTimer(float deltaTime)
    {
        timeRemaining -= deltaTime;
    }

    public bool TimeExpired()
    {
        return timeRemaining <= 0f;
    }

    public void ResetFight()
    {
        timeRemaining = fightDuration;
    }
}