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
    public TextMeshProUGUI BootTeamInfo;


    void Start()
    {
        gameInfo = FindObjectOfType<PyrrhicGame>();
        gameInfo.BootTeamInfo.OnValueChanged += (oldValue, newValue) => 
        {
            StringBuilder sb = new StringBuilder();
            var x = (TeamInfo)newValue;
            foreach(var y in x.Teammates)
            {
                sb.Append(y.Name);
            }

        };

    }


}

