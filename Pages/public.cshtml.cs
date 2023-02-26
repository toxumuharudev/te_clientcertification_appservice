using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _024_ClientCertificationOnAppService.Pages;

public class PublicModel : PageModel
{
    private readonly ILogger<PublicModel> _logger;

    public PublicModel(ILogger<PublicModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }
}
