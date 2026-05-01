using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Financials;

public class HerbalistFinancialDashboardResponse
{
    public decimal CurrentBalance { get; set; }
    public decimal CancelledDeductions { get; set; }
    public IEnumerable<TaskHistoryResponse> TasksHistory { get; set; } = new List<TaskHistoryResponse>();
}