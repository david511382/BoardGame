using System;
using System.Threading.Tasks;

namespace Util.Transaction
{
    public class TransactionTask
    {
        private Func<Task> _workTask;
        private Func<Task> _rollbackHandler;
        private Action<Exception> _exceptionHandler;

        public TransactionTask(Func<Task> workTask, Func<Task> rollbackHandler = null, Action<Exception> exceptionHandler = null)
        {
            _workTask = workTask;
            _rollbackHandler = rollbackHandler;
            _exceptionHandler = exceptionHandler;
        }

        public async Task Work()
        {
            await _workTask();
        }

        public async Task Rollback()
        {
            if (_rollbackHandler != null)
                await _rollbackHandler();
        }

        public void Exception(Exception e)
        {
            _exceptionHandler?.Invoke(e);
        }
    }
}
