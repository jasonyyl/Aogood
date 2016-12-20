using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aogood.Foundation
{
    sealed class TObjectCachePool<T> where T : class
    {
        #region Fields
        short m_cacheLimit;
        LinkedList<T> m_cachedObjects;
        #endregion

        #region Public
        public TObjectCachePool()
        {
            m_cacheLimit = 10;
            m_cachedObjects = new LinkedList<T>();
        }
        /// <summary>
        /// 设置缓存上限
        /// </summary>
        public void SetCacheLimit(short iLimit)
        {
            m_cacheLimit = iLimit;
        }

        /// <summary>
        /// 获取有效的对象
        /// </summary>
        /// <returns></returns>
        public T GetAvailableObject()
        {
            if (m_cachedObjects.Count == 0)
                return null;

            T obj = m_cachedObjects.First.Value;
            m_cachedObjects.RemoveFirst();
            return obj;
        }

        /// 回收对象，返回true，回收成功，返回false，到达上限，不再缓存
        /// </summary>
        public bool RecycleObject(T obj)
        {
            if (m_cachedObjects.Count >= m_cacheLimit)
                return false;

            m_cachedObjects.AddLast(obj);
            return true;
        }
        #endregion
    }
    sealed class CObjectPool
    {
        public int m_iType;
        public TObjectCachePool<object> m_objPool;

        public CObjectPool()
        {
            m_iType = 0;
            m_objPool = new TObjectCachePool<object>();
        }

    }
    public class CObjectCachePool
    {
        #region Fields
        List<CObjectPool> m_objPools;
        #endregion

        #region Public
        public CObjectCachePool()
        {
            m_objPools = new List<CObjectPool>();
        }

        public void Clear()
        {
            m_objPools.Clear();
        }

        public void SetCachePoolSize(int iType, short iLimit, bool bAddPoolIfNotExist = true)
        {
            CObjectPool pool = _GetObjectPool(iType, bAddPoolIfNotExist);
            if (pool == null)
                return;
            pool.m_objPool.SetCacheLimit(iLimit);
        }

        public void RecycleObject(int iType, object obj, bool bAddPoolIfNotExist = true)
        {
            if (obj == null)
                return;

            CObjectPool pool = _GetObjectPool(iType, bAddPoolIfNotExist);
            if (pool == null)
                return;
            if (obj is IRecycle)
                ((IRecycle)obj).Recycle();

            pool.m_objPool.RecycleObject(obj);
        }

        public T GetAvailableObject<T>(int iType) where T : class
        {
            CObjectPool pool = _GetObjectPool(iType, false);
            if (pool == null)
                return null;

            object obj = pool.m_objPool.GetAvailableObject();
            if (obj is IReusable)
                ((IReusable)obj).Reuse();
            return obj as T;
        }
        #endregion

        #region Funcs
        CObjectPool _GetObjectPool(int iType, bool bAddPoolIfNotExist)
        {
            foreach (CObjectPool pool in m_objPools)
            {
                if (pool.m_iType == iType)
                    return pool;
            }

            if (bAddPoolIfNotExist)
            {
                CObjectPool pool = new CObjectPool();
                pool.m_iType = iType;
                m_objPools.Add(pool);
                return pool;
            }

            return null;
        }
        #endregion

    }
}
