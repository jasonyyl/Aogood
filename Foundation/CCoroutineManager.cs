using System;
using System.Collections;
using System.Collections.Generic;

namespace Aogood.Foundation
{
    public class CCoroutine : IEnumerator
    {
        Stack<IEnumerator> executionStack;

        public CCoroutine(IEnumerator iterator)
        {
            this.executionStack = new Stack<IEnumerator>();
            this.executionStack.Push(iterator);
        }

        public bool MoveNext()
        {
            IEnumerator i = this.executionStack.Peek();

            if (i.MoveNext())
            {
                object result = i.Current;
                if (result != null && result is IEnumerator)
                {
                    this.executionStack.Push((IEnumerator)result);
                }

                return true;
            }
            else
            {
                if (this.executionStack.Count > 1)
                {
                    this.executionStack.Pop();
                    return true;
                }
            }

            return false;
        }

        public void Reset()
        {
            throw new System.NotSupportedException("This Operation Is Not Supported.");
        }

        public object Current
        {
            get { return this.executionStack.Peek().Current; }
        }
        public bool Find(IEnumerator iterator)
        {
            return this.executionStack.Contains(iterator);
        }
    }
    public static class CCoroutineManager
    {
        static List<CCoroutine> m_CoroutineList;
        static List<IEnumerator> m_Buffer;
        static Action m_UpdateEvent;
        static System.Timers.Timer m_TimerUpdate;
        static CCoroutineManager()
        {
            if (m_TimerUpdate == null)
                m_TimerUpdate = new System.Timers.Timer();
            m_TimerUpdate.Interval = 1.0 / 60.0;
            m_TimerUpdate.Elapsed += UpdateRate;
            m_TimerUpdate.Start();
        }    
        static bool Find(IEnumerator iterator)
        {
            // If this iterator is already added
            // Then ignore it this time
            foreach (CCoroutine editorCoroutine in m_CoroutineList)
            {
                if (editorCoroutine.Find(iterator))
                {
                    return true;
                }
            }
            return false;
        }
        static void Update()
        {
            m_CoroutineList.RemoveAll
            (
            coroutine => { return coroutine.MoveNext() == false; }
            );

            // If we have iterators in buffer
            if (m_Buffer.Count > 0)
            {
                foreach (IEnumerator iterator in m_Buffer)
                {
                    // If this iterators not exists
                    if (!Find(iterator))
                    {
                        // Added this as new EditorCoroutine
                        m_CoroutineList.Add(new CCoroutine(iterator));
                    }
                }

                // Clear buffer
                m_Buffer.Clear();
            }

            // If we have no running Coroutine
            // Stop calling update anymore
            if (m_CoroutineList.Count == 0)
            {
                m_UpdateEvent -= Update;
            }
        }
        static void UpdateRate(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (m_UpdateEvent != null)
            {
                m_UpdateEvent();
            }
        }
        public static void Stop()
        {
            m_TimerUpdate.Stop();
            m_TimerUpdate.Dispose();
        }
        public static IEnumerator StartCoroutine(IEnumerator iterator)
        {
            if (m_CoroutineList == null)
            {
                m_CoroutineList = new List<CCoroutine>();
            }
            if (m_Buffer == null)
            {
                m_Buffer = new List<IEnumerator>();
            }
            if (m_CoroutineList.Count == 0)
            {
                m_UpdateEvent += Update;
            }
            // add iterator to buffer first
            m_Buffer.Add(iterator);

            return iterator;
        }
    }
}
