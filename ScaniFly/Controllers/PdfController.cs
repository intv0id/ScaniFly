using Microsoft.AspNetCore.Mvc;
using ScaniFly.Services;
using ScaniFly.Models;
using System.IO;

namespace ScaniFly.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PdfController : ControllerBase
{
    private readonly ProposalStateService _proposalStateService;

    public PdfController(ProposalStateService proposalStateService)
    {
        _proposalStateService = proposalStateService;
    }

    [HttpGet("preview/{proposalId}")]
    public IActionResult GetPdfPreview(string proposalId)
    {
        var proposal = _proposalStateService.ActiveProposals.FirstOrDefault(p => p.Id == proposalId);

        if (proposal == null || string.IsNullOrEmpty(proposal.OriginalFilePath) || !System.IO.File.Exists(proposal.OriginalFilePath))
        {
            return NotFound("PDF not found.");
        }

        var stream = new FileStream(proposal.OriginalFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return File(stream, "application/pdf");
    }
}
