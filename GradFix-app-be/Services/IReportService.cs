using System.Threading.Tasks;
using GradFix_app_be.Services.Dtos;

namespace GradFix_app_be.Services
{
    public interface IReportService
    {
        Task<int> CreateReportAsync(ReportCreateDto dto, string? reporterId);
    }
}
