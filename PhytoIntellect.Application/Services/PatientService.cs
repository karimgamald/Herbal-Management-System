using AutoMapper;
using PhytoIntellect.Application.DTOs.PatientDTOs;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Enums;

namespace PhytoIntellect.Application.Services;

public class PatientService(IUnitOfWork unitOfWork, IMapper mapper) : IPatientService
{
    public async Task<PatientDto?> GetMyProfileAsync(int userId, CancellationToken cancellationToken = default)
    {
        var patient = await unitOfWork.PatientRepository.GetAsync(p => p.UserId == userId, tracked: false, includeProperties: "MedicalHistory", cancellationToken: cancellationToken);
        return patient == null ? null : mapper.Map<PatientDto>(patient);
    }

    public async Task<string> UpdateMyProfileAsync(int userId, UpdatePatientDto request, 
        CancellationToken cancellationToken = default)
    {
        var patient = await unitOfWork.PatientRepository.GetAsync(p => p.UserId == userId, tracked: true, includeProperties: "MedicalHistory", cancellationToken: cancellationToken);
        if (patient == null)
            return "Patient profile not found.";

        patient.BirthDate = DateOnly.Parse(request.BirthDate);
        patient.Gender = Enum.Parse<Gender>(request.Gender, true);

        unitOfWork.PatientRepository.Update(patient);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return "Profile updated successfully.";
    }
   
    public async Task<PatientDto?> GetPatientByIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        var patient = await unitOfWork.PatientRepository.GetAsync(p => p.PatientId == patientId, tracked: false,
            includeProperties: "MedicalHistory", cancellationToken: cancellationToken);
        return patient == null ? null : mapper.Map<PatientDto>(patient);
    }

    public async Task<IEnumerable<PatientDto>> GetAllPatientsAsync(CancellationToken cancellationToken = default)
    {
        var patients = await unitOfWork.PatientRepository.GetAllAsync(tracked: false, includeProperties: "MedicalHistory", cancellationToken: cancellationToken);
        return mapper.Map<IEnumerable<PatientDto>>(patients);
    }
}