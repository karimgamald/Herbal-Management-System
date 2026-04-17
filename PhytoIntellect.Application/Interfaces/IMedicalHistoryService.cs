using PhytoIntellect.Application.Contracts.MedicalHistories;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Interfaces;

public interface IMedicalHistoryService
{
    Task<MedicalHistoryResponse?> GetMyMedicalHistoryAsync(int userId, CancellationToken cancellationToken = default);
    Task<string> AddOrUpdateMyMedicalHistoryAsync(int userId, MedicalHistoryRequest request, CancellationToken cancellationToken = default);
    // ضيف السطر ده تحت الميثودز اللي كتبناها
    Task<MedicalHistoryResponse?> GetPatientMedicalHistoryByPatientIdAsync(int patientId, CancellationToken cancellationToken = default);
}