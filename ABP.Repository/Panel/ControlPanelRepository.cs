using ABP.Domain;
using ABP.Domain.DataPointExpression;
using ABP.Domain.Panel;
using ABP.Repository.Contract.Contract.Panel;
using ABP.Repository.Contract.Factory;
using Dapper;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Repository.ControlPanel
{
    public class ControlPanelRepository : RepositoryBase, IControlPanelRepository
    {
        public IConfiguration Configuration { get; }
        public ControlPanelRepository(IConnectionFactory connectionFactory, IConfiguration configuration) : base(connectionFactory)
        {
            Configuration = configuration;
        }
        public string InsertControlPanel(ABP.Domain.Panel.ControlPanel ControlPanel)
        {
            string strOutput = "";
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var dyParam = new OracleDynamicParameters();
                if (ControlPanel.CONTROLID != 0)
                {
                    dyParam.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "UPDATE");
                }
                else
                {
                    dyParam.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "INSERT");
                    dyParam.Add("P_CONTROLNAME", OracleDbType.Varchar2, ParameterDirection.Input, ControlPanel.CONTROLNAME);
                }
                dyParam.Add("P_CONTROLID", OracleDbType.Int32, ParameterDirection.Input, ControlPanel.CONTROLID);
                dyParam.Add("P_DEPTID", OracleDbType.Int32, ParameterDirection.Input, ControlPanel.DEPTID);
                dyParam.Add("P_PROVIDERID", OracleDbType.Int32, ParameterDirection.Input, ControlPanel.PROVIDERID);
                dyParam.Add("P_TYPE", OracleDbType.Int32, ParameterDirection.Input, ControlPanel.TYPE);
                dyParam.Add("P_YEARTYPE", OracleDbType.Int32, ParameterDirection.Input, ControlPanel.YEARTYPE);
                dyParam.Add("P_DESCRIPTION", OracleDbType.Varchar2, ParameterDirection.Input, ControlPanel.DESCRIPTION);
                dyParam.Add("P_UNIT", OracleDbType.Varchar2, ParameterDirection.Input, ControlPanel.UNIT);
                dyParam.Add("P_PANELID", OracleDbType.Int32, ParameterDirection.Input, ControlPanel.PANELID);
                dyParam.Add("P_DISPLAYNAME", OracleDbType.Varchar2, ParameterDirection.Input, ControlPanel.DISPLAYNAME1);
                dyParam.Add("P_EFFECTIVADATE", OracleDbType.Varchar2, ParameterDirection.Input, ControlPanel.EFFECTIVEDATE);
                dyParam.Add("P_FREQUENCY", OracleDbType.Int32, ParameterDirection.Input, ControlPanel.FREQUENCYID);
                dyParam.Add("P_MONTHNO", OracleDbType.Int32, ParameterDirection.Input, ControlPanel.MONTHNO);
                dyParam.Add("P_USERID", OracleDbType.Int32, ParameterDirection.Input, ControlPanel.CREATEDBY);
                dyParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_CONTROLPANEL_MANAGE";
                SuccessMessages = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
                strOutput = SuccessMessages.AsList()[0].successid.ToString();
                return strOutput;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<ABP.Domain.Panel.ControlPanel>> ViewControlPanel(int SECTORID,int frequency)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "VIEW");
                dyParam.Add("P_PANELID", OracleDbType.Char, ParameterDirection.Input, SECTORID);
                dyParam.Add("P_FREQUENCY", OracleDbType.Char, ParameterDirection.Input, frequency);
                dyParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_CONTROLPANEL_MANAGE";
                var result = await Connection.QueryAsync<ABP.Domain.Panel.ControlPanel>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string DeleteControlPanel(ABP.Domain.Panel.ControlPanel Model)
        {
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var p = new OracleDynamicParameters();
                p.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "DELETE");
                p.Add("P_CONTROLID", OracleDbType.Int32, ParameterDirection.Input, Model.CONTROLID);
                p.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_CONTROLPANEL_MANAGE";
                SuccessMessages = Connection.Query<SuccessMessage>(query, p, commandType: CommandType.StoredProcedure);
                return SuccessMessages.AsList()[0].successid.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.Panel.ControlPanel>> ControlPanelGateById(int id)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "BYID");
                dyParam.Add("P_CONTROLID", OracleDbType.Int32, ParameterDirection.Input, id);
                dyParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_CONTROLPANEL_MANAGE";
                var result = await Connection.QueryAsync<Domain.Panel.ControlPanel>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.Panel.ControlPanel>> ControlPanelGateByPanelId(int Panelid,int fid)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_PanelId", OracleDbType.Int32, ParameterDirection.Input, Panelid);
                dyParam.Add("P_Frequencyid", OracleDbType.Int32, ParameterDirection.Input, fid);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_CONTROLLERBYPANELID";
                var result = await Connection.QueryAsync<Domain.Panel.ControlPanel>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string AlterTable(string TableName,string ControlName) 
        {
            var logtablename = TableName + "_LOG";
            try
            {
                using (OracleConnection conn = new OracleConnection(Configuration.GetValue<string>("MySettings:Con")))
                {
                    conn.Open();
                    using (OracleCommand cmd = new OracleCommand(" Alter TABLE " + TableName + " Add (" + ControlName + " Decimal(18,2))", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    using (OracleCommand cmd = new OracleCommand(" Alter TABLE " + logtablename + " Add (" + ControlName + " Decimal(18,2))", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                return "1";
            }
            catch(Exception ex)
            {
                return "2";
            }
        }

        public string InsertControlExpression(DatapointExpression DPEx)
        {
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var dyParam = new OracleDynamicParameters();
                if (DPEx.ExpressionID != 0)
                {
                    dyParam.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "Update");
                }
                else
                {
                    dyParam.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "S"); ;
                }
                dyParam.Add("P_UserId", OracleDbType.Int32, ParameterDirection.Input, DPEx.CREATEDBY);
                dyParam.Add("P_SECTORId", OracleDbType.Int32, ParameterDirection.Input, DPEx.SECTORID);
                dyParam.Add("P_EXPRESSIONID", OracleDbType.Int32, ParameterDirection.Input, DPEx.ExpressionID);
                dyParam.Add("P_EXPRESSION", OracleDbType.NVarchar2, ParameterDirection.Input, DPEx.ExpressionNAME);
                dyParam.Add("P_DataPoints", OracleDbType.NVarchar2, ParameterDirection.Input, DPEx.ExpressionNAMEWithID);
                dyParam.Add("P_SCRIPT", OracleDbType.NVarchar2, ParameterDirection.Input, DPEx.Script);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "U_ABP_DATAPOINTEXPRESSION";
                SuccessMessages = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
                return SuccessMessages.AsList()[0].successid.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string ModifySLNO(int primaryId, int slno)
        {
            string strOutput = "";
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "SNO");
                dyParam.Add("P_CONTROLID", OracleDbType.Int32, ParameterDirection.Input, primaryId);
                dyParam.Add("P_SERIALNO", OracleDbType.Varchar2, ParameterDirection.Input, slno);
                dyParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_CONTROLPANEL_MANAGE";
                SuccessMessages = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
                strOutput = SuccessMessages.AsList()[0].successmessage.ToString();
            }
            catch (Exception ex)
            {
                // Logger.LogError(ex, ex.Message, Model);
                throw ex;
            }
            return strOutput;
        }
        public async Task<IEnumerable<DatapointExpression>> GetAllExpressionData()
        {

            try
            {
                var dyParam = new OracleDynamicParameters();

                dyParam.Add("P_ACTION", OracleDbType.NVarchar2, ParameterDirection.Input, "ViewAll");
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "U_ABP_DATAPOINTEXPRESSION";
                var result = await Connection.QueryAsync<Domain.DataPointExpression.DatapointExpression>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<DatapointExpression>> GetExpressionDataById(int ExpressionId,int SECTORID)
        {

            try
            {
                var dyParam = new OracleDynamicParameters();

                dyParam.Add("P_ACTION", OracleDbType.NVarchar2, ParameterDirection.Input, "View");
                dyParam.Add("P_EXPRESSIONID", OracleDbType.Int32, ParameterDirection.Input, ExpressionId);
                dyParam.Add("P_SECTORId", OracleDbType.Int32, ParameterDirection.Input, SECTORID);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "U_ABP_DATAPOINTEXPRESSION";
                var result = await Connection.QueryAsync<Domain.DataPointExpression.DatapointExpression>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string DeleteExpression(Domain.DataPointExpression.DatapointExpression Model)
        {
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.NVarchar2, ParameterDirection.Input, "Delete");
                dyParam.Add("P_EXPRESSIONID", OracleDbType.Int32, ParameterDirection.Input, Model.ExpressionID);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "U_ABP_DATAPOINTEXPRESSION";
                SuccessMessages = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
                return SuccessMessages.AsList()[0].successid.ToString();
            }
            catch (Exception ex)
            {
               // Logger.LogError(ex, ex.Message, Model);
                throw ex;
            }
        }

        public async Task<IEnumerable<Domain.Panel.ControlPanel>> CommonControlPanelData()
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "CT");
                dyParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_CONTROLPANEL_MANAGE";
                var result = await Connection.QueryAsync<Domain.Panel.ControlPanel>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

}
