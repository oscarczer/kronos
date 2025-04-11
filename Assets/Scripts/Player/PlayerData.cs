using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerData : ScriptableObject
{
    private float remainingTime;
    public float RemainingTime
    {
        get => remainingTime;
        set => remainingTime = value;
    }

    private int maxJumps;
    public int MaxJumps
    {
        get => maxJumps;
        set => maxJumps = value;
    }

    private int maxDashes;
    public int MaxDashes
    {
        get => maxDashes;
        set => maxDashes = value;
    }

    private float dashDuration;
    public float DashDuration
    {
        get => dashDuration;
        set => dashDuration = value;
    }

    private float maxAttackCooldown;
    public float MaxAttackCooldown
    {
        get => maxAttackCooldown;
        set => maxAttackCooldown = value;
    }

    private float attackDamage;
    public float AttackDamage
    {
        get => attackDamage;
        set => attackDamage = value;
    }

    private float moveSpeed;
    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = value;
    }

    private float lifeStealConstant;
    public float LifeStealConstant
    {
        get => lifeStealConstant;
        set => lifeStealConstant = value;
    }
}
