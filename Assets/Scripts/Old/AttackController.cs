public class AttackController : IAttackService
{
    // Attack service works directly on GridModel instances.
    public AttackResult Attack(GridModel targetGrid, int x, int y)
    {
        var cell = targetGrid.GetCell(x, y);
        if (cell.Attacked)
            return new AttackResult { Hit = cell.HasShip, AlreadyAttacked = true };

        cell.Attacked = true;
        return new AttackResult { Hit = cell.HasShip, AlreadyAttacked = false };
    }
}
