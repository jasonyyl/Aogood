using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aogood.Foundation
{
    public class CAogoodFactory : CSingleton<CAogoodFactory>
    {
        #region Field
        CObjectCachePool m_objPools;
        #endregion

        #region Func
        public CAogoodFactory()
        {
            m_objPools = new CObjectCachePool();
        }

        public T GetObject<T>() where T : class, new()
        {
            T t = m_objPools.GetAvailableObject<T>(typeof(T).GetHashCode());
            if (t == null)
                t = new T();
            return t;
        }

        public void RecycleObject<T>(T obj) where T : class, IRecycle
        {
            if (obj == null)
                return;
            m_objPools.RecycleObject(obj.GetType().GetHashCode(), obj);
        }

        public void RecycleObject<T>(ref T obj) where T : class, IRecycle
        {
            RecycleObject(obj);
            obj = null;
        }
        #endregion
    }
}
