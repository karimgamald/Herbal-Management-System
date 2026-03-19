using AutoMapper;
using PhytoIntellect.Application.Contracts.Diseases;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Entities;
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

    public async Task<IEnumerable<DiseaseNamesResponse>> GetDiseasesNameAsync(CancellationToken cancellationToken = default)
    {
        // بنجيب الداتا خفيفة من غير Tracking
        var diseases = await unitOfWork.DiseaseRepository.GetAllAsync(tracked: false, cancellationToken: cancellationToken);

        // بنحولها لـ Dropdown Response (ID + Name بس) وبنرتبها أبجدياً
        var mapped = mapper.Map<IEnumerable<DiseaseNamesResponse>>(diseases);
        return mapped.OrderBy(d => d.DiseaseName).ToList();
    }

    public async Task<DiseaseResponse> CreateDiseaseAsync(CreateDiseaseRequest request, CancellationToken cancellationToken = default)
    {
        string cleanName = request.DiseaseName.Trim();

        // 🛡️ تأمين ضد التكرار (عشان الداتابيز متتمليش أمراض متكررة)
        var existingDisease = await unitOfWork.DiseaseRepository.GetAsync(
            d => d.DiseaseName.ToLower() == cleanName.ToLower(),
            tracked: false,
            cancellationToken: cancellationToken);

        if (existingDisease != null)
            throw new Exception("This disease already exists.");

        // سحر الـ AutoMapper (هياخد الاسم، ولو مفيش Type أو Symptoms هيحطهم بـ Null عادي)
        var diseaseEntity = mapper.Map<Disease>(request);
        diseaseEntity.DiseaseName = cleanName; // نتأكد إن الاسم نضيف من غير مسافات

        await unitOfWork.DiseaseRepository.CreateAsync(diseaseEntity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // بنرجع المرض كامل بالـ ID الجديد بتاعه
        return mapper.Map<DiseaseResponse>(diseaseEntity);
    }
}