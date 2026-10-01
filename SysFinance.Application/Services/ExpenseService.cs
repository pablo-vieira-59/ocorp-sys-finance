using SysFinance.Application.DTOs;
using SysFinance.Application.Interfaces;
using SysFinance.Domain.Entities;
using SysFinance.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SysFinance.Application.Services;

public class ExpenseService : IExpenseService
{
    private readonly IRepository<Expense> _expenseRepo;

    public ExpenseService(IRepository<Expense> expenseRepo)
    {
        _expenseRepo = expenseRepo;
    }

    public async Task DeleteExpenseAsync(Guid expenseId)
    {
        var existingExpense = await _expenseRepo.GetByIdAsync(expenseId);
        if (existingExpense != null)
        {
            await _expenseRepo.DeleteAsync(existingExpense);
        }
    }

    public async Task<IEnumerable<ExpenseDto>> GetExpensesAsync(Guid userId)
    {
        var expenses = await _expenseRepo.FindAsync(e => e.UserId == userId);
        return expenses.Select(e => new ExpenseDto{
            Id = e.Id,
            Description = e.Description,
            Amount = e.Amount,
            Category = e.Category 
        });
    }

    public async Task<ExpenseDto> AddExpenseAsync(Guid userId, ExpenseDto dto)
    {
        if (dto.Id != null)
        {
            var existingExpense = await _expenseRepo.GetByIdAsync(dto.Id.Value);

            if (existingExpense != null)
            {
                existingExpense.Amount = dto.Amount;
                existingExpense.Category = dto.Category;
                existingExpense.Description = dto.Description;

                await _expenseRepo.UpdateAsync(existingExpense);
            }
            return dto;
        }
        else
        {
            var expense = new Expense { 
                UserId = userId, 
                Description = dto.Description, 
                Amount = dto.Amount, 
                Category = dto.Category 
            };

            await _expenseRepo.AddAsync(expense);
            return dto with { Id = expense.Id };
        }
    }
}