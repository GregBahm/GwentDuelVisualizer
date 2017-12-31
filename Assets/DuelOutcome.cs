using UnityEngine;
public class DuelOutcome
{
    public readonly int FirstTargetHealth;
    public readonly int SecondTargetHealth;
    public readonly int RemainingHealth;
    public readonly int Damage;
    public readonly int Bonus;
    public readonly bool FirstSurvives;

    public DuelOutcome(int firstTargetHealth,
        int secondTargetHealth,
        int remainingHealth,
        bool firstSurvives)
    {
        FirstTargetHealth = firstTargetHealth;
        SecondTargetHealth = secondTargetHealth;
        RemainingHealth = remainingHealth;
        FirstSurvives = firstSurvives;
        Damage = firstTargetHealth + secondTargetHealth - remainingHealth;
        Bonus = Damage - Mathf.Min(firstTargetHealth, secondTargetHealth);
    }
}