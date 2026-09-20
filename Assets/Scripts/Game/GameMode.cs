using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Pyrrhic/GameMode")]
public class GameMode : ScriptableObject
{
    public int BootLives = 10;
    public int StrategistTickets = 1000;
    public int GameDurationMinutes = 10;
}
