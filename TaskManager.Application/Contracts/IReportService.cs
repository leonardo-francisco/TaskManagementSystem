using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.Contracts
{
    public interface IReportService
    {
        Task<List<PerformanceReportDto>> GetPerformanceReportAsync();
    }
}
