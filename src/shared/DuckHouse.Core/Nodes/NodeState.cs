namespace DuckHouse.Core.Nodes;

public enum NodeState
{
    Unknown,
    Stopped,
    Resuming,
    Running,
    Stopping,
    Deleting,
    Creating,
    Failure
}