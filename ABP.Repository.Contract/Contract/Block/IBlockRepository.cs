using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ABP.Repository.Contract.Block
{

    /// <summary>
    /// Interface IBlockRepository 
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    /// <seealso cref="FTP.Repository.Contract.IRepository" />
    public interface IBlockRepository : IDisposable, IRepository
    {
        /// <summary>
        /// Get Login Detail
        /// </summary>
        /// <param name="LoginClosure">Login Request ParaMeter</param>
        /// <returns><seealso cref="Domain.Block.Block"/></returns>       
        string InsertBlock(XElement Xml,int DistCode);
        Task<IEnumerable<Domain.Block.Block>> GetBlockByDist(string ACTION,int DistCode);
        Task<IEnumerable<Domain.Block.Block>> GetMappedeBlockByDist(int DistCode);

        string DeleteBlock(int DistCode);
        Task<IEnumerable<Domain.District.District>> GetBlockDetailsByID(int DostCode);

        Task<IEnumerable<Domain.Block.Block>> GetBlockByDistModal(int DostCode);

        Task<IEnumerable<Domain.Block.Block>> GetBlockAsync();
        Task<IEnumerable<Domain.Block.Block>> GetBlockByDistId(int DistID);
        Task<IEnumerable<Domain.CollectorData.BlockData>> GetBlockDistBlock(int DistID);
        Task<IEnumerable<Domain.Block.Block>> BlockView();
        Task<IEnumerable<Domain.Block.Block>> GetBlockDetailsByBlockId(int BlockId);

        Task<IEnumerable<Domain.Block.Block>> GetBlockCount(int levelid);
        Task<IEnumerable<Domain.Block.Block>> GetMapBlockCount(int levelid);
        Task<IEnumerable<Domain.Block.Block>> GetAutoMapBlockCount(int levelid);
    }
}
