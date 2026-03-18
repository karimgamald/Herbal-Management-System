using AutoMapper;
using PhytoIntellect.Application.Contracts.Diseases;
using PhytoIntellect.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Services;

public class DiseaseService(IUnitOfWork unitOfWork, IMapper mapper) : IDiseaseService
{
    public async Task<IEnumerable<DiseaseResponse>> GetAllDiseasesAsync(CancellationToken cancellationToken = default)
    {
        var diseases = await unitOfWork.DiseaseRepository.GetAllAsync(
            tracked: false,
            cancellationToken: cancellationToken);

        var mappedDiseases = mapper.Map<IEnumerable<DiseaseResponse>>(diseases);

        // رتبناهم أبجدياً عشان يظهروا في الـ Dropdown بشكل منظم
        return mappedDiseases.OrderBy(d => d.DiseaseName).ToList();
    }
}