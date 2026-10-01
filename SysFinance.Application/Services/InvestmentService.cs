using SysFinance.Application.DTOs;
using SysFinance.Application.Interfaces;
using SysFinance.Domain.Entities;
using SysFinance.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SysFinance.Application.Services;

public class InvestmentService : IInvestmentService
{
    private readonly IRepository<Investment> _investmentRepo;
    private readonly IRepository<Income> _incomeRepo;
    private readonly IRepository<FixedIncomeInvestment> _fixedIncomeInvestmentRepo;
    private readonly IRepository<VariableIncomeInvestment> _variableIncomeInvestmentRepo;

    public InvestmentService(IRepository<Investment> investmentRepo, 
                              IRepository<Income> incomeRepo,
                              IRepository<FixedIncomeInvestment> fixedIncomeInvestmentRepo,
                              IRepository<VariableIncomeInvestment> variableIncomeInvestmentRepo)
    {
        _investmentRepo = investmentRepo;
        _incomeRepo = incomeRepo;
        _fixedIncomeInvestmentRepo = fixedIncomeInvestmentRepo;
        _variableIncomeInvestmentRepo = variableIncomeInvestmentRepo;
    }

    public async Task<IEnumerable<InvestmentDto>> GetInvestmentsAsync(Guid userId)
    {
        var investments = await _investmentRepo.FindAsync(i => i.UserId == userId, i => i.Variable, i => i.Fixed);
        return investments.Select(i => new InvestmentDto
        {
            Id = i.Id,
            Name = i.Name,
            Type = i.Type,
            CreatedAt = i.CreatedAt,
            Fixed = i.Fixed == null ? null : new FixedIncomeInvestmentDto
            {
                CurrentAmount = i.Fixed.CurrentAmount,
                Id = i.Id,
                InitialAmount = i.Fixed.InitialAmount,
                InterestRate = i.Fixed.InterestRate,
                InvestmentId = i.Fixed.InvestmentId,
            },
            Variable = i.Variable == null ? null : new VariableIncomeInvestmentDto
            {
                AveragePrice = i.Variable.AveragePrice,
                Id = i.Id,
                CurrentQuotePrice = i.Variable.CurrentQuotePrice,
                InvestedAmount = i.Variable.InvestedAmount,
                InvestmentId = i.Variable.InvestmentId,
                MonthlyDividendYield = i.Variable.MonthlyDividendYield,
                Quantity = i.Variable.Quantity,
            },
        });
    }

