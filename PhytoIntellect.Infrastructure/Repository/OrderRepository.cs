using PhytoIntellect.Core.Entities;
using PhytoIntellect.Core.Interfaces;
using PhytoIntellect.Infrastructure.Presistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Infrastructure.Repository;

public class OrderRepository(ApplicationDbContext context) : Repository<Order>(context), IOrderRepository
{
}
