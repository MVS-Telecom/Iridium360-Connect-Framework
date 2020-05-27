using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rock.Threading
{
    /// <summary>
    /// Задача
    /// </summary>
    public interface IWork
    {
        string Name { get; }
        void Run();
    }

    /// <summary>
    /// Асинхронная задача
    /// </summary>
    public interface IAsyncWork
    {
        string Name { get; }
        void Run(Action onFinish);
    }


    /// <summary>
    /// 
    /// </summary>
    public abstract class BaseWorkContainer : IWork
    {
        public readonly string Name;
        public readonly StackTrace StackTrace;

        string IWork.Name => this.Name;

        public abstract void Run();

        public BaseWorkContainer(string name)
        {
            Name = name;
#if DEBUG
            StackTrace = new StackTrace();
#endif
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class WorkContainer : BaseWorkContainer
    {
        public readonly Action Work;

        public WorkContainer(Action work, string name) : base(name)
        {
            Work = work;
        }

        public override void Run()
        {
            Work.Invoke();
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class AWorkContainer : BaseWorkContainer
    {
        public readonly Func<Task> Work;

        public AWorkContainer(Func<Task> work, string name) : base(name)
        {
            Work = work;
        }

        public override void Run()
        {
            AutoResetEvent r = new AutoResetEvent(false);

            Task.Run(async () =>
            {
                try
                {
                    await Work.Invoke();
                }
                finally
                {
                    r.Set();
                }
            });

            r.WaitOne();
        }
    }



    /// <summary>
    /// 
    /// </summary>
    public class AsyncWorkContainer : BaseWorkContainer
    {
        public readonly IAsyncWork Work;

        public AsyncWorkContainer(IAsyncWork work) : base(work.Name)
        {
            Work = work;
        }

        public override void Run()
        {
            AutoResetEvent r = new AutoResetEvent(false);

            Work.Run(() =>
            {
                r.Set();
            });

            r.WaitOne();
        }
    }



    /// <summary>
    /// 
    /// </summary>
    public class ThreadWorker : IDisposable
    {
        readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        readonly object _locker = new object();
        readonly Thread _worker;
        public readonly ConcurrentQueue<IWork> _workQueue = new ConcurrentQueue<IWork>();

        /// <summary>
        /// Has any queued tasks?
        /// </summary>
        public bool HasWork
        {
            get
            {
                return Tasks > 0 || currentWork != null;
            }
        }

        /// <summary>
        /// Count of queued tasks
        /// </summary>
        public int Tasks
        {
            get
            {
                return _workQueue.Count;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SuperQueue{T}"/> class.
        /// </summary>
        /// <param name="workerCount">The worker count.</param>
        /// <param name="dequeueAction">The dequeue action.</param>
        public ThreadWorker(string name = null)
        {
            _worker = new Thread(Consume) { IsBackground = true, Name = $"SuperQueue worker `{name}`" };
            _worker.Start();

        }


        /// <summary>
        /// Enqueues the task.
        /// </summary>
        /// <param name="task">The task.</param>
        public virtual void PostDelayed(Action task, TimeSpan delay, string name)
        {
            Task.Run(async () =>
            {
                await Task.Delay(delay);
                Post(task, name);
            });
        }


        /// <summary>
        /// Enqueues the task.
        /// </summary>
        /// <param name="task">The task.</param>
        public virtual void PostDelayed(IAsyncWork runnable, TimeSpan delay)
        {
            Task.Run(async () =>
            {
                await Task.Delay(delay);
                Post(runnable);
            });
        }

        /// <summary>
        /// Enqueues the task.
        /// </summary>
        /// <param name="task">The task.</param>
        public virtual void Post(Func<Task> task, string name)
        {
            Post(new AWorkContainer(task, name));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual Task<T> PostAsync<T>(Func<Task<T>> task, string name)
        {
            return Task.Run(() =>
            {
                AutoResetEvent r = new AutoResetEvent(false);
                T result = default(T);

                Post(new AWorkContainer(async () =>
                {
                    result = await task();
                    r.Set();

                }, name));

                r.WaitOne();
                return result;
            });
        }


        /// <summary>
        /// Enqueues the task.
        /// </summary>
        /// <param name="task">The task.</param>
        public virtual void Post(Action task, string name)
        {
            Post(new WorkContainer(task, name));
        }

        /// <summary>
        /// Enqueues the task.
        /// </summary>
        /// <param name="runnable"></param>
        public virtual void Post(IAsyncWork runnable)
        {
            Post(new AsyncWorkContainer(runnable));
        }

        /// <summary>
        ///  Enqueues the task.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="skipIfAny">Skip task execution if any other task are queued</param>
        private async void Post(IWork task)
        {
            if (disposed)
                throw new InvalidOperationException("Worker is already disposed");

            await semaphore.WaitAsync();

            await Task.Run(() =>
            {
                try
                {
                    lock (_locker)
                    {
                        _workQueue.Enqueue(task);
                        Monitor.PulseAll(_locker);
                    }
                }
                finally
                {
                    semaphore.Release();
                }
            });
        }




        private IWork currentWork;
        private bool disposed = false;


        /// <summary>
        /// Consumes this instance.
        /// </summary>
        void Consume()
        {
            while (!disposed)
            {
                lock (_locker)
                {
                    while (_workQueue.Count == 0) Monitor.Wait(_locker);

                    try
                    {
                        bool ok = _workQueue.TryDequeue(out currentWork);

                        if (!ok)
                            throw new Exception("Try dequeue returns false");


                        currentWork.Run();
                    }
                    catch (Exception e)
                    {
#if DEBUG
                        Debugger.Break();
#endif
                    }
                    finally
                    {
                        currentWork = null;
                    }
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
#if DEBUG
            if (disposed)
                throw new InvalidOperationException("Worker is already disposed");
#endif

            disposed = true;
        }
    }
}
