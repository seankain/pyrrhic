using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CommandableCapability : UnitCapability
{
    public UnitCommandType CapabilityCommand;
    public Stack<UnitCommandBase> Commands = new Stack<UnitCommandBase>();

    protected override void OnStarting()
    {
    }
}

