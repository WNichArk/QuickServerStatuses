using System.Collections.Generic;

public class IndexModel : PageModel
{
    private readonly StatusService _statusService;

    public Dictionary<string, string> Statuses { get; set; } = new();

    public IndexModel(StatusService statusService)
    {
        _statusService = statusService;
    }

    public void OnGet()
    {
        Statuses = _statusService.GetAll().ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }
}
