using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using PhytoIntellect.Application.Contracts.Diseases;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Application.Paginations;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PhytoIntellect.Application.Services;

public class DiseaseService(IUnitOfWork unitOfWork, IMapper mapper) : IDiseaseService
{
    public async Task<PaginatedList<DiseaseResponse>> GetAllDiseasesAsync(RequestFilters filters, CancellationToken cancellationToken = default)
    {
        var query = unitOfWork.DiseaseRepository.GetQueryable(tracked: false)
            .Where(d => d.IsApproved);

        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue.ToLower();
            query = query.Where(d => d.DiseaseName.ToLower().Contains(search));
        }

        bool isDesc = string.Equals(filters.SortDirection, "DESC", StringComparison.OrdinalIgnoreCase);
        if (!string.IsNullOrWhiteSpace(filters.SortColumn))
        {
            query = filters.SortColumn.ToLower() switch
            {
                "diseasename" => isDesc ? query.OrderByDescending(d => d.DiseaseName) : query.OrderBy(d => d.DiseaseName),
                "diseasetype" => isDesc ? query.OrderByDescending(d => d.DiseaseType) : query.OrderBy(d => d.DiseaseType),
                _ => isDesc ? query.OrderByDescending(d => d.DiseaseName) : query.OrderBy(d => d.DiseaseName)
            };
        }
        else
        {
            query = isDesc ? query.OrderByDescending(d => d.DiseaseName) : query.OrderBy(d => d.DiseaseName);
        }

        var projectedQuery = query.ProjectTo<DiseaseResponse>(mapper.ConfigurationProvider);
        return await PaginatedList<DiseaseResponse>.CreateAsync(projectedQuery, filters.PageNumber, filters.PageSize, cancellationToken);
    }

    public async Task<IEnumerable<DiseaseNamesResponse>> GetDiseasesNameAsync(CancellationToken cancellationToken = default)
    {
        var diseases = await unitOfWork.DiseaseRepository.GetAllAsync(
            filter: d => d.IsApproved,
            tracked: false,
            cancellationToken: cancellationToken);

        var mapped = mapper.Map<IEnumerable<DiseaseNamesResponse>>(diseases);
        return mapped.OrderBy(d => d.DiseaseName).ToList();
    }

    public async Task<DiseaseResponse> CreateDiseaseAsync(CreateDiseaseRequest request, CancellationToken cancellationToken = default)
    {
        string cleanName = request.DiseaseName.Trim();

        var existingDisease = await unitOfWork.DiseaseRepository.GetAsync(
            d => d.DiseaseName.ToLower() == cleanName.ToLower(),
            tracked: false,
            cancellationToken: cancellationToken);

        if (existingDisease != null)
            throw new Exception("This disease already exists in the system.");

        var diseaseEntity = mapper.Map<Disease>(request);
        diseaseEntity.DiseaseName = cleanName;
        diseaseEntity.IsSupportedByAi = false; 
        diseaseEntity.IsApproved = false;     

        await unitOfWork.DiseaseRepository.CreateAsync(diseaseEntity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return mapper.Map<DiseaseResponse>(diseaseEntity);
    }

    public async Task<PaginatedList<DiseaseResponse>> GetPendingDiseasesAsync(RequestFilters filters, CancellationToken cancellationToken = default)
    {
        var query = unitOfWork.DiseaseRepository.GetQueryable(tracked: false)
            .Where(d => !d.IsApproved); 

        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue.ToLower();
            query = query.Where(d => d.DiseaseName.ToLower().Contains(search));
        }

        query = query.OrderByDescending(d => d.DiseaseId);

        var projectedQuery = query.ProjectTo<DiseaseResponse>(mapper.ConfigurationProvider);
        return await PaginatedList<DiseaseResponse>.CreateAsync(projectedQuery, filters.PageNumber, filters.PageSize, cancellationToken);
    }

    public async Task<DiseaseResponse> AddDiseaseByAdminAsync(CreateDiseaseRequest request, bool isAiSupported, CancellationToken cancellationToken = default)
    {
        string cleanName = request.DiseaseName.Trim();

        var existingDisease = await unitOfWork.DiseaseRepository.GetAsync(
            d => d.DiseaseName.ToLower() == cleanName.ToLower(),
            tracked: false,
            cancellationToken: cancellationToken);

        if (existingDisease != null)
            throw new Exception("This disease already exists.");

        var diseaseEntity = mapper.Map<Disease>(request);
        diseaseEntity.DiseaseName = cleanName;
        diseaseEntity.IsSupportedByAi = isAiSupported;
        diseaseEntity.IsApproved = true;              

        await unitOfWork.DiseaseRepository.CreateAsync(diseaseEntity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return mapper.Map<DiseaseResponse>(diseaseEntity);
    }

    public async Task<bool> ApproveDiseaseAsync(int diseaseId, CancellationToken cancellationToken = default)
    {
        var disease = await unitOfWork.DiseaseRepository.GetAsync(d => d.DiseaseId == diseaseId, tracked: true, cancellationToken: cancellationToken);
        if (disease == null) return false;

        disease.IsApproved = true;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> RejectDiseaseAsync(int diseaseId, CancellationToken cancellationToken = default)
    {
        var disease = await unitOfWork.DiseaseRepository.GetAsync(d => d.DiseaseId == diseaseId, tracked: true, cancellationToken: cancellationToken);
        if (disease == null) return false;

        unitOfWork.DiseaseRepository.Remove(disease);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}