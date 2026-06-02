using Datateal.Core.Orchestration;

namespace Datateal.Orchestrator.Core.Entities;

public class SqlQueryTaskRun : ComputeTaskRun
{
    public SqlQueryTaskRun() { TaskType = TaskType.SqlQuery; }
}
