using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Application.Contracts;
using TaskManager.Application.DTOs;
using TaskManager.Domain.Contracts;

namespace TaskManager.Application.Services
{
    public sealed class ReportService(ITaskRepository taskRepository) : IReportService
    {
        public async Task<List<PerformanceReportDto>> GetPerformanceReportAsync()
        {
            var last30Days = DateTime.UtcNow.AddDays(-30);
            var tasks = await taskRepository.GetTasksCompletedSinceAsync(last30Days);

            var report = tasks
                .GroupBy(t => t.AssignedUserName)
                .Where(g => !string.IsNullOrEmpty(g.Key))
                .Select(g =>
                {
                    var total = g.Count();
                    return new PerformanceReportDto(
                        g.Key!,
                        total,
                        total / 30.0
                    );
                })
                .ToList();

            return report;
        }
    }
}
