public interface IAIController
{
    IGrid Grid { get; }
    void PlaceShips();
    (int x, int y) SelectAttackTarget(IGrid opponentGrid);
}
