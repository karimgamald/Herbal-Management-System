using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhytoIntellect.Application.Contracts.Diseases;
using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Constants;

namespace PhytoIntellect.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DiseasesController(IDiseaseService diseaseService) : ControllerBase
{
    [HttpGet("all")] 
    public async Task<IActionResult> GetAllDiseases(CancellationToken cancellationToken)
    {
        var diseases = await diseaseService.GetAllDiseasesAsync(cancellationToken);
        return Ok(diseases);
    }

    [HttpGet("all-names")]
    public async Task<IActionResult> GetDiseasesName(CancellationToken cancellationToken)
    {
        var diseases = await diseaseService.GetDiseasesNameAsync(cancellationToken);
        return Ok(diseases);
    }

    [HttpPost("add")]
    [Authorize(Roles = AppRoles.Herbalist)]
    public async Task<IActionResult> CreateDisease([FromBody] CreateDiseaseRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await diseaseService.CreateDiseaseAsync(request, cancellationToken);
            return Ok(result); // بيرجعلك الـ 200 OK ومعاها بيانات المرض بالـ ID الجديد
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
}

#region Add Diseases With Admin Approvaled
// 🟢 متاح لأي حد يشوف الأمراض المعتمدة
//[HttpGet("approved")]
//public async Task<IActionResult> GetAllApprovedDiseases(CancellationToken cancellationToken)
//{
//    var diseases = await diseaseService.GetAllDiseasesAsync(cancellationToken);
//    return Ok(diseases);
//}

//// 🟢 متاح لأي حد يشوف أسامي الأمراض المعتمدة
//[HttpGet("approved-names")]
//public async Task<IActionResult> GetApprovedDiseasesName(CancellationToken cancellationToken)
//{
//    var diseases = await diseaseService.GetDiseasesNameAsync(cancellationToken);
//    return Ok(diseases);
//}

//// 👨‍⚕️ للعطار بس: يقترح مرض جديد
//[HttpPost("propose")]
//[Authorize(Roles = AppRoles.Herbalist)]
//public async Task<IActionResult> ProposeDisease([FromBody] CreateDiseaseRequest request, CancellationToken cancellationToken)
//{
//    try
//    {
//        var result = await diseaseService.ProposeDiseaseAsync(request, cancellationToken);
//        return Ok(new { Message = "Disease proposed successfully and is pending admin approval.", Data = result });
//    }
//    catch (Exception ex)
//    {
//        return BadRequest(new { Message = ex.Message });
//    }
//}

//// 👑 للأدمن بس: يشوف الأمراض المتعلقة
//[HttpGet("pending")]
//[Authorize(Roles = AppRoles.Admin)]
//public async Task<IActionResult> GetPendingDiseases(CancellationToken cancellationToken)
//{
//    var diseases = await diseaseService.GetPendingDiseasesAsync(cancellationToken);
//    return Ok(diseases);
//}

//// 👑 للأدمن بس: يضيف مرض من عنده ويحدد هو تبع الـ AI ولا لأ
//[HttpPost("add-admin")]
//[Authorize(Roles = AppRoles.Admin)]
//public async Task<IActionResult> AddDiseaseByAdmin([FromBody] CreateDiseaseRequest request, [FromQuery] bool isAiSupported, CancellationToken cancellationToken)
//{
//    try
//    {
//        var result = await diseaseService.AddDiseaseByAdminAsync(request, isAiSupported, cancellationToken);
//        return Ok(result);
//    }
//    catch (Exception ex)
//    {
//        return BadRequest(new { Message = ex.Message });
//    }
//}

//// 👑 للأدمن بس: يوافق على مرض
//[HttpPut("{id}/approve")]
//[Authorize(Roles = AppRoles.Admin)]
//public async Task<IActionResult> ApproveDisease(int id, CancellationToken cancellationToken)
//{
//    try
//    {
//        await diseaseService.ApproveDiseaseAsync(id, cancellationToken);
//        return Ok(new { Message = "Disease approved successfully." });
//    }
//    catch (Exception ex)
//    {
//        return BadRequest(new { Message = ex.Message });
//    }
//}

//// 👑 للأدمن بس: يرفض مرض
//[HttpDelete("{id}/reject")]
//[Authorize(Roles = AppRoles.Admin)]
//public async Task<IActionResult> RejectDisease(int id, CancellationToken cancellationToken)
//{
//    try
//    {
//        await diseaseService.RejectDiseaseAsync(id, cancellationToken);
//        return Ok(new { Message = "Disease rejected and removed successfully." });
//    }
//    catch (Exception ex)
//    {
//        return BadRequest(new { Message = ex.Message });
//    }
//}
#endregion
