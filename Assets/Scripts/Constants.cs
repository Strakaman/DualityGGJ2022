using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public static class Constants
{
    public static string sessionName = "SessionName";
    public static string playerName = "PlayerName";
    public static Color greenTeamColor = new Color(26, 255, 26);
    public static Color purpleTeamColor = new Color(182, 105, 255);
    public const string TEAM = "Team";
    public const string GREEN_TEAM = "GreenTeam";
    public const string PURPLE_TEAM = "PurpleTeam";
    public const string ENEMY_TEAM = "EnemyTeam";
    public const string GREEN_SCORE = "GreenTeamScore";
    public const string PURPLE_SCORE = "PurpleTeamScore";
    public static string CharacterHead = "CharacterHead";
}

public enum Shape
{
    Block = 0,
    Cone = 1,
    Radio = 2,
    Star = 3,
    Round = 4,
    Random = 999,
}
