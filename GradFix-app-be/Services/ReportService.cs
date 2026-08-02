using AutoMapper;
using GradFix_app_be.Domain;
using GradFix_app_be.Infrastructure;
using GradFix_app_be.Services.Dtos;
using GradFix_app_be.Services.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GradFix_app_be.Services
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public ReportService(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Guid> CreateReportAsync(ReportCreateDto dto, string? reporterId)
        {
            var images = dto.Images ?? new List<ReportImageCreateDto>();

            if (images.Count > 3)
            {
                throw new BadRequestException("A report cannot have more than 3 images.");
            }

            Report report = _mapper.Map<Report>(dto);
            report.StatusId = 1;
            report.CreatedAt = DateTime.UtcNow;

            _db.Reports.Add(report);

            // map images
            int order = 0;
            foreach (var img in images)
            {
                var ri = new ReportImage
                {
                    Report = report,
                    FileName = img.FileName,
                    FilePath = img.FilePath,
                    ContentType = img.ContentType,
                    Size = img.Size,
                    Order = order++,
                    CreatedAt = DateTime.UtcNow
                };
                _db.ReportImages.Add(ri); //Service ne radi dodavanje u bazu izmeniti
            }

            await _db.SaveChangesAsync();
            return report.Id;
        }
    }
}
