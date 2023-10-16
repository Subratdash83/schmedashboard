// ***********************************************************************
// Assembly         : DataUnification.Domain
// Author           : 
// Created          : 
//
// Last Modified By :  
// Last Modified On : 
// ***********************************************************************
// <copyright file="Districts.cs" company="CSM technologies">
//     Copyright ©  2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.ComponentModel.DataAnnotations.Schema;

namespace ABP.Domain.District
{
    /// <summary>
    /// District model
    /// </summary>
    [Table("M_DISTRICT")]
    public class District: BaseEntity<int>
    {
        /// <summary>
        /// District Id
        /// </summary>
        public int DISTRICT_CODE { get; set; }

        public int DISTRICTID { get; set; }
        /// <summary>
        /// District Name
        /// </summary>
        public string DISTRICT_NAME { get; set; }
        public int COUNTS { get; set; }

        public string TYPE { get; set; }
        /// <summary>
        /// State Id
        /// </summary>
        public int? StateId { get; set; }
        /// <summary>
        /// Deleted flag : 1-deleted, 0-active
        /// </summary>
        public bool bitStatus { get; set; }
        public string SOURCE_DIST_CODE { get; set; }
        public string SOURCE_DIST_NAME { get; set; }
        public string CENSUS_2001_CODE { get; set; }
        public string CENSUS_2011_CODE { get; set; }
        /// <summary>
        /// Ovveride the Primary key of the entity
        /// </summary>
        public override int Key
        {
            get { return DISTRICT_CODE; }
            set { DISTRICT_CODE = value; }
        }
    }
}
