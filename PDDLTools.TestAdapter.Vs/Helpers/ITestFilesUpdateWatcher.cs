using System;
using PDDLTools.TestAdapter.EventWatchers.EventArgs;

namespace PDDLTools.TestAdapter.EventWatchers
{
    public interface ITestFilesUpdateWatcher
    {
        event EventHandler<TestFileChangedEventArgs> FileChangedEvent;
        void AddWatch(string path);
        void RemoveWatch(string path);
    }
}