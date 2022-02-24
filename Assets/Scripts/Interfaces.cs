using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILogicManager {
    /// <summary>
    /// This event is to be invoked every time the game state changes.
    /// </summary>
	event LogicManager.GameStateChangeEventHandler GameStateChange;
    /// <summary>
    /// This event is to be invoked every time something changes in the game that would require a redraw of the visual representation.
    /// </summary>
	event LogicManager.GameUpdateEventHandler GameUpdate;
    /// <summary>
    /// Returns the size of the grid.
    /// </summary>
    Vector2Int GridSize { get; }
    /// <summary>
    /// Returns the 'board', the int represents the color of the tile, 0 is empty and 1 to 7 are colors.
    /// </summary>
    int[,] FixedPieces { get; }
    /// <summary>
    /// Returns the current active piece
    /// </summary>
    int[,,] ActivePiece { get; }
    /// <summary>
    /// Returns the position of the active piece.
    /// </summary>
    Vector2Int ActivePiecePosition { get; }
    /// <summary>
    /// Returns the rotation index of the current active piece.
    /// </summary>
    int ActivePieceRotation { get; }
    /// <summary>
    /// Returns the current game state.
    /// </summary>
    LogicManager.GameState CurrentGameState { get; }
    /// <summary>
    /// Returns the level the game is currently on, higher level should mean faster automatic soft drops.
    /// </summary>
    int Level { get; }
    /// <summary>
    /// Returns player score.
    /// </summary>
    int Score { get; }
    /// <summary>
    /// This is used to get the position if the active piece would be hard dropped (useful for the ghost piece).
    /// </summary>
    /// <returns>Position of the active piece if it would be hard dropped.</returns>
    Vector2Int GetActivePieceHardDropPosition();
    /// <summary>
    /// Get the next piece in the tetris bag (useful for the 'next piece preview').
    /// </summary>
    /// <returns>The next piece in the bag.</returns>
    int[,,] GetNextPieceInBag();
}