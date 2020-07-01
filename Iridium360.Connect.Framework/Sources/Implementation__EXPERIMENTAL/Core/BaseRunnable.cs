using System;
using System.Diagnostics;
using System.Threading.Tasks;


namespace Iridium360.Connect.Framework.Implementations
{
    /// <summary>
    /// 
    /// </summary>
    internal abstract class BaseRunnable : IAsyncWork
    {
        public virtual string Name => this.GetType().Name;


        private Action onFinish;
        private Action<Exception> onError;
        private ILogger logger;


        public BaseRunnable(ILogger logger, Action onFinish, Action<Exception> onError)
        {
            this.logger = logger;
            this.onFinish = onFinish;
            this.onError = onError;
        }


        async void IAsyncWork.Run(Action onFinish)
        {
            try
            {
                await InternalRun();
                this.onFinish?.Invoke();
            }
            catch (Exception e)
            {
                logger.Log($"[RUNNABLE] Exception occured while executing: {e}");
#if DEBUG
                Debugger.Break();
#endif
                this.onError?.Invoke(e);
            }
            finally
            {
                onFinish();
            }
        }


        protected abstract Task InternalRun();

    }
}