    public async Task<InvestmentDto> AddInvestmentAsync(Guid userId, InvestmentDto dto)
    {
        if(dto.Id == null)
        {
            var investment = new Investment
            {
                UserId = userId,
                Name = dto.Name,
                Type = dto.Type,
                CreatedAt = dto.CreatedAt
            };

            if(dto.Type == "Renda Fixa")
            {
                investment.Fixed = new FixedIncomeInvestment
                {
                    CurrentAmount = dto.Fixed!.CurrentAmount,
                    InitialAmount = dto.Fixed!.InitialAmount,
                    InterestRate = dto.Fixed!.InterestRate,
                };

                if (investment.Fixed.InterestRate > 0)
                {
                    var income = new Income
                    {
                        Amount = dto.Fixed!.CurrentAmount * ((dto.Fixed!.InterestRate / 100) / 12),
                        Description = dto.Name,
                        Discounts = 0,
                        UserId = userId,
                        Type = "Investimento"
                    };

                    await _incomeRepo.AddAsync(income);
                }
            }
            else
            {
                investment.Variable = new VariableIncomeInvestment
                {
                    CurrentQuotePrice = dto.Variable!.CurrentQuotePrice,
                    AveragePrice = dto.Variable!.AveragePrice,
                    InvestedAmount = dto.Variable!.InvestedAmount,
                    MonthlyDividendYield = dto.Variable!.MonthlyDividendYield,
                    Quantity = dto.Variable!.Quantity
                };

                if (investment.Variable.MonthlyDividendYield > 0)
                {
                    var income = new Income
                    {
                        Amount = (dto.Variable!.CurrentQuotePrice * (decimal)dto.Variable!.Quantity) * (dto.Variable!.MonthlyDividendYield / 100),
                        Description = dto.Name,
                        Discounts = 0,
                        UserId = userId,
                        Type = "Investimento"
                    };

                    await _incomeRepo.AddAsync(income);
                }
            }

            await _investmentRepo.AddAsync(investment);

            return new InvestmentDto { Id = investment.Id };
        }
        else
        {
            var existingInvestments = await _investmentRepo.FindAsync(x => x.Id == dto.Id.Value, x=> x.Variable, x=>x.Fixed);
            var existingInvestment = existingInvestments.FirstOrDefault();

            if (existingInvestment != null) 
            {
                existingInvestment.Name = dto.Name;
                existingInvestment.Type = dto.Type;

                if(existingInvestment.Type == "Renda Fixa" && existingInvestment.Fixed == null)
                {
                    existingInvestment.Fixed = new FixedIncomeInvestment
                    {
                        CurrentAmount = dto.Fixed.CurrentAmount,
                        InterestRate = dto.Fixed.InterestRate,
                        InitialAmount = dto.Fixed.InitialAmount,
                        InvestmentId = dto.Fixed.InvestmentId
                    };

                    await _fixedIncomeInvestmentRepo.AddAsync(existingInvestment.Fixed);
                }

                if (existingInvestment.Type != "Renda Fixa" && existingInvestment.Variable == null)
                {
                    existingInvestment.Variable = new VariableIncomeInvestment
                    {
                        AveragePrice = dto.Variable.AveragePrice,
                        Quantity = dto.Variable.Quantity,
                        InvestedAmount = dto.Variable.InvestedAmount,
                        InvestmentId = dto.Variable.InvestmentId,
                        CurrentQuotePrice = dto.Variable.CurrentQuotePrice,
                        MonthlyDividendYield = dto.Variable.MonthlyDividendYield,
                    };

                    await _variableIncomeInvestmentRepo.AddAsync(existingInvestment.Variable);
                }

                if (existingInvestment.Fixed != null)
                {
                    existingInvestment.Fixed.InterestRate = dto.Fixed.InterestRate;
                    existingInvestment.Fixed.CurrentAmount = dto.Fixed.CurrentAmount;
                    existingInvestment.Fixed.InitialAmount = dto.Fixed.InitialAmount;
                }

                if(existingInvestment.Variable != null)
                {
                    existingInvestment.Variable.MonthlyDividendYield = dto.Variable.MonthlyDividendYield;
                    existingInvestment.Variable.Quantity = dto.Variable.Quantity;
                    existingInvestment.Variable.CurrentQuotePrice = dto.Variable.CurrentQuotePrice;
                    existingInvestment.Variable.AveragePrice = dto.Variable.AveragePrice;
                    existingInvestment.Variable.InvestedAmount = dto.Variable.InvestedAmount;
                }

                await _investmentRepo.UpdateAsync(existingInvestment);

                var income = (await _incomeRepo.FindAsync(x => x.UserId == existingInvestment.UserId 
                                && x.Description == existingInvestment.Name 
                                && x.Type == "Investimento")).FirstOrDefault();

                if (income != null) 
                { 
                    if(existingInvestment.Type == "Renda Fixa")
                    {
                        income.Amount = dto.Fixed!.CurrentAmount * ((dto.Fixed!.InterestRate / 100) / 12);
                        await _incomeRepo.UpdateAsync(income);
                    }
                    else
                    {
                        income.Amount = (dto.Variable!.CurrentQuotePrice * (decimal)dto.Variable!.Quantity) * (dto.Variable!.MonthlyDividendYield / 100);
                        if(dto.Variable!.MonthlyDividendYield <= 0)
                        {
                            await _incomeRepo.DeleteAsync(income);
                        }
                        else
                        {
                            await _incomeRepo.UpdateAsync(income);
                        }
                    }
                }
                else
                {
                    if(existingInvestment.Type == "Renda Fixa" && existingInvestment.Fixed.InterestRate > 0)
                    {
                        var newIncome = new Income
                        {
                            Amount = dto.Fixed!.CurrentAmount * dto.Fixed!.InterestRate,
                            Type = "Investimento",
                            UserId = userId,
                            Description = dto.Name,
                        };

                        await _incomeRepo.AddAsync(newIncome);
                    }
                    if(existingInvestment.Type != "Renda Fixa" && existingInvestment.Variable.MonthlyDividendYield > 0)
                    {
                        var newIncome = new Income
                        {
                            Amount = ((decimal)dto.Variable!.Quantity * dto.Variable!.CurrentQuotePrice) * (dto.Variable!.MonthlyDividendYield /100),
                            Type = "Investimento",
                            UserId = userId,
                            Description = dto.Name,
                        };

                        await _incomeRepo.AddAsync(newIncome);
                    }
                }
            }

            return new InvestmentDto { Id = dto.Id };
        }
    }

    public async Task DeleteInvestmentAsync(Guid userId, Guid investmentId)
    {
        var investment = (await _investmentRepo.FindAsync(i => i.Id == investmentId && i.UserId == userId)).FirstOrDefault();
        if (investment == null) throw new InvalidOperationException("Investment not found");

        var name = investment.Name;
        await _investmentRepo.DeleteAsync(investment);

        var existingIncome = (await _incomeRepo.FindAsync(i => i.UserId == userId && i.Description == name && i.Type == "Investimento")).FirstOrDefault();
        if (existingIncome != null)
        {
            await _incomeRepo.DeleteAsync(existingIncome);
        }
    }
}