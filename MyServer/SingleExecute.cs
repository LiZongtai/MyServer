using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyServer
{
    // executing method
    public delegate void ExecuteDelegate();
    public class SingleExecute
    {
        private static object ob = new object();
        private static SingleExecute instance = null;
        public static SingleExecute Instance
        {
            get
            {
                lock (ob)
                {
                    if (instance == null)
                    {
                        instance = new SingleExecute();
                    }
                    return instance;
                }               
            }
        }
        private object objLock = new object();
        private Mutex mutex;
        public SingleExecute()
        {
            mutex = new Mutex();
        }
        // single execute logic
        public void Execute(ExecuteDelegate executeDelegate)
        {
            lock (objLock)
            {
                mutex.WaitOne();
                executeDelegate();
                mutex.ReleaseMutex();
            }
        }
    }
}
