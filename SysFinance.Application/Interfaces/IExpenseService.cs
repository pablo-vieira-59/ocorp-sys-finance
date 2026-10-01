using SysFinance.Application.DTOs;
using SysFinance.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SysFinance.Application.Interfaces;

public interface IExpenseService
{
    Task DeleteExpenseAsync(Guid expenseId);
    Task<IEnumerable<ExpenseDto>> GetExpensesAsync(Guid userId);
    Task<ExpenseDto> AddExpenseAsync(Guid userId, ExpenseDto expenseDto);
}