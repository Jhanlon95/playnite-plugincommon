﻿using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CommonPluginsShared
{
    public class SystemTask
    {
        public CancellationTokenSource tokenSource { get; set; }
        public Task task { get; set; }
    }


    public class TaskHelper
    {
        private static readonly ILogger logger = LogManager.GetLogger();

        private List<SystemTask> SystemTask = new List<SystemTask>();


        public void Add(Task task, CancellationTokenSource tokenSource)
        {
            SystemTask.Add(new SystemTask { task = task, tokenSource = tokenSource });
        }

        public void Check()
        {
            try
            {
                List<SystemTask> TaskDelete = new List<SystemTask>();

#if DEBUG
                logger.Debug($"CommonPluginsShared [Ignored] - SystemTask {SystemTask.Count}");
#endif

                // Check task status
                foreach (var taskRunning in SystemTask)
                {
                    if (taskRunning.task.Status != TaskStatus.RanToCompletion)
                    {
#if DEBUG
                        logger.Debug($"CommonPluginsShared [Ignored] - Task {taskRunning.task.Id} ({taskRunning.task.Status}) is canceled");
#endif
                        // Cancel task if not terminated
                        taskRunning.tokenSource.Cancel();
                    }
                    else
                    {
                        // Add for delete
                        TaskDelete.Add(taskRunning);
                    }
                }

                // Delete tasks
                foreach (var taskRunning in TaskDelete)
                {
                    SystemTask.Remove(taskRunning);
#if DEBUG
                    logger.Debug($"SystemChecker [Ignored] - Task {taskRunning.task.Id} ({taskRunning.task.Status}) is removed");
#endif
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Common.LogError(ex, "CommonPluginsShared [Ignored]");
#endif
            }
        }
    }
}