namespace Datateal.Ui.Client.Notebook;

public class NotebookDocument
{
    public string Title { get; set; } = "Untitled";
    public List<NotebookCell> Cells { get; set; } = [];
    public bool IsDirty { get; set; }
}
