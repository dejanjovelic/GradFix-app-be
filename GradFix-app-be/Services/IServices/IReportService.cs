using System.Threading.Tasks;
using GradFix_app_be.Services.Dtos;

namespace GradFix_app_be.Services.IServices
{
    public interface IReportService
    {
        Task<int> CreateReportAsync(ReportCreateDto dto, string? reporterId);
    }
}
