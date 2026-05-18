using AutoMapper;
using PhytoIntellect.Application.Contracts.MedicalHistories;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Entities;
using PhytoIntellect.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace PhytoIntellect.Application.Services;

public class MedicalHistoryService(IUnitOfWork unitOfWork, IMapper mapper) : IMedicalHistoryService
{
    public async Task<MedicalHistoryResponse?> GetMyMedicalHistoryAsync(int userId, CancellationToken cancellationToken = default)
    {
        // 1. نجيب الـ PatientId بتاع اليوزر ده
        var patient = await unitOfWork.PatientRepository.GetAsync(p => p.UserId == userId, tracked: false, cancellationToken: cancellationToken);
        if (patient == null) return null;

        // 2. ندور في جدول التاريخ المرضي مباشرة بالـ PatientId
        var history = await unitOfWork.MedicalHistoryRepository.GetAsync(h => h.PatientId == patient.PatientId, tracked: false, cancellationToken: cancellationToken);

        return mapper.Map<MedicalHistoryResponse>(history);
    }

    public async Task<MedicalHistoryResponse?> GetPatientMedicalHistoryByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        // بما إننا معانا الـ PatientId جاهز، هندور بيه في جدول التاريخ المرضي على طول (أسرع بكتير)
        var history = await unitOfWork.MedicalHistoryRepository.GetAsync(h => h.PatientId == patientId, tracked: false, cancellationToken: cancellationToken);

        if (history == null) return null;

        return mapper.Map<MedicalHistoryResponse>(history);
    }
    public async Task<string> AddOrUpdateMyMedicalHistoryAsync(int userId, MedicalHistoryRequest request, CancellationToken cancellationToken = default)
    {
        // 1. نجيب المريض عشان محتاجين الـ PatientId بتاعه
        var patient = await unitOfWork.PatientRepository.GetAsync(p => p.UserId == userId, tracked: false, cancellationToken: cancellationToken);
        if (patient == null) return "Patient profile not found.";

        // 2. ندور هل المريض ده ليه تاريخ مرضي متسجل قبل كده ولا لأ؟
        var existingHistory = await unitOfWork.MedicalHistoryRepository.GetAsync(h => h.PatientId == patient.PatientId, tracked: true, cancellationToken: cancellationToken);

        // الحالة الأولى: ملوش تاريخ مرضي (Create)
        if (existingHistory == null)
        {
            var newHistory = mapper.Map<MedicalHistory>(request);

            // السحر هنا: بنربط التاريخ المرضي الجديد بالمريض عن طريق الـ FK
            newHistory.PatientId = patient.PatientId;

            await unitOfWork.MedicalHistoryRepository.CreateAsync(newHistory, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return "Medical history created successfully.";
        }

        // الحالة التانية: ليه تاريخ مرضي وبيعدله (Update)
        else
        {
            mapper.Map(request, existingHistory); // بنحدث الداتا القديمة بالجديدة

            unitOfWork.MedicalHistoryRepository.Update(existingHistory);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return "Medical history updated successfully.";
        }
    }
    public async Task<bool> DeleteMedicalHistoryByAdminAsync(int patientId, CancellationToken cancellationToken = default)
    {
        // جلب السجل مع تفعيل الـ Tracking للحذف
        var history = await unitOfWork.MedicalHistoryRepository.GetAsync(h => h.PatientId == patientId, tracked: true, cancellationToken: cancellationToken);
        if (history == null) return false;

        unitOfWork.MedicalHistoryRepository.Remove(history);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
