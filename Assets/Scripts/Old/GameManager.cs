// GameManager.cs
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ShipPlacer))]
public class GameManager : MonoBehaviour
{
    [Header("Grid")]
    public int gridWidth = 10;
    public int gridHeight = 10;

    [Header("Game rules")]
    public int shipPartsPerPlayer = 5; // total single-tile parts per player
    public float aiDelaySeconds = 0.6f;

    [Header("References (UI)")]
    public UIManager uiManager;

    // runtime models & services
    private GridModel playerGridModel;
    private GridModel aiGridModel;
    private GridCellView[,] playerViews;
    private GridCellView[,] aiViews;

    private ShipPlacer shipPlacer;
    private AttackController attackController;
    private TurnManager turnManager;
    private AIController aiController;

    // miss-streaks (tracked here to show result or use in logic if needed)
    private int playerMissStreak = 0;
    private int aiMissStreak = 0;

    private void Awake()
    {
        shipPlacer = GetComponent<ShipPlacer>();
        attackController = new AttackController();
        turnManager = new TurnManager();
    }

    private IEnumerator Start()
    {
        // build models
        playerGridModel = new GridModel(gridWidth, gridHeight);
        aiGridModel = new GridModel(gridWidth, gridHeight);

        // Build UI grids (UIManager holds two GridGenerator references)
        playerViews = uiManager.playerGridGenerator.Build(playerGridModel, true, OnCellClicked);
        aiViews = uiManager.aiGridGenerator.Build(aiGridModel, false, OnCellClicked);

        uiManager.PlayerViews = playerViews;
        uiManager.AIViews = aiViews;

        // initial UI
        uiManager.ShowPlacementUI();
        uiManager.UpdatePlacementCounter(shipPartsPerPlayer);

        // hook up AI
        aiController = new AIController(playerGridModel);

        yield return null;
    }

    // Called by UI when player clicks any cell (either grid). Clicks are routed here.
    private void OnCellClicked(int x, int y, bool isPlayerGrid)
    {
        // Placement phase: player can place until parts placed
        if (!turnManager.IsPlayerTurn && !turnManager.IsPlayerTurn) { } // no-op, keep consistent

        // If it's placement stage (we use shipPartsPerPlayer to decide)
        int placedCount = CountPlacedParts(playerGridModel);
        if (placedCount < shipPartsPerPlayer)
        {
            // only allow placing on player's grid
            if (!isPlayerGrid) return;
            var cell = playerGridModel.GetCell(x, y);
            if (!cell.HasShip)
            {
                cell.HasShip = true;
                uiManager.MarkPlayerShipVisible(x, y, true);
                placedCount++;
                uiManager.UpdatePlacementCounter(shipPartsPerPlayer - placedCount);
            }
            else
            {
                // allow toggling off
                cell.HasShip = false;
                uiManager.MarkPlayerShipVisible(x, y, false);
                placedCount--;
                uiManager.UpdatePlacementCounter(shipPartsPerPlayer - placedCount);
            }

            // if completed placement, auto-place AI and switch to battle
            if (placedCount >= shipPartsPerPlayer)
            {
                StartBattle();
            }

            return;
        }

        // Battle phase: player attacks AI grid only
        if (isPlayerGrid) return; // ignore clicks on player grid during battle
        if (!turnManager.IsPlayerTurn) return;

        var res = attackController.Attack(aiGridModel, x, y);
        if (res.AlreadyAttacked) return;

        if (res.Hit)
        {
            playerMissStreak = 0;
            uiManager.MarkAICellHit(x, y);
            if (aiGridModel.IsAllShipsSunk())
            {
                uiManager.ShowResult("YOU WIN");
                return;
            }
            // player keeps turn
            return;
        }
        else
        {
            playerMissStreak++;
            uiManager.MarkAICellMiss(x, y);
            if (playerMissStreak >= 3)
            {
                uiManager.ShowResult("YOU LOSE");
                return;
            }
            // switch to AI turn
            turnManager.SwitchTurn();
            StartCoroutine(AITurnRoutine());
        }
    }

    private IEnumerator AITurnRoutine()
    {
        yield return new WaitForSeconds(aiDelaySeconds);

        bool aiContinues = true;
        while (aiContinues)
        {
            var (tx, ty) = aiController.GetNextTarget();
            if (tx < 0) { aiContinues = false; break; }

            var res = attackController.Attack(playerGridModel, tx, ty);
            if (res.AlreadyAttacked) continue;

            if (res.Hit)
            {
                aiMissStreak = 0;
                uiManager.MarkPlayerCellHit(tx, ty);
                // allow AI to queue neighbors in hunting AI
                aiController.EnqueueNeighbors(tx, ty);

                if (playerGridModel.IsAllShipsSunk())
                {
                    uiManager.ShowResult("YOU LOSE");
                    yield break;
                }
                // AI keeps turn
                yield return new WaitForSeconds(0.15f);
                continue;
            }
            else
            {
                aiMissStreak++;
                uiManager.MarkPlayerCellMiss(tx, ty);
                if (aiMissStreak >= 3)
                {
                    uiManager.ShowResult("YOU WIN");
                    yield break;
                }
                aiContinues = false;
            }
        }

        // back to player
        turnManager.SwitchTurn();
    }

    private int CountPlacedParts(GridModel model)
    {
        int count = 0;
        for (int x = 0; x < model.Width; x++)
            for (int y = 0; y < model.Height; y++)
                if (model.GetCell(x, y).HasShip) count++;
        return count;
    }

    private void StartBattle()
    {
        // ensure AI has ships placed
        shipPlacer.PlaceRandomParts(aiGridModel, shipPartsPerPlayer);

        uiManager.ShowBattleUI();
        turnManager.StartFirstTurn(true); // player starts
    }
}
