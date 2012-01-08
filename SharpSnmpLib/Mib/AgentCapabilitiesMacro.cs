using System;
using System.Collections.Generic;

namespace Lextm.SharpSnmpLib.Mib
{
    public class AgentCapabilitiesMacro : ISmiType, IEntity
    {
        public EntityStatus Status;
        public string Description;
        public string Reference;
        public IList<AgentCapabilitiesModule> Modules = new List<AgentCapabilitiesModule>();

        public AgentCapabilitiesMacro(string productRelease)
        {
            
        }

        [CLSCompliant(false)]
        public uint Value { get; set; }
        public string Parent { get; set; }
        public string ModuleName { get; set; }
        public string Name { get; set; }
    }
}