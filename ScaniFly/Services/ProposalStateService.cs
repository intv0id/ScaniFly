using ScaniFly.Models;

namespace ScaniFly.Services;

public class ProposalStateService
{
    private readonly List<DocumentProposal> _activeProposals = new();
    public IReadOnlyList<DocumentProposal> ActiveProposals => _activeProposals.AsReadOnly();

    public event Action? OnStateChanged;

    public void AddProposal(DocumentProposal proposal)
    {
        _activeProposals.Add(proposal);
        NotifyStateChanged();
    }

    public void RemoveProposal(DocumentProposal proposal)
    {
        _activeProposals.Remove(proposal);
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnStateChanged?.Invoke();
}
