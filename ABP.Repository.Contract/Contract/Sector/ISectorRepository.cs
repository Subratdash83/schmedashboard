using ABP.Domain.Panel;
using ABP.Domain.Sector;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Repository.Contract.Contract.Sector
{
    public interface ISectorRepository : IDisposable, IRepository
    {
        string InsertSector(ABP.Domain.Sector.sector sector); 

        Task<IEnumerable<ABP.Domain.Sector.sector>> ViewSector();

        string DeleteSector(sector sector);

        Task<IEnumerable<sector>> SectorGateById(int id);
       

        
    }
}
