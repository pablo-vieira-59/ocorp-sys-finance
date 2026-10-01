using SysFinance.Application.DTOs;
using SysFinance.Application.Interfaces;
using SysFinance.Domain.Entities;
using SysFinance.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SysFinance.Application.Services;

public class IncomeService : IIncomeService
{
    private readonly IRepository<Income> _incomeRepo;
    private readonly IRepository<Investment> _investmentRepo;

    public IncomeService(IRepository<Income> incomeRepo, IRepository<Investment> investmentRepo)
    {
        _incomeRepo = incomeRepo;
        _investmentRepo = investmentRepo;
    }

    public async Task<IEnumerable<IncomeDto>> GetIncomesAsync(Guid userId)
    {
        var assets = await _incomeRepo.FindAsync(a => a.UserId == userId);
        return assets.Select(a => new IncomeDto { 
            Id = a.Id, 
            Description = a.Description,
            Amount = a.Amount,
            Discounts = a.Discounts,
            Type = a.Type,
            UserId = a.UserId 
        });
    }

    public async Task<IncomeDto> AddIncomeAsync(IncomeDto incomeDto)
    {
        if(incomeDto.Id != null)
        {
            var existingIncome = await _incomeRepo.GetByIdAsync(incomeDto.Id.Value);

            if(existingIncome != null)
            {
                existingIncome.Description = incomeDto.Description;
                existingIncome.Amount = incomeDto.Amount;
                existingIncome.Discounts = incomeDto.Discounts;
                existingIncome.Type = incomeDto.Type;

                await _incomeRepo.UpdateAsync(existingIncome);
            }

            return incomeDto;
        }
        else
        {
            var newIncome = new Income
            {
                Amount = incomeDto.Amount,
                UserId = incomeDto.UserId,
                Description = incomeDto.Description,
                Discounts = incomeDto.Discounts,
                Type = incomeDto.Type,
                Id = Guid.NewGuid(),
            };

            await _incomeRepo.AddAsync(newIncome);

            return new IncomeDto { 
                Id = newIncome.Id, 
                Description = newIncome.Description,
                Amount = newIncome.Amount,
                Discounts = newIncome.Discounts,
                Type = newIncome.Type,
                UserId = newIncome.UserId 
            };
        }
    }

    public async Task DeleteIncomeAsync(Guid incomeGuid)
    {
        var existingIncome = await _incomeRepo.GetByIdAsync(incomeGuid);

        if (existingIncome != null)
        {
            var userId = existingIncome.UserId;
            var description = existingIncome.Description;

            if (existingIncome!.Type == "Investimento")
            {
                var investment = (await _investmentRepo.FindAsync(x => x.UserId == userId
                                && x.Name == existingIncome.Description)).FirstOrDefault();

                if (investment != null)
                {
                    await _investmentRepo.DeleteAsync(investment);
                }
            }

            await _incomeRepo.DeleteAsync(existingIncome);
        }
    }
}