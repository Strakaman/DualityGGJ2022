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
    public static Color greenTeamColor = new Color(0.082f, 1f, 0);
    public static Color purpleTeamColor = new Color(0.769f, 0, 1f);
    public const string TEAM_KEY = "Team";
    public const string SCORE_KEY = "Score";
    public const string GREEN_SCORE_KEY = "GreenTeamScore";
    public const string PURPLE_SCORE_KEY = "PurpleTeamScore";
    public const string GREEN_TEAM = "GreenTeam";
    public const string PURPLE_TEAM = "PurpleTeam";
    public const string ENEMY_TEAM = "EnemyTeam";
    public static string CharacterHead = "CharacterHead";

    public const string WallLayer = "Wall";
    public const string PlayerLayer = "Player";
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
