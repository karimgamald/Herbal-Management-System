using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using PhytoIntellect.Application.Contracts.Herbalists;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Application.Paginations;
using PhytoIntellect.Core.Entities;
using PhytoIntellect.Core.Interfaces;
using System.Reflection;

namespace PhytoIntellect.Application.Services;

public class HerbalistService(IUnitOfWork unitOfWork, IMapper mapper) : IHerbalistService
{
    public async Task<HerbalistResponse?> GetMyProfileAsync(int userId, CancellationToken cancellationToken = default)
    {
        var herbalist = await unitOfWork.HerbalistRepository
            .GetAsync(h => h.UserId == userId, tracked: false, cancellationToken: cancellationToken);

        return herbalist == null ? null : mapper.Map<HerbalistResponse>(herbalist);
    }

    public async Task<HerbalistResponse?> GetHerbalistByIdAsync(int herbalistId, CancellationToken cancellationToken = default)
    {
        var herbalist = await unitOfWork.HerbalistRepository
            .GetAsync(h => h.HerbalistId == herbalistId, tracked: false, cancellationToken: cancellationToken);

        return herbalist == null ? null : mapper.Map<HerbalistResponse>(herbalist);
    }

    public async Task<PaginatedList<HerbalistResponse>> GetAllHerbalistsAsync(RequestFilters filters, CancellationToken cancellationToken = default)
    {
        var query = unitOfWork.HerbalistRepository.GetQueryable(tracked: false);

        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue.ToLower();
            query = query.Where(h => h.User!.FullName.ToLower().Contains(search));
        }

        bool isDesc = filters.SortDirection?.ToUpper() == "DESC";
        if (!string.IsNullOrWhiteSpace(filters.SortColumn))
        {
            query = filters.SortColumn.ToLower() switch
            {
                "fullname" => isDesc ? query.OrderByDescending(h => h.User!.FullName) : query.OrderBy(h => h.User!.FullName),
                "licensenumber" => isDesc ? query.OrderByDescending(h => h.LicenseNumber) : query.OrderBy(h => h.LicenseNumber),
                "averagerating" => isDesc ? query.OrderByDescending(h => h.AverageRating) : query.OrderBy(h => h.AverageRating),
                _ => isDesc ? query.OrderByDescending(h => h.User!.FullName) : query.OrderBy(h => h.User!.FullName)
            };
        }
        else
        {
            query = isDesc ? query.OrderByDescending(h => h.User!.FullName) : query.OrderBy(h => h.User!.FullName);
        }

        var projectedQuery = query.ProjectTo<HerbalistResponse>(mapper.ConfigurationProvider);

        var paginatedHerbalists = await PaginatedList<HerbalistResponse>.CreateAsync(
            projectedQuery,
            filters.PageNumber,
            filters.PageSize,
            cancellationToken);

        return paginatedHerbalists;
    }

    public async Task<string> CreateProfileAsync(int userId, CreateOrUpdateHerbalistRequest request, CancellationToken cancellationToken = default)
    {
        var exists = await unitOfWork.HerbalistRepository
            .GetAsync(h => h.UserId == userId, tracked: false, cancellationToken: cancellationToken);

        if (exists != null)
            return "Profile already exists.";

        var herbalist = mapper.Map<Herbalist>(request);
        herbalist.UserId = userId;
        herbalist.AverageRating = 0;

        await unitOfWork.HerbalistRepository.CreateAsync(herbalist, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return "Herbalist profile created successfully.";
    }

    public async Task<string> UpdateMyProfileAsync(int userId,CreateOrUpdateHerbalistRequest request, CancellationToken cancellationToken = default)
    {
        var herbalist = await unitOfWork.HerbalistRepository
            .GetAsync(h => h.UserId == userId, tracked: true, cancellationToken: cancellationToken);

        if (herbalist == null)
            return "Herbalist profile not found.";

        herbalist.Bio = request.Bio;
        herbalist.AvailableFrom = request.AvailableFrom;
        herbalist.AvailableTo = request.AvailableTo;

        unitOfWork.HerbalistRepository.Update(herbalist);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return "Profile updated successfully.";
    }

    public async Task<PaginatedList<HerbalistResponse>> AdminGetAllHerbalistsAsync(RequestFilters filters, CancellationToken cancellationToken = default)
    {
        var query = unitOfWork.HerbalistRepository.GetQueryable(tracked: false);

        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue.ToLower();
            query = query.Where(h => h.User!.FullName.ToLower().Contains(search) || h.LicenseNumber.ToLower().Contains(search));
        }

        bool isDesc = filters.SortDirection?.ToUpper() == "DESC";
        if (!string.IsNullOrWhiteSpace(filters.SortColumn))
        {
            query = filters.SortColumn.ToLower() switch
            {
                "fullname" => isDesc ? query.OrderByDescending(h => h.User!.FullName) : query.OrderBy(h => h.User!.FullName),
                "licensenumber" => isDesc ? query.OrderByDescending(h => h.LicenseNumber) : query.OrderBy(h => h.LicenseNumber),
                "averagerating" => isDesc ? query.OrderByDescending(h => h.AverageRating) : query.OrderBy(h => h.AverageRating),
                _ => isDesc ? query.OrderByDescending(h => h.HerbalistId) : query.OrderBy(h => h.HerbalistId)
            };
        }
        else
        {
            query = query.OrderByDescending(h => h.HerbalistId);
        }

        var projectedQuery = query.ProjectTo<HerbalistResponse>(mapper.ConfigurationProvider);
        return await PaginatedList<HerbalistResponse>.CreateAsync(projectedQuery, filters.PageNumber, filters.PageSize, cancellationToken);
    }

    public async Task<bool> DeleteHerbalistAsync(int herbalistId, CancellationToken cancellationToken = default)
    {
        var herbalist = await unitOfWork.HerbalistRepository.GetAsync(
            h => h.HerbalistId == herbalistId,
            tracked: true,
            cancellationToken: cancellationToken);

        if (herbalist == null) return false;

        unitOfWork.HerbalistRepository.Remove(herbalist);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<object> GetHerbalistsStatsAsync(CancellationToken cancellationToken = default)
    {
        var query = unitOfWork.HerbalistRepository.GetQueryable(tracked: false);

        var totalHerbalists = await query.CountAsync(cancellationToken);

        double averageRating = 0;
        if (totalHerbalists > 0)
        {
            averageRating = await query.AverageAsync(h => h.AverageRating, cancellationToken);
        }

        return new
        {
            TotalHerbalists = totalHerbalists,
            SystemAverageRating = Math.Round(averageRating, 1)
        };
    }
} 