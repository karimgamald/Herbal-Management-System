using System;
using System.Collections.Generic;
using System.Text;

using PhytoIntellect.Application.Contracts.Inventory;

public interface IInventoryService
{
    Task<IEnumerable<InventoryResponse>> GetMyInventoryAsync(int userId,CancellationToken cancellationToken);
    Task<IEnumerable<InventoryResponse>> GetAllByHerbalistIdAsync(int herbalistId, CancellationToken cancellationToken);
    Task<InventoryResponse?> AddHerbAsync(int userId,AddHerbToInventoryRequest request,CancellationToken cancellationToken);

    Task<bool> UpdateInventoryAsync(int userId,int herbId,UpdateInventoryRequest request,CancellationToken cancellationToken);

    Task<bool> RemoveHerbAsync(int userId,int herbId,CancellationToken cancellationToken);
}
