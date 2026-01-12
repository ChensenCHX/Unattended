namespace CodeExecutor
{
    public enum RunningState
    {
        Ready,          // ready to run
        Waiting,        // waiting for resume main thread
        Finished,       // execution finished
        Faulted,        // lua vm occured a error | vm internal error
        Terminated,     // terminated (probably by user) and should be discarded
    }

    internal enum InfoHookMode
    {
        Normal,         // normal mode
        NextLine,       // single line step-in
        LineBreakPoint, // line breakpoint mode
    }
}