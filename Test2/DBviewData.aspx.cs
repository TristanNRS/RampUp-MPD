using System;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using Authentication;
using DbAccess;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace Test2
{
    public partial class DBviewData : System.Web.UI.Page
    {
        private Db db = new Db();
        private string selectedTable;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                statusPanel.Style.Add("display", "none");
                tableList.Items.Add(new ListItem("----", "----"));
                this.loadTablesInDropdown();
            } else
            {
                this.selectedTable = (string)ViewState["this.selectedTable"];
                if (this.selectedTable != null)
                {
                    this.bindTable();
                }
            }
        }

        protected void loadTablesInDropdown()
        { 
            List<string> tableNames = db.getListOfTables();
            if (tableNames != null)
            {
                tableNames.ForEach((tableName) =>
                {
                    tableList.Items.Add(new ListItem(tableName, tableName));
                });
            }
        }

        protected void tableList_SelectedIndexChanged(object sender, EventArgs e)
        {
            // get all data from selected table
            this.selectedTable = tableList.SelectedItem.Value.ToString();
            if (!this.selectedTable.Equals("----"))
            {
                statusPanel.Style.Add("display", "none");
                statusPanel.Controls.Clear();

                GridView1.Style.Add("display", "inline");
                GridView1.PageIndex = 0;
                this.bindTable();
            } else
            {
                GridView1.Style.Add("display", "none");
            }

            ViewState["this.selectedTable"] = this.selectedTable;
        }

        private void bindTable()
        {
            GridView1.Columns.Clear();
            try
            {
                SqlConnection conn = db.getConnection();

                conn.Open();

                string sql = $"SELECT * FROM {this.selectedTable}";

                SqlCommand cmd = db.getCommand(sql, conn);

                SqlDataAdapter ad = new SqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                ad.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    List<string> colNames = db.getColumnNamesWithoutPk(this.selectedTable, conn);
                    BoundField Field;
                    DataControlField Col;
                    colNames.ForEach((colName) =>
                    {
                        Field = new BoundField();

                        Field.DataField = Field.HeaderText = colName;

                        Col = Field;

                        GridView1.Columns.Add(Col);
                    });
                    GridView1.DataSource = dt;

                    GridView1.DataBind();
                }

                conn.Close();
            } catch (Exception err)
            {
                statusPanel.Style.Add("display", "inline");
                HtmlGenericControl h3 = new HtmlGenericControl("h3");
                h3.InnerText = "Error";
                statusPanel.Controls.Add(h3);
                statusPanel.Controls.Add(new LiteralControl(err.Message));
            }

        }

        public void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            bindTable();
        }


    }
}