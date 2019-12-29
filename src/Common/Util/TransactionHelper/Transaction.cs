using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransactionHelper
{
    public class Transaction
    {
        public static Transaction New()
        {
            return new Transaction();
        }

        private delegate Task WorkGetter(int id);

        private List<TransactionTask> _taskList;

        public Transaction()
        {
            _taskList = new List<TransactionTask>();
        }

        public async Task Run(bool throwException = true)
        {
            await runLogic((i) => _taskList[i].Work(), throwException);
        }

        public async Task RunNoOrder(bool throwException = true)
        {
            Task[] tasks = _taskList.Select((t) => t.Work())
                .ToArray();

            await runLogic((i) => tasks[i], throwException, true);
        }

        public Transaction Add(Func<Task> workTask, Func<Task> rollbackHandler = null, Action<Exception> exceptionHandler = null)
        {
            TransactionTask t = new TransactionTask(workTask, rollbackHandler, exceptionHandler);
            return Add(t);
        }

        public Transaction Add(TransactionTask task)
        {
            _taskList.Add(task);
            return this;
        }

        private async Task runLogic(WorkGetter workGetter, bool throwException = true, bool isRollbackAll = false)
        {
            Exception exception = null;
            bool needRollback = false;
            bool isRollback = false;
            for (int i = 0; i < _taskList.Count() && i >= 0;)
            {
                TransactionTask t = _taskList[i];

                if (isRollback)
                {
                    await t.Rollback();
                    i -= 1;
                }
                else
                {
                    try
                    {
                        await workGetter(i);
                        i += 1;

                        if (needRollback && i == _taskList.Count())
                        {
                            i -= 1;
                            isRollback = true;
                        }
                    }
                    catch (Exception e)
                    {
                        exception = e;

                        if (isRollbackAll && i < _taskList.Count() - 1)
                        {
                            needRollback = true;
                            i += 1;
                        }
                        else
                            isRollback = true;

                        t.Exception(e);
                    }
                }
            }

            if (throwException &&
                exception != null)
                throw exception;
        }

    }
}
