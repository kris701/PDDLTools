using System;
using PDDLTools.TestAdapter.EventWatchers.EventArgs;

namespace PDDLTools.TestAdapter.EventWatchers
{
    public interface ITestFileAddRemoveListener
    {
        event EventHandler<TestFileChangedEventArgs> TestFileChanged;
        void StartListeningForTestFileChanges();
        void StopListeningForTestFileChanges();
    }
}