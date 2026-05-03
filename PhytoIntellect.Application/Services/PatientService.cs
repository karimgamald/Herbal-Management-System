using AutoMapper;
using AutoMapper.QueryableExtensions;
using PhytoIntellect.Application.Contracts.Patients;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Application.Paginations;
using PhytoIntellect.Core.Enums;

namespace PhytoIntellect.Application.Services;

public class PatientService(IUnitOfWork unitOfWork, IMapper mapper) : IPatientService
{
    public async Task<PatientRequest?> GetMyProfileAsync(int userId, CancellationToken cancellationToken = default)
    {
        var patient = await unitOfWork.PatientRepository.GetAsync(p => p.UserId == userId, tracked: false, includeProperties: "MedicalHistory", cancellationToken: cancellationToken);
        return patient == null ? null : mapper.Map<PatientRequest>(patient);
    }

    public async Task<string> UpdateMyProfileAsync(int userId, UpdatePatientRequest request, 
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
   
    public async Task<PatientRequest?> GetPatientByIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        var patient = await unitOfWork.PatientRepository.GetAsync(p => p.PatientId == patientId, tracked: false,
            includeProperties: "MedicalHistory", cancellationToken: cancellationToken);
        return patient == null ? null : mapper.Map<PatientRequest>(patient);
    }

    public async Task<PaginatedList<PatientRequest>> GetAllPatientsAsync(RequestFilters filters,
     CancellationToken cancellationToken = default)
    {
        var query = unitOfWork.PatientRepository
            .GetQueryable(tracked: false);

        // 🔍 Search
        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
        {
            var search = filters.SearchValue.ToLower();

            query = query.Where(p =>
                p.User.FullName.ToLower().Contains(search));
        }

        // 🔃 Sorting
        if (!string.IsNullOrWhiteSpace(filters.SortColumn))
        {
            bool isDesc = filters.SortDirection?.ToUpper() == "DESC";

            query = filters.SortColumn.ToLower() switch
            {
                "fullname" => isDesc
                    ? query.OrderByDescending(p => p.User.FullName)
                    : query.OrderBy(p => p.User.FullName),

                "date" => isDesc
                    ? query.OrderByDescending(p => p.User.CreatedAt)
                    : query.OrderBy(p => p.User.CreatedAt),

                _ => query.OrderBy(p => p.PatientId)
            };
        }
        else
        {
            query = query.OrderBy(p => p.PatientId);
        }

        // 🚀 Projection
        var projectedQuery = query.ProjectTo<PatientRequest>(
            mapper.ConfigurationProvider);

        // 📄 Pagination
        var result = await PaginatedList<PatientRequest>.CreateAsync(
            projectedQuery,
            filters.PageNumber,
            filters.PageSize,
            cancellationToken);

        return result;
    }
}