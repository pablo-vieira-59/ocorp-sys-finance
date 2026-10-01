using SysFinance.Application.DTOs;
using SysFinance.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SysFinance.Application.Interfaces;

public interface IInvestmentService
{
    Task<IEnumerable<InvestmentDto>> GetInvestmentsAsync(Guid userId);
    Task<InvestmentDto> AddInvestmentAsync(Guid userId, InvestmentDto investmentDto);
    Task DeleteInvestmentAsync(Guid userId, Guid investmentId);
}