using Svelto.ECS;
using Svelto.Tasks;
using System.Collections;
using System.Collections.Generic;

namespace VectorEngine.Engine.Common
{
    public static class VectorSchedulers
    {
        public static VectorScheduler<IEnumerator> RenderScheduler
        {
            get
            {
                if (render == null)
                    render = new VectorScheduler<IEnumerator>();

                return render;
            }
        }

        public static VectorScheduler<IEnumerator> UpdateScheduler
        {
            get
            {
                if (update == null)
                    update = new VectorScheduler<IEnumerator>();

                return update;
            }
        }

        public static SimpleSubmissionEntityViewScheduler EntityScheduler
        {
            get
            {
                if (entityScheduler == null)
                    entityScheduler = new SimpleSubmissionEntityViewScheduler();

                return entityScheduler;
            }
        }

        public static void RunRender()
        {
            if (render != null)
                render.Run();
        }

        public static void RunUpdate()
        {
            if (update != null)
                update.Run();

            if (entityScheduler != null)
                entityScheduler.SubmitEntities();
        }

        private static SimpleSubmissionEntityViewScheduler entityScheduler;
        private static VectorScheduler<IEnumerator> render;
        private static VectorScheduler<IEnumerator> update;
    }

    public sealed class VectorScheduler<T> : IRunner<T> where T : IEnumerator
    {
        public bool isPaused { get; set; }

        public bool isStopping { get; private set; } = false;

        public bool isKilled { get; private set; } = false;

        public int numberOfRunningTasks => runningTasks.Count > 0 ? 1 : 0;

        public int numberOfQueuedTasks => runningTasks.Count > 1 ? runningTasks.Count - 1 : 0;

        private readonly HashSet<ISveltoTask<T>> runningTasks = new HashSet<ISveltoTask<T>>();

        public void Dispose()
        {
            isKilled = true;

            foreach (ISveltoTask<T> task in runningTasks)
            {
                task.Stop();
            }

            runningTasks.Clear();
        }

        public void StartCoroutine(ISveltoTask<T> task)
        {
            runningTasks.Add(task);
        }

        public void StopAllCoroutines()
        {
            isStopping = true;
            foreach (ISveltoTask<T> task in runningTasks)
            {
                task.Stop();
            }
        }

        public void Run()
        {
            if (isPaused)
                return;

            foreach (ISveltoTask<T> task in runningTasks)
            {
                if (!task.MoveNext())
                {
                    task.Stop();
                    runningTasks.Remove(task);
                }
            }
        }
    }
}
