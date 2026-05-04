using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.Contracts.Diseases;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Application.Paginations;
using PhytoIntellect.Core.Constants;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Services;

public class DiseaseService(IUnitOfWork unitOfWork, IMapper mapper) : IDiseaseService
{
    public async Task<PaginatedList<DiseaseResponse>> GetAllDiseasesAsync(
     RequestFilters filters,
     CancellationToken cancellationToken = default)
    {
        // 1. نجيب الـ IQueryable
        var query = unitOfWork.DiseaseRepository.GetQueryable(tracked: false);

        // 2. البحث (Searching) - هيبحث باسم المرض
        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue.ToLower();
            // لو في عمود للوصف Description ممكن تزوده هنا
            query = query.Where(d => d.DiseaseName.ToLower().Contains(search));
        }

        // 3. الترتيب (Sorting)
        if (!string.IsNullOrWhiteSpace(filters.SortColumn))
        {
            bool isDesc = filters.SortDirection?.ToUpper() == "DESC";
            query = filters.SortColumn.ToLower() switch
            {
                "diseaseName" => isDesc ? query.OrderByDescending(d => d.DiseaseName) : query.OrderBy(d => d.DiseaseName),
                // لو عندك تاريخ إضافة للمرض
                // "date" => isDesc ? query.OrderByDescending(d => d.CreatedAt) : query.OrderBy(d => d.CreatedAt),
                _ => query.OrderBy(d => d.DiseaseName) // الديفولت لو بعت عمود غلط (ترتيب أبجدي)
            };
        }
        else
        {
            // الديفولت لو مبعتش ترتيب خالص (ترتيب أبجدي زي ما كنت عامل)
            query = query.OrderBy(d => d.DiseaseName);
        }

        // 4. المابينج باستخدام AutoMapper
        var projectedQuery = query.ProjectTo<DiseaseResponse>(mapper.ConfigurationProvider);

        // 5. تطبيق الـ Pagination الجاهز
        var paginatedDiseases = await PaginatedList<DiseaseResponse>.CreateAsync(
            projectedQuery,
            filters.PageNumber,
            filters.PageSize,
            cancellationToken);

        return paginatedDiseases;
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

        // 👈 هياخد القيمة المبعوتة في الريكويست (لو مبعتش حاجة هتبقى false)
        //diseaseEntity.IsSupportedByAi = request.IsSupportedByAi;

        await unitOfWork.DiseaseRepository.CreateAsync(diseaseEntity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // بنرجع المرض كامل بالـ ID الجديد بتاعه
        return mapper.Map<DiseaseResponse>(diseaseEntity);
    }

}


#region Add Diseases service with Admin Approvaled
//public bool IsApproved { get; set; } = false;

// --- 1. جلب الأمراض المعتمدة فقط (عشان تظهر للمرضى والعطارين) ---
//public async Task<IEnumerable<DiseaseResponse>> GetAllDiseasesAsync(CancellationToken cancellationToken = default)
//{
//    var diseases = await unitOfWork.DiseaseRepository.GetAllAsync(
//        filter: d => d.IsApproved == true, // 👈 التعديل هنا
//        tracked: false,
//        cancellationToken: cancellationToken);

//    var mappedDiseases = mapper.Map<IEnumerable<DiseaseResponse>>(diseases);
//    return mappedDiseases.OrderBy(d => d.DiseaseName).ToList();
//}

//// --- 2. جلب أسماء الأمراض المعتمدة فقط (للـ Dropdown) ---
//public async Task<IEnumerable<DiseaseNamesResponse>> GetDiseasesNameAsync(CancellationToken cancellationToken = default)
//{
//    var diseases = await unitOfWork.DiseaseRepository.GetAllAsync(
//        filter: d => d.IsApproved == true, // 👈 التعديل هنا
//        tracked: false,
//        cancellationToken: cancellationToken);

//    var mapped = mapper.Map<IEnumerable<DiseaseNamesResponse>>(diseases);
//    return mapped.OrderBy(d => d.DiseaseName).ToList();
//}

