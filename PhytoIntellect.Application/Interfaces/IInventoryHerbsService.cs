using PhytoIntellect.Application.Contracts.Inventory;
using PhytoIntellect.Application.Paginations;
using System;
using System.Collections.Generic;
using System.Text;

public interface IInventoryHerbsService
{
    Task<PaginatedList<InventoryResponse>> GetMyInventoryAsync(int userId,RequestFilters filters ,CancellationToken cancellationToken);
    Task<PaginatedList<InventoryResponse>> GetAllByHerbalistIdAsync(int herbalistId, RequestFilters filters,
    CancellationToken cancellationToken = default);
    Task<InventoryResponse?> AddHerbAsync(int userId,AddHerbToInventoryRequest request,CancellationToken cancellationToken);

    Task<bool> UpdateInventoryAsync(int userId,int herbId,UpdateInventoryRequest request,CancellationToken cancellationToken);

    Task<bool> RemoveHerbAsync(int userId,int herbId,CancellationToken cancellationToken);
    Task<PaginatedList<InventoryResponse>> GetAllInventoryByAdminAsync(RequestFilters filters, CancellationToken cancellationToken = default);
    Task<bool> RemoveHerbFromHerbalistInventoryByAdminAsync(int herbalistId, int herbId, CancellationToken cancellationToken = default);
}
