using Microsoft.AspNetCore.Mvc.RazorPages;
using QuickServerStatuses.Models.Servers;
using System.Collections.Generic;

public class IndexModel : PageModel
{
    private readonly StatusService _statusService;

    public List<Server> Statuses { get; set; } = new();

    public IndexModel(StatusService statusService)
    {
        _statusService = statusService;
    }

    public void OnGet()
    {
        Statuses = _statusService.GetAll();
    }
}
