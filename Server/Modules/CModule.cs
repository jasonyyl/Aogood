using Aogood.SHLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public interface IModule
    {
        EModuleType GetModuleType();
        CNetworkMessage MessageHandle(CNetworkMessage msg);
    }
    public abstract class CModule : IModule
    {
        public abstract EModuleType GetModuleType();
        public abstract CNetworkMessage MessageHandle(CNetworkMessage msg);
    }
}
