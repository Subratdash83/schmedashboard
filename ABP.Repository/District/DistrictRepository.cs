using ABP.Domain;
using ABP.Domain.Frequency;
using ABP.Domain.Indicator;
using ABP.Domain.Report;
using ABP.Repository;
using ABP.Repository.Contract.Contract.District;
using ABP.Repository.Contract.Factory;
using Dapper;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Repository.District
{
    /// <summary>
    /// DistrictRepository Repository
    /// </summary>
    /// See also <see cref="FTP.Repository.RepositoryBase" />, <see cref="FTP.Repository.Login.LoginRepository" />
    public class DistrictRepository : RepositoryBase, IDistrictRepository
    {
        private readonly ILogger<DistrictRepository> Logger;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionFactory">Connection Factory</param>
        /// See also <see cref="DataUnification.Repository.Contract.Factory.IConnectionFactory" />
        public DistrictRepository(IConnectionFactory connectionFactory, ILogger<DistrictRepository> logger) : base(connectionFactory)
        {
            Logger = logger;
            Logger.LogInformation("DistrictRepository initialized");
        }
        public string InsertDistrict(Domain.District.District Model)
        {
            string strOutput = "";
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var p = new OracleDynamicParameters();
                p.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "Save");
                p.Add("P_DISTRICTID", OracleDbType.Int32, ParameterDirection.Input, Model.DISTRICTID);
                p.Add("P_DISTRICTCODE", OracleDbType.Varchar2, ParameterDirection.Input, Model.DISTRICT_CODE);
                p.Add("P_DISTRICTNAME", OracleDbType.Varchar2, ParameterDirection.Input, Model.DISTRICT_NAME);
                p.Add("P_CENSUS_2001_CODE", OracleDbType.Varchar2, ParameterDirection.Input, Model.CENSUS_2001_CODE);
                p.Add("P_CENSUS_2011_CODE", OracleDbType.Varchar2, ParameterDirection.Input, Model.CENSUS_2011_CODE);
                p.Add("P_CREATEDBY", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(0));
                p.Add("P_UPDATEBY", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(0));
                p.Add("P_Msg", OracleDbType.RefCursor, direction: ParameterDirection.Output);
                var query = "USP_DUT_DISTRICT";
                SuccessMessages = Connection.Query<SuccessMessage>(query, p, commandType: CommandType.StoredProcedure);
                strOutput = SuccessMessages.AsList()[0].successid.ToString();
                // Connection.Query<int>("USP_USERREGISTRATION", p, commandType: CommandType.StoredProcedure);
                // strOutput = "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return strOutput;
        }
        public async Task<IEnumerable<Domain.District.District>> GetDistrictById(int DISTRICTID)
        {


            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_DISTRICTID", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(DISTRICTID));
                dyParam.Add("P_Action", OracleDbType.Varchar2, ParameterDirection.Input, "E");
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_DISTRICT_View";
                var result = await Connection.QueryAsync<Domain.District.District>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.District.District>> GetDistrictDetails(Domain.District.District search)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();

                dyParam.Add("P_Action", OracleDbType.Varchar2, ParameterDirection.Input, "V");
                dyParam.Add("P_Search", OracleDbType.Varchar2, ParameterDirection.Input, search.DISTRICT_NAME);


                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_DISTRICT_View";
                var result = await Connection.QueryAsync<Domain.District.District>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string DeleteDistrict(Domain.District.District Model)
        {
            string strOutput = "";
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var p = new OracleDynamicParameters();
                p.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "D");
                p.Add("P_DISTRICTID", OracleDbType.Int32, ParameterDirection.Input, Model.DISTRICTID);
                p.Add("P_Msg", OracleDbType.RefCursor, direction: ParameterDirection.Output);
                var query = "USP_DUT_DISTRICT";
                SuccessMessages = Connection.Query<SuccessMessage>(query, p, commandType: CommandType.StoredProcedure);
                strOutput = SuccessMessages.AsList()[0].successid.ToString();
                // Connection.Query<int>("USP_USERREGISTRATION", p, commandType: CommandType.StoredProcedure);
                // strOutput = "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return strOutput;
        }
        public async Task<IEnumerable<Domain.Report.DistReport>> GetDistrictAsync()
        {

            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Varchar2, ParameterDirection.Input, "GD");
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BLOCKSIDBYDISTID";
                var result = await Connection.QueryAsync<Domain.Report.DistReport>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

        public async Task<IEnumerable<Domain.Block.Block>> GetBlockByDist(int DistCode)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Varchar2, ParameterDirection.Input, "GB");
                dyParam.Add("P_DISTRICTID", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(DistCode));
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BLOCKSIDBYDISTID";
                var result = await Connection.QueryAsync<Domain.Block.Block>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

        public async Task<IEnumerable<Domain.Block.Block>> GetUserAndBlockByDist(int DistCode)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Varchar2, ParameterDirection.Input, "GUID");
                dyParam.Add("P_DISTRICTID", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(DistCode));
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BLOCKSIDBYDISTID";
                var result = await Connection.QueryAsync<Domain.Block.Block>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

        public async Task<IEnumerable<Domain.Login.Login>> GetByBlockDetails(int DistCode)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Varchar2, ParameterDirection.Input, "GUB");
                dyParam.Add("P_DISTRICTID", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(DistCode));
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BLOCKSIDBYDISTID";
                var result = await Connection.QueryAsync<Domain.Login.Login>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

        public async Task<IEnumerable<Domain.Login.Login>> GetByDistrictDetails()
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Varchar2, ParameterDirection.Input, "GUD");
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BLOCKSIDBYDISTID";
                var result = await Connection.QueryAsync<Domain.Login.Login>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

        public async Task<IEnumerable<DistReport>> GetByDistrictDataDetails(int year, int FREQUENCYNO)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Varchar2, ParameterDirection.Input, "GUS");
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, year);
                dyParam.Add("P_MONTH", OracleDbType.Int32, ParameterDirection.Input, FREQUENCYNO);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BLOCKSIDBYDISTID";
                var result = await Connection.QueryAsync<Domain.Report.DistReport>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
        public async Task<IEnumerable<DistReport>> IndicatorScore(int distcode, int FREQUENCYVALUE, int FREQUENCYNO)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Varchar2, ParameterDirection.Input, "A");
                dyParam.Add("P_distid", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(distcode));
                dyParam.Add("P_YEAR", OracleDbType.Varchar2, ParameterDirection.Input, FREQUENCYNO);
                dyParam.Add("P_MONTH", OracleDbType.Varchar2, ParameterDirection.Input, FREQUENCYVALUE);
                dyParam.Add("P_OUT", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_SECTORWISE_INDICATOR";
                var result = await Connection.QueryAsync<Domain.Report.DistReport>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }




        public async Task<IEnumerable<DistReport>> getblockdata(int distcode)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Varchar2, ParameterDirection.Input, "BP");
                dyParam.Add("P_DISTRICTID", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(distcode));
                //dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(BlockCode));
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BLOCKSIDBYDISTID";
                var result = await Connection.QueryAsync<Domain.Report.DistReport>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

        public async Task<IEnumerable<IndicatorValueScore>> GetAppnoForCalculation(int distcode)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Varchar2, ParameterDirection.Input, "APPNO");
                dyParam.Add("P_distid", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(distcode));
                //dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(BlockCode));
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_INDICATORSCORE";
                var result = await Connection.QueryAsync<Domain.Indicator.IndicatorValueScore>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
       
        public async Task<int> INSERTSCORE(IndicatorScoreXML ivs)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "SCR");
                dyParam.Add("P_XMLDATA", OracleDbType.XmlType, ParameterDirection.Input, ivs.indicatorvaluexml);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_INDICATORSCORE";
              
                var result = await Connection.ExecuteAsync(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
        public async Task<int> YEARLYINSERTSCORE(IndicatorScoreXML ivs)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "SCR");
                dyParam.Add("P_XMLDATA", OracleDbType.XmlType, ParameterDirection.Input, ivs.indicatorvaluexml);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                //var query = "USP_ABP_INDICATORSCORE";
                var query = "USP_ABP_YEARLYINDICATORSCORE";
                var result = await Connection.ExecuteAsync(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
        public async Task<int> INSERTAnalyticData(int Pdist, int Pblock, int Pyear, int Pmonth, int Pfrquency)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "INS");
                dyParam.Add("P_DISTRICTID", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(Pdist));
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(Pblock));
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(Pyear));
                dyParam.Add("P_DPFREQUENCYNO", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(Pmonth));
                dyParam.Add("P_FREQUENCYID", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(Pfrquency));
               // dyParam.Add("P_MSG", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_ANALYTICSVIEW";
                var result = await Connection.ExecuteAsync(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
        public async Task<int> INSERTAnalyticDataCopy(int Pdist, int Pblock, int Pyear, int Pmonth, int Pfrquency)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "INS");
                dyParam.Add("P_DISTRICTID", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(Pdist));
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(Pblock));
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(Pyear));
                dyParam.Add("P_DPFREQUENCYNO", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(Pmonth));
                dyParam.Add("P_FREQUENCYID", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(Pfrquency));
                // dyParam.Add("P_MSG", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_ANALYTICSVIEW_COPYTEST";
                var result = await Connection.ExecuteAsync(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
        public async Task<int> INSERTAnalyticDataCopyYearly(int Pdist, int Pblock, int Pyear, int Pmonth, int Pfrquency)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "INSYEARLY");
                dyParam.Add("P_DISTRICTID", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(Pdist));
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(Pblock));
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(Pyear));
                dyParam.Add("P_DPFREQUENCYNO", OracleDbType.Int32, ParameterDirection.Input, Pyear);
                dyParam.Add("P_FREQUENCYID", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(Pfrquency));
                // dyParam.Add("P_MSG", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_ANALYTICSVIEW_COPYTEST";
                var result = await Connection.ExecuteAsync(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

        public async Task<int> INSERTAnalyticDataYearly(int Pdist, int Pblock, int Pyear, int Pfrquency)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "INS");
                dyParam.Add("P_DISTRICTID", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(Pdist));
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(Pblock));
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(Pyear));
                dyParam.Add("P_DPFREQUENCYNO", OracleDbType.Int32, ParameterDirection.Input, Pyear);
                dyParam.Add("P_FREQUENCYID", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(Pfrquency));
                // dyParam.Add("P_MSG", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_ANALYTICSVIEW";
                var result = await Connection.ExecuteAsync(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
        public async Task<IEnumerable<IndicatorValueScore>> GetAvgIndicatorScore(int distcode)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Varchar2, ParameterDirection.Input, "BP");               
                dyParam.Add("P_distid", OracleDbType.Int32, ParameterDirection.Input, distcode);             
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_INDICATORSCORE";
                var result = await Connection.QueryAsync<Domain.Indicator.IndicatorValueScore>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

        public async Task<IEnumerable<Domain.Report.DistReport>> GetDepartmentAsync()
        {

            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Varchar2, ParameterDirection.Input, "GD");
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BLOCKSIDBYDISTID";
                var result = await Connection.QueryAsync<Domain.Report.DistReport>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

        public async Task<IEnumerable<Domain.Designation.Designation>> GetUserAndDesgByDept(int Deptid)
        {
            try
            {
                //***P_DISTRICTID refers to Department Name******//
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Varchar2, ParameterDirection.Input, "DUID");
                dyParam.Add("P_DISTRICTID", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(Deptid));
                
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BLOCKSIDBYDISTID";
                var result = await Connection.QueryAsync<Domain.Designation.Designation>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.Designation.Designation>> GetUserAndColByDist(int DistId)
        {
            try
            {
                //***P_DISTRICTID refers to Department Name******//
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Varchar2, ParameterDirection.Input, "CUID");
                dyParam.Add("P_DISTRICTID", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(DistId));

                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BLOCKSIDBYDISTID";
                var result = await Connection.QueryAsync<Domain.Designation.Designation>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

        public async Task<int> UPDATESCORQUERYE(Domain.Indicator.IndicatorValueScore ivs)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "UPD");
                dyParam.Add("P_XMLDATA", OracleDbType.XmlType, ParameterDirection.Input, ivs.BLOCKID);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_INDICATORSCORE";
                var result = await Connection.ExecuteAsync(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

        public async Task<IEnumerable<Domain.Block.Block>> GetAllDist(int DistCode)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Varchar2, ParameterDirection.Input, "DIST");
                dyParam.Add("P_DISTRICTID", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(DistCode));
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BLOCKSIDBYDISTID";
                var result = await Connection.QueryAsync<Domain.Block.Block>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.Block.Block>> GetDistData(int DistCode)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Varchar2, ParameterDirection.Input, "DIST");
                dyParam.Add("P_DISTRICTID", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(DistCode));
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BLOCKSIDBYDISTID";
                var result = await Connection.QueryAsync<Domain.Block.Block>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
        public async Task<int> SelectYearlyYeare(int Pblock, int Pyear)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "INS");
                
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(Pblock));
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(Pyear));
               
                var query = "USP_ABP_ANALYTICSVIEW";
                var result = await Connection.ExecuteAsync(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.Report.Report>> GetDPMonCount()
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "DPMON");
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BLOCKSIDBYDISTID";
                var result = await Connection.QueryAsync<Domain.Report.Report>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                // Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
       
        public async Task<int> INSERTAspirationalDetail(Domain.Report.Report b)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "ASUPT");
                dyParam.Add("P_blockid", OracleDbType.Int32, ParameterDirection.Input, b.BlockId);
                dyParam.Add("P_MAPPEDBLOCK", OracleDbType.Varchar2, ParameterDirection.Input, b.MAPPEDBLOCK);
                dyParam.Add("P_result", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_REPORT_DASHBOARD";
                var result = await Connection.ExecuteAsync(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

        public async Task<IEnumerable<Domain.Report.Report>> ViewAspirationalBlock(Domain.Report.Report rpt)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "VIEWDB");
                dyParam.Add("P_distid", OracleDbType.Int32, ParameterDirection.Input, rpt.DistrictId);
                dyParam.Add("P_Aspirationblock", OracleDbType.Int32, ParameterDirection.Input, rpt.Aspirationblock);
                dyParam.Add("P_result", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_REPORT_DASHBOARD";
                var result = await Connection.QueryAsync<Domain.Report.Report>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Domain.Report.Report>> GetAspirationalid(int Blockid)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Varchar2, ParameterDirection.Input, "GETID");
                dyParam.Add("P_blockid", OracleDbType.Int32, ParameterDirection.Input, Blockid);
                dyParam.Add("P_result", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_REPORT_DASHBOARD";
                var result = await Connection.QueryAsync<Domain.Report.Report>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

        public async Task<IEnumerable<Domain.Report.Report>> GetMappedeBlockByDist(int DistId)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "GET");
                dyParam.Add("P_distid", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(DistId));
                dyParam.Add("P_result", OracleDbType.RefCursor, direction: ParameterDirection.Output);
                var query = "USP_REPORT_DASHBOARD";
                var result = await Connection.QueryAsync<Domain.Report.Report>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string GetMobNo(string username)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "MOBDET");
                dyParam.Add("P_USERNAME", OracleDbType.Varchar2, ParameterDirection.Input, username);
                var query = "USP_ABP_SMS_SEND";
                var result = Connection.QueryFirstOrDefault<string>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

