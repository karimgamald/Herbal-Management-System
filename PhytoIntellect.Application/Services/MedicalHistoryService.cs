using AutoMapper;
using PhytoIntellect.Application.DTOs.MedicalHistoryDTOs;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Entities;
using PhytoIntellect.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace PhytoIntellect.Application.Services;

public class MedicalHistoryService(IUnitOfWork unitOfWork, IMapper mapper) : IMedicalHistoryService
{
    public async Task<MedicalHistoryDto?> GetMyMedicalHistoryAsync(int userId, CancellationToken cancellationToken = default)
    {
        // 1. نجيب المريض الأول بالـ UserId
        var patient = await unitOfWork.PatientRepository.GetAsync(p => p.UserId == userId, tracked: false, cancellationToken);

        // لو مفيش مريض أو لسه معملش تاريخ مرضي، نرجع null
        if (patient == null || patient.MedicalHistoryId == null) return null;

        // 2. نجيب التاريخ المرضي بتاعه
        var history = await unitOfWork.MedicalHistoryRepository.GetAsync(h => h.MedicalHistoryId == patient.MedicalHistoryId, tracked: false, cancellationToken);

        return mapper.Map<MedicalHistoryDto>(history);
    }

    public async Task<string> AddOrUpdateMyMedicalHistoryAsync(int userId, ManageMedicalHistoryDto request, CancellationToken cancellationToken = default)
    {
        // لازم tracked: true عشان لو عدلنا الـ Patient يسمع في الداتابيز
        var patient = await unitOfWork.PatientRepository.GetAsync(p => p.UserId == userId, tracked: true, cancellationToken);
        if (patient == null) return "Patient profile not found.";

        // الحالة الأولى: المريض لسه معملش تاريخ مرضي خالص (Create)
        if (patient.MedicalHistoryId == null)
        {
            var newHistory = mapper.Map<MedicalHistory>(request);

            // بنحفظ التاريخ المرضي الأول عشان ياخد ID من الداتابيز
            await unitOfWork.MedicalHistoryRepository.CreateAsync(newHistory, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            // نربط الـ ID الجديد بالمريض
            patient.MedicalHistoryId = newHistory.MedicalHistoryId;
            unitOfWork.PatientRepository.Update(patient);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return "Medical history created successfully.";
        }

        // الحالة التانية: المريض عنده تاريخ مرضي وجاي يعدله (Update)
        else
        {
            var existingHistory = await unitOfWork.MedicalHistoryRepository.GetAsync(h => h.MedicalHistoryId == patient.MedicalHistoryId, tracked: true, cancellationToken);
            if (existingHistory == null) return "Error finding medical history.";

            // المابر هنا بياخد القيم من الريكويست يحطها في الريكورد القديم
            mapper.Map(request, existingHistory);

            unitOfWork.MedicalHistoryRepository.Update(existingHistory);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return "Medical history updated successfully.";
        }
    }
    // ضيف الميثود دي جوه الكلاس
    public async Task<MedicalHistoryDto?> GetPatientMedicalHistoryByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        var patient = await unitOfWork.PatientRepository.GetAsync(p => p.PatientId == patientId, tracked: false, cancellationToken);

        // لو المريض مش موجود أو لسه مكسل يكتب تاريخه المرضي
        if (patient == null || patient.MedicalHistoryId == null) return null;

        var history = await unitOfWork.MedicalHistoryRepository.GetAsync(h => h.MedicalHistoryId == patient.MedicalHistoryId, tracked: false, cancellationToken);

        return mapper.Map<MedicalHistoryDto>(history);
    }
}