using AutoMapper;
using PhytoIntellect.Application.DTOs.PatientDTOs;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Entities;
using PhytoIntellect.Core.Enums;
using PhytoIntellect.Core.Interfaces;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace PhytoIntellect.Application.Services;

public class PatientService(IUnitOfWork unitOfWork, IMapper mapper) : IPatientService
{
    public async Task<PatientDto?> GetMyProfileAsync(int userId, CancellationToken cancellationToken = default)
    {
        var patient = await unitOfWork.PatientRepository.GetAsync(p => p.UserId == userId, tracked: false, cancellationToken);
        return patient == null ? null : mapper.Map<PatientDto>(patient);
    }

    public async Task<string> UpdateMyProfileAsync(int userId, UpdatePatientDto request, 
        CancellationToken cancellationToken = default)
    {
        var patient = await unitOfWork.PatientRepository.GetAsync(p => p.UserId == userId, tracked: true, cancellationToken);
        if (patient == null)
            return "Patient profile not found.";

        //patient.BirthDate = request.BirthDate;
        //patient.Gender = request.Gender; // Casting

        patient.BirthDate = DateOnly.Parse(request.BirthDate);
        patient.Gender = Enum.Parse<Gender>(request.Gender, true);

        unitOfWork.PatientRepository.Update(patient);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return "Profile updated successfully.";
    }
   
    public async Task<PatientDto?> GetPatientByIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        var patient = await unitOfWork.PatientRepository.GetAsync(p => p.PatientId == patientId, tracked: false, 
            cancellationToken);
        return patient == null ? null : mapper.Map<PatientDto>(patient);
    }

    public async Task<IEnumerable<PatientDto>> GetAllPatientsAsync(CancellationToken cancellationToken = default)
    {
        var patients = await unitOfWork.PatientRepository.GetAllAsync(tracked: false, cancellationToken: cancellationToken);
        return mapper.Map<IEnumerable<PatientDto>>(patients);
    }
}