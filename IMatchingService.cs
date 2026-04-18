using ProjectApprovalSystem.Models;
using ProjectApprovalSystem.Models.ViewModels;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ProjectApprovalSystem.Services
{
    public interface IMatchingService
    {
        Task<bool> ExpressInterestAsync(int projectId, string supervisorId);
        Task<bool> ConfirmMatchAsync(int matchId, string supervisorId);
        Task<bool> RevealIdentityAsync(int projectId);
        Task<List<BlindReviewViewModel>> GetAvailableProjectsForSupervisorAsync(string supervisorId);
        Task<Match?> GetPendingMatchForProjectAsync(int projectId);
        Task<bool> IsProjectAvailableForMatchingAsync(int projectId);
    }
}