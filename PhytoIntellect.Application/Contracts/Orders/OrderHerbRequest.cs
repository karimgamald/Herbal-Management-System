using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Orders;

public record OrderHerbRequest(int HerbId, int HerbalistId, int Quantity);