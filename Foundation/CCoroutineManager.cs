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
        static List<CCoroutine> editorCoroutineList;
        static List<IEnumerator> buffer;

        public static IEnumerator StartCoroutine(IEnumerator iterator)
        {
            if (editorCoroutineList == null)
            {
                editorCoroutineList = new List<CCoroutine>();
            }
            if (buffer == null)
            {
                buffer = new List<IEnumerator>();
            }
            if (editorCoroutineList.Count == 0)
            {
                ClientApp.updateEvent += Update;
            }
            // add iterator to buffer first
            buffer.Add(iterator);

            return iterator;
        }
        static bool Find(IEnumerator iterator)
        {
            // If this iterator is already added
            // Then ignore it this time
            foreach (CCoroutine editorCoroutine in editorCoroutineList)
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
            editorCoroutineList.RemoveAll
            (
            coroutine => { return coroutine.MoveNext() == false; }
            );

            // If we have iterators in buffer
            if (buffer.Count > 0)
            {
                foreach (IEnumerator iterator in buffer)
                {
                    // If this iterators not exists
                    if (!Find(iterator))
                    {
                        // Added this as new EditorCoroutine
                        editorCoroutineList.Add(new CCoroutine(iterator));
                    }
                }

                // Clear buffer
                buffer.Clear();
            }

            // If we have no running Coroutine
            // Stop calling update anymore
            if (editorCoroutineList.Count == 0)
            {
                ClientApp.updateEvent -= Update;
            }
        }
    }

    public static class ClientApp
    {
        public static Action updateEvent;
        static System.Timers.Timer t;
        public static void Start()
        {
            if (t == null)
                t = new System.Timers.Timer();
            t.Interval = 1.0 / 60.0;
            t.Elapsed += UpdateRate;
            t.Start();
        }
        public static void Stop()
        {
            t.Stop();
            t.Dispose();
        }
        static void UpdateRate(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (updateEvent != null)
            {
                updateEvent();
            }
        }
    }
}
