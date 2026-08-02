using System.Threading.Tasks;
using GradFix_app_be.Services.Dtos;

namespace GradFix_app_be.Services
{
    public interface IReportService
    {
        Task<Guid> CreateReportAsync(ReportCreateDto dto, string? reporterId);
    }
}
