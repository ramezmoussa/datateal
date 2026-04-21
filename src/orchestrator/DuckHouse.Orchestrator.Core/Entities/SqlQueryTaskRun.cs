using DuckHouse.Core.Orchestration;

namespace DuckHouse.Orchestrator.Core.Entities;

public class SqlQueryTaskRun : ComputeTaskRun
{
    public SqlQueryTaskRun() { TaskType = TaskType.SqlQuery; }
}
