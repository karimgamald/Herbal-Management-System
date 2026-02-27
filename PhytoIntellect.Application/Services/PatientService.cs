using AutoMapper;
using PhytoIntellect.Application.DTOs.PatientDTOs;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PhytoIntellect.Application.Services;

public class PatientService(IUnitOfWork unitOfWork, IMapper mapper) : IPatientService
{
    public async Task<string> CreatePatientAsync(CreatePatientDto request, CancellationToken cancellationToken = default)
    {
        var patient = mapper.Map<Patient>(request);

        await unitOfWork.PatientRepository.CreateAsync(patient, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return "Patient created successfully.";
    }

    public async Task<PatientDto?> GetPatientByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        // tracked: false عشان إحنا بنقرا بس ومش هنعدل
        var patient = await unitOfWork.PatientRepository.GetAsync(p => p.PatientId == id, tracked: false, cancellationToken);
        if (patient == null) return null;

        return mapper.Map<PatientDto>(patient);
    }

    public async Task<IEnumerable<PatientDto>> GetAllPatientsAsync(CancellationToken cancellationToken = default)
    {
        var patients = await unitOfWork.PatientRepository.GetAllAsync(tracked: false, cancellationToken: cancellationToken);
        return mapper.Map<IEnumerable<PatientDto>>(patients);
    }

    public async Task<string> UpdatePatientAsync(int id, UpdatePatientDto request, CancellationToken cancellationToken = default)
    {
        // tracked: true عشان الـ Entity Framework يحس بالتعديل
        var patient = await unitOfWork.PatientRepository.GetAsync(p => p.PatientId == id, tracked: true, cancellationToken);
        if (patient == null) return "Patient not found.";

        patient.BirthDate = request.BirthDate;
        patient.Gender = (PhytoIntellect.Core.Enums.Gender)request.Gender;

        // مفيش await هنا لأن الميثود void في الـ Repo بتاعك
        unitOfWork.PatientRepository.Update(patient);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return "Patient updated successfully.";
    }

    public async Task<string> DeletePatientAsync(int id, CancellationToken cancellationToken = default)
    {
        var patient = await unitOfWork.PatientRepository.GetAsync(p => p.PatientId == id, tracked: true, cancellationToken);
        if (patient == null) return "Patient not found.";

        // مفيش await هنا برضه
        unitOfWork.PatientRepository.Remove(patient);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return "Patient deleted successfully.";
    }
}