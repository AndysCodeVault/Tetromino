using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    public static Dictionary<ControlType, string> ControlTypeNames = new Dictionary<ControlType, string> {
        { ControlType.WASD, "WASD" },
        { ControlType.Arrow, "Arrow" },
        { ControlType.Gamepad1, "Gamepad1" },
        { ControlType.Gamepad2, "Gamepad2" },
        { ControlType.Gamepad3, "Gamepad3" },
        { ControlType.Gamepad4, "Gamepad4" },
    };
    public static PlayerMode mode = PlayerMode.Single;
    public static ControlType player1Controls = ControlType.Gamepad1;
    public static ControlType player2Controls = ControlType.Gamepad2;
    public static ControlType player3Controls = ControlType.Gamepad3;
    public static ControlType player4Controls = ControlType.Gamepad4;

    public static string GetControlType(int playerNumber)
    {
        if (playerNumber == 1)
        {
            return ControlTypeNames[player2Controls];
        }

        if (playerNumber == 2)
        {
            return ControlTypeNames[player3Controls];
        }

        if (playerNumber == 3)
        {
            return ControlTypeNames[player4Controls];
        }

        return ControlTypeNames[player1Controls];
    }
}

public enum PlayerMode { Single, TwoPlayers, ThreePlayers, FourPlayers }
public enum ControlType { WASD, Arrow, Gamepad1, Gamepad2, Gamepad3, Gamepad4 }




