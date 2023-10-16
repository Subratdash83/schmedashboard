using ABP.Domain;
using ABP.Repository.Contract.Contract.Department;
using ABP.Repository.Contract.Factory;
using ABP.Repository.Indicator;
using Dapper;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Repository.Department
{
    public class DepartmentRepository : RepositoryBase, IDepartmentRepository
    {
        private readonly ILogger<DepartmentRepository> Logger;
        public DepartmentRepository(IConnectionFactory connectionFactory, ILogger<DepartmentRepository> logger) : base(connectionFactory)
        {
            Logger = logger;
            Logger.LogInformation("DepartmentRepository initialized");
        }
        public string AddDepartment(Domain.Department.Department dept)
        {
            string strOutput = "";
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var dyParam = new OracleDynamicParameters();
                if (dept.ID != 0)
                {
                    dyParam.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "UPDATE");
                    dyParam.Add("P_SectorId", OracleDbType.Int32, ParameterDirection.Input, dept.SECTORID);
                    dyParam.Add("P_IndicatorId", OracleDbType.Int32, ParameterDirection.Input, dept.INDICATORID);
                    dyParam.Add("P_Id", OracleDbType.Int32, ParameterDirection.Input, dept.ID);
                    dyParam.Add("P_DeptId", OracleDbType.Varchar2, ParameterDirection.Input, dept.DEPTID);
                    dyParam.Add("P_Description", OracleDbType.Varchar2, ParameterDirection.Input, dept.DESCRIPTION);
                    dyParam.Add("P_UserId", OracleDbType.Int32, ParameterDirection.Input, dept.CREATEDBY);
                    dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                    var query = "USP_ABP_DEPARTMENTADDUPDATE";
                    SuccessMessages = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
                    strOutput = SuccessMessages.AsList()[0].successid.ToString();
                }
                else
                {
                    dyParam.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "INSERT");
                    dyParam.Add("P_SectorId", OracleDbType.Int32, ParameterDirection.Input, dept.SECTORID);
                    dyParam.Add("P_IndicatorId", OracleDbType.Int32, ParameterDirection.Input, dept.INDICATORID);
                    dyParam.Add("P_Id", OracleDbType.Int32, ParameterDirection.Input, dept.ID);
                    dyParam.Add("P_DeptId", OracleDbType.Varchar2, ParameterDirection.Input, dept.DEPTID);
                    dyParam.Add("P_Description", OracleDbType.Varchar2, ParameterDirection.Input, dept.DESCRIPTION);
                    dyParam.Add("P_UserId", OracleDbType.Int32, ParameterDirection.Input, dept.CREATEDBY);
                    dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                    var query = "USP_ABP_DEPARTMENTADDUPDATE";
                    SuccessMessages = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
                    strOutput = SuccessMessages.AsList()[0].successid.ToString();
                }
                return strOutput;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message, dept);
                throw ex;
            }
            //return strOutput;
        }

        public async Task<IEnumerable<Domain.Department.Department>> ViewDepartment(int sectorid, int deptid)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "VIEW");
                dyParam.Add("P_Sectorid", OracleDbType.Int32, ParameterDirection.Input, sectorid);
                dyParam.Add("P_Deptid", OracleDbType.Varchar2, ParameterDirection.Input, deptid);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_DEPARTMENTVIEWDELETE";
                var result = await Connection.QueryAsync<Domain.Department.Department>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string DeleteDepartment(Domain.Department.Department Model)
        {
            string strOutput = "";
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var p = new OracleDynamicParameters();
                p.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "DELETE");
                p.Add("P_Id", OracleDbType.Int32, ParameterDirection.Input, Model.ID);
                p.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_DEPARTMENTVIEWDELETE";
                SuccessMessages = Connection.Query<SuccessMessage>(query, p, commandType: CommandType.StoredProcedure);
                if (SuccessMessages.AsList()[0].successid == 3)
                {
                    return "3";
                }
                else
                {
                    strOutput = SuccessMessages.AsList()[0].successmessage;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message, Model);
                throw ex;
            }
            return strOutput;
        }
        public async Task<IEnumerable<Domain.Department.Department>> DepartmentGateById(int id)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Id", OracleDbType.Int32, ParameterDirection.Input, id);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_GETDEPARTMENTBYID";
                var result = await Connection.QueryAsync<Domain.Department.Department>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.Department.Department>> ViewLevels()
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_LEVELVIEW";
                var result = await Connection.QueryAsync<Domain.Department.Department>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
