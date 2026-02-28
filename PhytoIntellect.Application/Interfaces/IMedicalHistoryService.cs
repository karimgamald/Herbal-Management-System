using PhytoIntellect.Application.DTOs.MedicalHistoryDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Interfaces;

public interface IMedicalHistoryService
{
    Task<MedicalHistoryDto?> GetMyMedicalHistoryAsync(int userId, CancellationToken cancellationToken = default);
    Task<string> AddOrUpdateMyMedicalHistoryAsync(int userId, ManageMedicalHistoryDto request, CancellationToken cancellationToken = default);
    // ضيف السطر ده تحت الميثودز اللي كتبناها
    Task<MedicalHistoryDto?> GetPatientMedicalHistoryByPatientIdAsync(int patientId, CancellationToken cancellationToken = default);
}