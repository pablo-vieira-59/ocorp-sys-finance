using SysFinance.Application.DTOs;
using SysFinance.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SysFinance.Application.Interfaces;

public interface IIncomeService
{
    Task<IEnumerable<IncomeDto>> GetIncomesAsync(Guid userId);
    Task<IncomeDto> AddIncomeAsync(IncomeDto incomeDto);
    Task DeleteIncomeAsync(Guid incomeGuid);
}