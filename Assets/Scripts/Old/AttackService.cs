/*public class AttackService : IAttackService
{
    public AttackResult Attack(IGrid targetGrid, int x, int y)
    {
        var cell = targetGrid.GetCell(x, y);
        if (cell.Attacked)
            return new AttackResult { Hit = cell.HasShip, AlreadyAttacked = true };

        cell.Attacked = true;
        return new AttackResult { Hit = cell.HasShip, AlreadyAttacked = false };
    }
}
*/