using System;

namespace Aogood.Foundation
{
    /// <summary>
    /// 单例模式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CSingleton<T> where T : new()
    {
        public static T Instance;
        static CSingleton()
        {
            if (Instance == null)
            {
                Instance = new T();
            }

        }
    }
}
