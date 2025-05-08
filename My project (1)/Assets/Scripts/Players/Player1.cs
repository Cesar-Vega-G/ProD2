using UnityEngine;
using UnityEngine.InputSystem;


public class Player1Controller : BasePlayerController
{
    protected override Key GetLeftKey() => Key.A;
    protected override Key GetRightKey() => Key.D;
    protected override Key GetJumpKey() => Key.W;
    protected override Key GetAttackKey() => Key.E;

    protected override string GetOpponentTag() => "Player 2";
    protected override System.Type GetOpponentType() => typeof(Player2Controller);
}



