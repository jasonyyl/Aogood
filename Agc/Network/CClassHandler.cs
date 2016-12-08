using System;
using System.Collections.Generic;
using System.Reflection;
using Aogood.Foundation;

namespace Aogood.Network
{
    public class CClassHandler : CSingleton<CClassHandler>
    {
        /// <summary>
        /// 获取所有的子类
        /// </summary>
        /// <param name="parentType"></param>
        /// <returns></returns>
        public Type[] GetInheritType(Type parentType)
        {
            try
            {
                List<Type> lstType = new List<Type>();
                Assembly assem = Assembly.GetAssembly(parentType);
                foreach (Type tChild in assem.GetTypes())
                {
                    if (tChild.BaseType == parentType)
                    {
                        lstType.Add(tChild);
                    }

                }
                return lstType.ToArray();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
