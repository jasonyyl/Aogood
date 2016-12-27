using Aogood.SHLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{


    public class CModuleManager
    {
        Dictionary<EModuleType, IModule> m_ServerModules;
        public CModuleManager()
        {
            m_ServerModules = new Dictionary<EModuleType, IModule>();
        }
        public CNetworkMessage GetHandledMessage(CNetworkMessage msg)
        {

            if (m_ServerModules.ContainsKey((EModuleType)msg.MessageModuleId))
            {
                return m_ServerModules[(EModuleType)msg.MessageModuleId].MessageHandle(msg);
            }

            return null;
        }

        public void LoadModule(EModuleType moduletype)
        {
            if (!m_ServerModules.ContainsKey(moduletype))
                m_ServerModules.Add(moduletype, ModuleFactory(moduletype));
        }
        public void UnLoadModule(EModuleType moduletype)
        {
            if (m_ServerModules.ContainsKey(moduletype))
                m_ServerModules.Remove(moduletype);
        }

        IModule ModuleFactory(EModuleType moduleType)
        {
            switch (moduleType)
            {
                case EModuleType.LOGIN_SYSTEM:
                    return new CModuleLogin();
                default:
                    return null;

            }
        }
    }
}
