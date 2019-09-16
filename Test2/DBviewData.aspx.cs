using System;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using Authentication;
using DbAccess;
using System.Collections.Generic;
using System.Collections;
using System.Data;

namespace Test2
{
    public partial class DBviewData : System.Web.UI.Page
    {
        private Db db = new Db();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                tableList.Items.Add(new ListItem("----", "----"));
                this.loadTablesInDropdown();
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
            string selectedTable = tableList.SelectedItem.Value.ToString();
            if (!selectedTable.Equals("----"))
            {
                GridView1.Style.Add("display", "inline");
                this.bindTable(selectedTable);
            } else
            {
                GridView1.Style.Add("display", "none");
            }
        }

        private void bindTable(string tableName)
        {
            GridView1.Columns.Clear();
            try
            {
                SqlConnection conn = db.getConnection();

                conn.Open();

                string sql = $"SELECT * FROM {tableName}";

                SqlCommand cmd = db.getCommand(sql, conn);

                SqlDataAdapter ad = new SqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                ad.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    List<string> colNames = db.getColumnNamesWithoutPk(tableName, conn);
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
                Response.Write(err);
            }

        }

        public void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            bindTable(tableList.SelectedItem.Value.ToString());
        }


    }
}