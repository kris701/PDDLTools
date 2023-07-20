using System;
using PDDLTools.TestAdapter.EventWatchers.EventArgs.Vs;

namespace PDDLTools.TestAdapter.EventWatchers.Vs
{
    public interface ITestFileAddRemoveListener
    {
        event EventHandler<TestFileChangedEventArgs> TestFileChanged;
        void StartListeningForTestFileChanges();
        void StopListeningForTestFileChanges();
    }
}