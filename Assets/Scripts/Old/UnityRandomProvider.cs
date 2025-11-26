using UnityEngine;

public class UnityRandomProvider : IRandomProvider
{
    public int Range(int minInclusive, int maxExclusive) => Random.Range(minInclusive, maxExclusive);
    public float Value() => Random.value;
}
