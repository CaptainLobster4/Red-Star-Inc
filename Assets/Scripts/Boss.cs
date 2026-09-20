using UnityEngine;

public enum BossType
{
    Comedy,
    Tragedy
}

public class Boss : MonoBehaviour
{
    public BossType bossType;

    [Tooltip("Seconds the player has to shoot out this boss's target before it destroys the player's target. No hard-set value yet - tune per boss/level in the Inspector.")]
    public float fightDuration = 30f;

    [Header("Collectible Reward")]
    public CollectibleData maskReward;

    [HideInInspector]
    public float timeRemaining;

    void Start()
    {
        timeRemaining = fightDuration;
    }

    // Called every frame by GameManager while this boss fight is active
    // (kept here rather than in Boss.Update() so GameManager stays the single
    // place that decides when the fight is running - see GameManager.Update)
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