//// --- 3. جلب الأمراض المعلقة (عشان الأدمن يراجعها) ---
//public async Task<IEnumerable<DiseaseResponse>> GetPendingDiseasesAsync(CancellationToken cancellationToken = default)
//{
//    var diseases = await unitOfWork.DiseaseRepository.GetAllAsync(
//        filter: d => d.IsApproved == false,
//        tracked: false,
//        cancellationToken: cancellationToken);

//    return mapper.Map<IEnumerable<DiseaseResponse>>(diseases);
//}

//// --- 4. العطار بيقترح مرض جديد ---
//public async Task<DiseaseResponse> ProposeDiseaseAsync(CreateDiseaseRequest request, CancellationToken cancellationToken = default)
//{
//    string cleanName = request.DiseaseName.Trim();

//    var existingDisease = await unitOfWork.DiseaseRepository.GetAsync(
//        d => d.DiseaseName.ToLower() == cleanName.ToLower(),
//        tracked: false,
//        cancellationToken: cancellationToken);

//    if (existingDisease != null)
//        throw new Exception("This disease already exists in the system.");

//    var diseaseEntity = mapper.Map<Disease>(request);
//    diseaseEntity.DiseaseName = cleanName;
//    diseaseEntity.IsSupportedByAi = false; // العطار ملوش دعوة بالـ AI
//    diseaseEntity.IsApproved = false;      // 👈 مستني موافقة الأدمن

//    await unitOfWork.DiseaseRepository.CreateAsync(diseaseEntity, cancellationToken);
//    await unitOfWork.SaveChangesAsync(cancellationToken);

//    return mapper.Map<DiseaseResponse>(diseaseEntity);
//}

//// --- 5. الأدمن بيضيف مرض (AI أو عادي) ---
//public async Task<DiseaseResponse> AddDiseaseByAdminAsync(CreateDiseaseRequest request, bool isAiSupported, CancellationToken cancellationToken = default)
//{
//    string cleanName = request.DiseaseName.Trim();

//    var existingDisease = await unitOfWork.DiseaseRepository.GetAsync(
//        d => d.DiseaseName.ToLower() == cleanName.ToLower(),
//        tracked: false,
//        cancellationToken: cancellationToken);

//    if (existingDisease != null)
//        throw new Exception("This disease already exists.");

//    var diseaseEntity = mapper.Map<Disease>(request);
//    diseaseEntity.DiseaseName = cleanName;
//    diseaseEntity.IsSupportedByAi = isAiSupported; // الأدمن بيحدد
//    diseaseEntity.IsApproved = true;               // 👈 موافق عليه فوراً

//    await unitOfWork.DiseaseRepository.CreateAsync(diseaseEntity, cancellationToken);
//    await unitOfWork.SaveChangesAsync(cancellationToken);

//    return mapper.Map<DiseaseResponse>(diseaseEntity);
//}

//// --- 6. الأدمن بيوافق على اقتراح العطار ---
//public async Task ApproveDiseaseAsync(int diseaseId, CancellationToken cancellationToken = default)
//{
//    var disease = await unitOfWork.DiseaseRepository.GetAsync(d => d.DiseaseId == diseaseId, tracked: true, cancellationToken: cancellationToken);

//    if (disease == null) throw new Exception("Disease not found.");

//    disease.IsApproved = true;
//    await unitOfWork.SaveChangesAsync(cancellationToken);
//}

//// --- 7. الأدمن بيرفض ويمسح اقتراح العطار ---
//public async Task RejectDiseaseAsync(int diseaseId, CancellationToken cancellationToken = default)
//{
//    var disease = await unitOfWork.DiseaseRepository.GetAsync(d => d.DiseaseId == diseaseId, tracked: true, cancellationToken: cancellationToken);

//    if (disease == null) throw new Exception("Disease not found.");

//    unitOfWork.DiseaseRepository.Remove(disease);
//    await unitOfWork.SaveChangesAsync(cancellationToken);
//}
    #endregion