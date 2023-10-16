using ABP.Domain;
using ABP.Domain.Sector;
using ABP.Repository.Contract.Contract.Panel;
using ABP.Repository.Contract.Factory;
using Dapper;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Repository.Panel
{
    public class PanelRepository : RepositoryBase, IPanelRepository
    {
        public IConfiguration Configuration { get; }
        public PanelRepository(IConnectionFactory connectionFactory, IConfiguration configuration) : base(connectionFactory)
        {
            Configuration = configuration;
        }
        public string InsertPanel(ABP.Domain.Panel.Panel panel)
        {
            string strOutput = "";
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var dyParam = new OracleDynamicParameters();
                if (panel.PANELID != 0)
                {
                    dyParam.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "UPDATE");
                }
                else
                {
                    dyParam.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "INSERT");

                }
                dyParam.Add("P_PANELID", OracleDbType.Int32, ParameterDirection.Input, panel.PANELID);
                //dyParam.Add("P_DEPTID", OracleDbType.Int32, ParameterDirection.Input, panel.DEPTID);
                dyParam.Add("P_PANELNAME", OracleDbType.Varchar2, ParameterDirection.Input, panel.PANELNAME);
                dyParam.Add("P_DISPLAYNAME", OracleDbType.Varchar2, ParameterDirection.Input, panel.DISPLAYNAME);
                //dyParam.Add("P_EFFECTIVADATE", OracleDbType.Varchar2, ParameterDirection.Input, panel.EFFECTIVADATE);
                dyParam.Add("P_USERID", OracleDbType.Int32, ParameterDirection.Input, panel.CREATEDBY);
                dyParam.Add("P_IMAGE", OracleDbType.Varchar2, ParameterDirection.Input, panel.IMAGE);
                dyParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_MANAGE_PANEL";
                SuccessMessages = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
                strOutput = SuccessMessages.AsList()[0].successid.ToString();
                return strOutput;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<ABP.Domain.Panel.Panel>> ViewPanel()
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "VIEW");
                dyParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_MANAGE_PANEL";
                var result = await Connection.QueryAsync<Domain.Panel.Panel>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


            public async Task<IEnumerable<ABP.Domain.Panel.Panel>> GetIndicatorName(int sectorid, int indicatorid)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "GIN");
                dyParam.Add("P_sectorid", OracleDbType.Int32, ParameterDirection.Input, sectorid);
                dyParam.Add("P_indicatorid", OracleDbType.Int32, ParameterDirection.Input, indicatorid);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_GETINDICATORBYSECID";
                var result = await Connection.QueryAsync<Domain.Panel.Panel>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetIndicatorTrend(int Distid, int Blockid, int sectorid, int indicatorid, int Year)
        {
            try
            {
                Domain.Panel.Panel p = new Domain.Panel.Panel();
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Distid", OracleDbType.Int32, ParameterDirection.Input, Distid);
                dyParam.Add("P_Blockid", OracleDbType.Int32, ParameterDirection.Input, Blockid);
                dyParam.Add("P_sectorid", OracleDbType.Int32, ParameterDirection.Input, sectorid);
                dyParam.Add("P_indicatorid", OracleDbType.Int32, ParameterDirection.Input, indicatorid);
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, Year);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_GETINDICATORTRENDGRAPH";
                p = Connection.QueryFirstOrDefault<Domain.Panel.Panel>(query, dyParam, commandType: CommandType.StoredProcedure);
                return p.IndicatorTrend.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetIndicatorTrendv2(int Distid, int indicatorid, int sectorid, int Blockid, int Year)
        {
            try
            {
                Domain.Panel.Panel p = new Domain.Panel.Panel();
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Distid", OracleDbType.Int32, ParameterDirection.Input, Distid);
                dyParam.Add("P_Blockid", OracleDbType.Int32, ParameterDirection.Input, Blockid);
                dyParam.Add("P_sectorid", OracleDbType.Int32, ParameterDirection.Input, sectorid);
                dyParam.Add("P_indicatorid", OracleDbType.Int32, ParameterDirection.Input, indicatorid);
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, Year);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_GETINDICATORTRENDGRAPH";
                p = Connection.QueryFirstOrDefault<Domain.Panel.Panel>(query, dyParam, commandType: CommandType.StoredProcedure);
                return p.IndicatorTrend.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetDataPointTrend(int Blockid, int sectorid, int indicatorid, int Year, string controlname)
        {
            try
            {
                Domain.Panel.Panel p = new Domain.Panel.Panel();
                var dyParam = new OracleDynamicParameters();
                //dyParam.Add("P_Distid", OracleDbType.Int32, ParameterDirection.Input, Distid);
                dyParam.Add("P_CONTROL_NAME", OracleDbType.Varchar2, ParameterDirection.Input, controlname);
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, Blockid);
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, Convert.ToString(sectorid));
                //dyParam.Add("P_indicatorid", OracleDbType.Int32, ParameterDirection.Input, indicatorid);
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, Year);
                dyParam.Add("P_OUT", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_GET_DATAPOINT";
                p = Connection.QueryFirstOrDefault<Domain.Panel.Panel>(query, dyParam, commandType: CommandType.StoredProcedure);
                return p.GRAPHDATA.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetDataPointTrendv2(string controlname, int sectorid, int indicatorid, int Year,string Blockid)
        {
            try
            {
                Domain.Panel.Panel p = new Domain.Panel.Panel();
                var dyParam = new OracleDynamicParameters();
                //dyParam.Add("P_Distid", OracleDbType.Int32, ParameterDirection.Input, Distid);
                dyParam.Add("P_CONTROL_NAME", OracleDbType.Varchar2, ParameterDirection.Input, controlname);
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(Blockid));
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, Convert.ToString(sectorid));
                //dyParam.Add("P_indicatorid", OracleDbType.Int32, ParameterDirection.Input, indicatorid);
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, Year);
                dyParam.Add("P_OUT", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_GET_DATAPOINT";
                p = Connection.QueryFirstOrDefault<Domain.Panel.Panel>(query, dyParam, commandType: CommandType.StoredProcedure);
                return p.GRAPHDATA.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public string GetDataPointName(int sectorid, string controlname)
        {
            try
            {
                Domain.Panel.Panel p = new Domain.Panel.Panel();
                var dyParam = new OracleDynamicParameters();
                //dyParam.Add("P_Distid", OracleDbType.Int32, ParameterDirection.Input, Distid);
                dyParam.Add("P_CONTROLNAME", OracleDbType.Varchar2, ParameterDirection.Input, controlname);
                //dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, Blockid);
                dyParam.Add("P_PANELID", OracleDbType.Varchar2, ParameterDirection.Input, Convert.ToString(sectorid));
                //dyParam.Add("P_indicatorid", OracleDbType.Int32, ParameterDirection.Input, indicatorid);
                //dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, Year);
                dyParam.Add("P_OUT", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_GETDATAPOINTBYSECID";
                p = Connection.QueryFirstOrDefault<Domain.Panel.Panel>(query, dyParam, commandType: CommandType.StoredProcedure);
                return p.DISPLAYNAME.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetIndictorNameTrend(int sectorid, string indicatorid)
        {
            try
            {
                Domain.Panel.Panel p = new Domain.Panel.Panel();
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "GIN");
                //dyParam.Add("P_Distid", OracleDbType.Int32, ParameterDirection.Input, Distid);
                dyParam.Add("P_indicatorid", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(indicatorid) );
                
                dyParam.Add("P_SectorId", OracleDbType.Int32, ParameterDirection.Input, sectorid);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_GETINDICATORBYSECID";
                p = Connection.QueryFirstOrDefault<Domain.Panel.Panel>(query, dyParam, commandType: CommandType.StoredProcedure);
                return p.INDICATORNAME.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetBlockNameTrend(int Distid, string Blockid)
        {
            try
            {
                Domain.Panel.Panel p = new Domain.Panel.Panel();
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "TBS");
                dyParam.Add("P_Distid", OracleDbType.Int32, ParameterDirection.Input, Distid);
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(Blockid));
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_GETINDICATORBYSECID";
                p = Connection.QueryFirstOrDefault<Domain.Panel.Panel>(query, dyParam, commandType: CommandType.StoredProcedure);
                return p.DISPLAYNAME.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<IEnumerable<ABP.Domain.Panel.Panel>> GetIndicatorBySector(int sectorid)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "GIBS");
                dyParam.Add("P_SectorId", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(sectorid));
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_GETINDICATORBYSECID";
                var result = await Connection.QueryAsync<Domain.Panel.Panel>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<ABP.Domain.Panel.Panel>> GetIndicatorBySectorYearly(int sectorid)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "GISY");
                dyParam.Add("P_SectorId", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(sectorid));
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_GETINDICATORBYSECID";
                var result = await Connection.QueryAsync<Domain.Panel.Panel>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<ABP.Domain.Panel.Panel>> GetIndicatorBySectorMonthly(int sectorid)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "GISM");
                dyParam.Add("P_SectorId", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(sectorid));
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_GETINDICATORBYSECID";
                var result = await Connection.QueryAsync<Domain.Panel.Panel>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<ABP.Domain.Panel.Panel>> GetDatapointBySector(int sectorid)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "GDBS");
                dyParam.Add("P_SectorId", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(sectorid));
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_GETINDICATORBYSECID";
                var result = await Connection.QueryAsync<Domain.Panel.Panel>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string DeletePanel(ABP.Domain.Panel.Panel Model)
        {
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var p = new OracleDynamicParameters();
                p.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "DELETE");
                p.Add("P_PANELID", OracleDbType.Int32, ParameterDirection.Input, Model.PANELID);
                p.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_MANAGE_PANEL";
                SuccessMessages = Connection.Query<SuccessMessage>(query, p, commandType: CommandType.StoredProcedure);
                return SuccessMessages.AsList()[0].successid.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.Panel.Panel>> PanelGateById(int id)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "BYID");
                dyParam.Add("P_PANELID", OracleDbType.Int32, ParameterDirection.Input, id);
                dyParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_MANAGE_PANEL";
                var result = await Connection.QueryAsync<Domain.Panel.Panel>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string GetSeqValue(string SequenceName)
        {
            DataTable dt = new DataTable();
            string query = "select " + SequenceName + ".nextval as nextvalue from dual";
            using (OracleConnection conn = new OracleConnection(Configuration.GetValue<string>("MySettings:Con")))
            {
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    conn.Open();
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt.Rows[0]["nextvalue"].ToString();
        }

        public async Task<IEnumerable<ABP.Domain.Panel.Panel>> ViewSector()
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "SECTOR");
                dyParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_SECTOR_NAME";
                var result = await Connection.QueryAsync<Domain.Panel.Panel>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<ABP.Domain.Panel.Panel>> ViewLevels()
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_LEVELVIEW";
                var result = await Connection.QueryAsync<ABP.Domain.Panel.Panel>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<ABP.Domain.Panel.Panel>> ViewProviders()
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_PROVIDERVIEW";
                var result = await Connection.QueryAsync<ABP.Domain.Panel.Panel>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<ABP.Domain.Indicator.Indicator>> ViewIndicatorFormula()
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "VIEW");
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_INDICATORFORMULA_VIEW";
                var result = await Connection.QueryAsync<ABP.Domain.Indicator.Indicator>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                //Logger.LogError(ex, ex.Message);
                throw ex; 
            }
        }

        public string DeleteIndicatorFormulaId(ABP.Domain.Indicator.Indicator Model)
        {
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var p = new OracleDynamicParameters();
                p.Add("P_Action", OracleDbType. Varchar2, ParameterDirection.Input, "DELETE");
                p.Add("P_FORMULAID", OracleDbType.Int32, ParameterDirection.Input, (Convert.ToInt32(Model.IndicatorFormulaID)));
                p.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_INDICATORFORMULA_VIEW";
                SuccessMessages = Connection.Query<SuccessMessage>(query, p, commandType: CommandType.StoredProcedure);
                return SuccessMessages.AsList()[0].successid.ToString();
            }
            catch (Exception ex)
            {
                //Logger.LogError(ex, ex.Message, Model);
                throw ex;
            }
        }


        public async Task<IEnumerable<ABP.Domain.Indicator.Indicator>> BindFormulaById(int id)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_FORMULAID", OracleDbType.Int32, ParameterDirection.Input, id);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_INDICATORFORULABYID";
                var result = await Connection.QueryAsync<ABP.Domain.Indicator.Indicator>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                //Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
        public string CreateTable(string TableName)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(Configuration.GetValue<string>("MySettings:Con")))
                {
                    var logtablename = TableName + "_LOG";
                    conn.Open();
                    using (OracleCommand cmd = new OracleCommand(" CREATE TABLE " + TableName + " (ID NUMBER,APPLICATIONNO varchar(200),CREATEDON DATE,CREATEDBY NUMBER,UPDATEDON DATE,UPDATEDBY NUMBER,DELETEDFLAG NUMBER)", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    using (OracleCommand cmd = new OracleCommand("ALTER TABLE " + TableName + " ADD ( CONSTRAINT " + TableName + "_pk PRIMARY KEY(ID)) ", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    using (OracleCommand cmd = new OracleCommand(" CREATE SEQUENCE " + TableName + "_SEQ START WITH 1", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    using (OracleCommand cmd = new OracleCommand(" CREATE TABLE " + logtablename + "(ID NUMBER,APPLICATIONNO varchar(200),CREATEDON DATE,CREATEDBY NUMBER,UPDATEDON DATE,UPDATEDBY NUMBER,DELETEDFLAG NUMBER)", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    using (OracleCommand cmd = new OracleCommand("ALTER TABLE " + logtablename + " ADD ( CONSTRAINT " + logtablename + "_pk PRIMARY KEY(ID)) ", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    using (OracleCommand cmd = new OracleCommand(" CREATE SEQUENCE " + logtablename + "_SEQ START WITH 1", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                return "1";
            }
            catch(Exception ex) 
            {
                throw ex;
                //return "2";
            }
        }
        public async Task<IEnumerable<ABP.Domain.Panel.Panel>> ViewPanel1()
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "VIEW1");
                dyParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_MANAGE_PANEL";
                var result = await Connection.QueryAsync<Domain.Panel.Panel>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
