using System;
using System.Collections.Generic;

namespace Aogood.Foundation
{
    /// <summary>
    /// 利用享元模式创建实例
    /// </summary>
    public class CFactoryCreateInstance
    {
        public static CFactoryCreateInstance Instance;
        private Dictionary<Type, object> instanceList = new Dictionary<Type, object>();

        static CFactoryCreateInstance()
        {
            if (Instance == null)
            {
                Instance = new CFactoryCreateInstance();
            }
        }
        public T CreateInstance<T>() where T : class
        {
            Type type = typeof(T);
            if (instanceList.ContainsKey(type))
            {
                object obj = instanceList[type];
                return obj as T;
            }
            else
            {
                object obj = Activator.CreateInstance(type);
                instanceList.Add(typeof(T), obj);
                return obj as T;
            }
        }
    }
}
