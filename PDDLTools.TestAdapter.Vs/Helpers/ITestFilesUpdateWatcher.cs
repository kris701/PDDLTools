using System;
using PDDLTools.TestAdapter.EventWatchers.EventArgs.Vs;

namespace PDDLTools.TestAdapter.EventWatchers.Vs
{
    public interface ITestFilesUpdateWatcher
    {
        event EventHandler<TestFileChangedEventArgs> FileChangedEvent;
        void AddWatch(string path);
        void RemoveWatch(string path);
    }
}