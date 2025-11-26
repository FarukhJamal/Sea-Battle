using System;

public class TurnManager
{
    public bool IsPlayerTurn { get; private set; } = true;

    public event Action<bool> OnTurnChanged; // param = isPlayerTurn

    public void StartFirstTurn(bool playerStarts = true)
    {
        IsPlayerTurn = playerStarts;
        OnTurnChanged?.Invoke(IsPlayerTurn);
    }

    public void KeepTurn()
    {
        // do not toggle; just notify
        OnTurnChanged?.Invoke(IsPlayerTurn);
    }

    public void SwitchTurn()
    {
        IsPlayerTurn = !IsPlayerTurn;
        OnTurnChanged?.Invoke(IsPlayerTurn);
    }
}
