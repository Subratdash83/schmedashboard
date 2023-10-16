using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Repository.Contract.DistDetailReport
{
  public interface IDistDetailRepository
    {
       // Task<IEnumerable<Domain.District.District>> GetDistrictAsync();

      //  List<Domain.Login.Login> GetAllBlockUserAsync();

        //Task<IEnumerable<Domain.Block.Block>> GetUserAndBlockByDist(int DistCode);

        Task<IEnumerable<Domain.Report.DistReport>> GetBlockByDistrict(int DistCode);
        List<Domain.Report.DistReport> GetAllDistrictUserAsync();

        Task<IEnumerable<Domain.Report.DistReport>> GetByDistrictDataDetails(int DistCode);
    }
}
