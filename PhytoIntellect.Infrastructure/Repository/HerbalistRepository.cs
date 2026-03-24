using PhytoIntellect.Core.Entities;
using PhytoIntellect.Core.Interfaces;
using PhytoIntellect.Infrastructure.Presistence;
using Microsoft.EntityFrameworkCore; // 👈 ده اللي هيطير الإيرور الأحمر

using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Infrastructure.Repository;

public class HerbalistRepository(ApplicationDbContext context) : Repository<Herbalist>(context), IHerbalistRepository
{
    public async Task<int> GetIdByUserIdAsync(string userId)
    {
        // 1. نفس التحويل من string لـ int
        if (!int.TryParse(userId, out int parsedUserId))
            return 0;

        // 2. نقارن الرقم بالرقم
        var herbalist = await context.Herbalists
                                     .FirstOrDefaultAsync(h => h.UserId == parsedUserId);

        return herbalist?.HerbalistId ?? 0;
    }
}
