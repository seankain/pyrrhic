using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Unity.Netcode;

public class ScorePanel : MonoBehaviour
{
    private PyrrhicGame gameInfo;
    public TextMeshProUGUI BootTeamInfoText;
    public TextMeshProUGUI StratTeamInfoText;
    public TextMeshProUGUI BootLivesText;
    public TextMeshProUGUI StratTicketsText;


    void Start()
    {
        gameInfo = FindObjectOfType<PyrrhicGame>();
        gameInfo.BootTeamInfo.OnValueChanged += (oldValue, newValue) =>
        {
            StringBuilder sb = new StringBuilder();
            var x = (TeamInfo)newValue;
            foreach (var y in x.Teammates)
            {
                sb.AppendLine(y.Name);
            }
            BootTeamInfoText.text = sb.ToString();
        };

    }

    public void Refresh()
    {
        gameInfo = FindObjectOfType<PyrrhicGame>();
        var bootTeamInfo = gameInfo.BootTeamInfo.Value;
        StringBuilder sb = new StringBuilder();
        foreach (var bootTeammate in bootTeamInfo.Teammates)
        {
            sb.AppendLine(bootTeammate.Name);
        }
        BootTeamInfoText.text = sb.ToString();
        sb.Clear();
        var stratTeamInfo = gameInfo.StrategistTeamInfo.Value;
        foreach (var stratTeammate in stratTeamInfo.Teammates)
        {
            sb.AppendLine(stratTeammate.Name);
        }
        StratTeamInfoText.text = sb.ToString();
        BootLivesText.text = $"{gameInfo.BootTeamLivesRemaining.Value} lives";
        StratTicketsText.text = $"{gameInfo.StrategistTickets.Value} tickets";


    }


}

