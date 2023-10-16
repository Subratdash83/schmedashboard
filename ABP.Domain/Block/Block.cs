using System;
using System.Collections.Generic;
using System.Text;

namespace ABP.Domain.Block
{
    /// <summary>
    /// Block model
    /// </summary>
    public class Block : BaseEntity<int>
    {   

       // public int ID { get; set; }hgghghhggh
        /// <summary>
        /// District Id
        /// </summary>
        public int DISTRICT_CODE { get; set; }

        public int BLOCKID { get; set; }
        public int BLOCKCODE { get; set; }
        public int DISTRICTID { get; set; }
        public int DISTRICTCODE { get; set; } 
        public string BLOCK_ID { get; set; }
        public string USERNAME { get; set; }

    /// <summary>
    /// District Name
    /// </summary>
        public string DISTRICT_NAME { get; set; }
        /// <summary>
        /// Block Id
        /// </summary>
        public int BLOCK_CODE { get; set; }

        /// <summary>
        /// Block Name
        /// </summary>
        public string BLOCK_NAME { get; set; }
       
        /// <summary>
        /// Deleted flag : 1-deleted, 0-active
        /// </summary>
        public bool bitStatus { get; set; }
        
        public string CENSUS_2011_CODE { get; set;}
       public string SOURCE_BLOCK_CODE { get; set; }
        public string SOURCE_BLOCK_NAME { get; set; }

        public int SCHEMEID { get; set; }

        public int BLOCKCOUNT { get; set; }
        public int MAPBLOCKCOUNT { get; set; }
        public int AUTOMAPBLOCKCOUNT { get; set; }

        /// <summary>
        /// Ovveride the Primary key of the entity
        /// </summary>
        public override int Key
        {
            get { return BLOCK_CODE; }
            set { BLOCK_CODE = value; }
        }
    }
}
