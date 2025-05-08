using UnityEngine;
using UnityEngine.InputSystem;


public class Player2Controller : BasePlayerController
{
    protected override Key GetLeftKey() => Key.J;
    protected override Key GetRightKey() => Key.L;
    protected override Key GetJumpKey() => Key.I;
    protected override Key GetAttackKey() => Key.O;
    protected override string GetOpponentTag() => "Player 1";
    protected override System.Type GetOpponentType() => typeof(Player1Controller);
}


