using DbAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Test2
{
    public partial class DBdelete : System.Web.UI.Page
    {
        private Db db = new Db();
        private string selectedTable;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                statusPanel.Style.Add("display", "none");

                searchBox.Style.Add("display", "none");

                tableList.Items.Add(new ListItem("----", "----"));
                this.loadTablesInDropdown();
            }
            else
            {
                this.selectedTable = (string)ViewState["selectedTable"];
                if (this.selectedTable != null)
                {
                    string sql = (string)ViewState["isSearch"];
                    this.bindTable(sql);
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
            ViewState["isSearch"] = null;
            // get all data from selected table
            if (!tableList.SelectedItem.Value.ToString().Equals("----"))
            {
                this.selectedTable = tableList.SelectedItem.Value.ToString();

                statusPanel.Style.Add("display", "none");
                statusPanel.Controls.Clear();

                searchBox.Style.Add("display", "inline");
                searchBox.Text = string.Empty;

                GridView1.Style.Add("display", "inline");
                GridView1.PageIndex = 0;
                this.bindTable();
            }
            else
            {
                searchBox.Style.Add("display", "none");

                GridView1.Style.Add("display", "none");
                this.selectedTable = null;
            }
            ViewState["selectedTable"] = this.selectedTable;
        }

        private void bindTable(string sql = null)
        {
            GridView1.Columns.Clear();
            try
            {
                SqlConnection conn = db.getConnection();

                conn.Open();

                List<string> colNames = db.getAllColumnNames(this.selectedTable, conn);
                List<string> primaryKeys = db.getPrimaryKeys(this.selectedTable);
                List<string> foreignKeys = db.getForeignKeys(this.selectedTable);

                // set name of primary key
                if (primaryKeys != null)
                    GridView1.DataKeyNames = primaryKeys.ToArray();

                if(sql == null)
                    sql = $"SELECT * FROM [dbo].[{this.selectedTable}]";

                SqlCommand cmd = db.getCommand(sql, conn);

                SqlDataAdapter ad = new SqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                ad.Fill(dt);

                if (dt.Rows.Count > 0)
                {
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
                } else
                {
                    GridView1.DataSource = null;
                }

                conn.Close();
            }
            catch (Exception err)
            {
                statusPanel.Style.Add("display", "inline");
                HtmlGenericControl h3 = new HtmlGenericControl("h3");
                h3.InnerText = "Error";
                statusPanel.Controls.Add(h3);
                statusPanel.Controls.Add(new LiteralControl(err.Message));
            }

        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            // delete record
            int numPrimaryKeys = GridView1.DataKeyNames.Length;

            Dictionary<string, string> primaryKeys = new Dictionary<string, string>();

            for (int i = 0; i < numPrimaryKeys; i += 1)
            {
                primaryKeys.Add(GridView1.DataKeyNames.GetValue(i).ToString(), GridView1.DataKeys[e.RowIndex].Values[i].ToString());
            }

            string sql = db.getSqlDelete(primaryKeys, this.selectedTable);
            try
            {
                SqlConnection conn = db.getConnection();
                conn.Open();
                SqlCommand command = db.getCommand(sql, conn);
                command.ExecuteNonQuery();
                this.bindTable();
            }
            catch (Exception err)
            {
                statusPanel.Style.Add("display", "inline");
                HtmlGenericControl h3 = new HtmlGenericControl("h3");
                h3.InnerText = "DB Error";
                statusPanel.Controls.Add(h3);
                statusPanel.Controls.Add(new LiteralControl(err.Message));
            }
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            string sql = (string)ViewState["isSearch"];
            this.bindTable(sql);
        }

        protected void searchBox_TextChanged(object sender, EventArgs e)
        {
            string searchText = searchBox.Text;

            if (!searchText.Length.Equals(string.Empty))
            {
                List<string> cols = db.getEditableInsertableColumnNames(this.selectedTable);
                string sql = db.getSqlSearch(searchText, cols, this.selectedTable);
                this.bindTable(sql);

                ViewState["isSearch"] = sql;

                if (GridView1.DataSource == null)
                {
                    ViewState["isSearch"] = null;
                    statusPanel.Style.Add("display", "inline");
                    HtmlGenericControl h3 = new HtmlGenericControl("h3");
                    h3.InnerText = "Search Status";
                    statusPanel.Controls.Add(h3);
                    statusPanel.Controls.Add(new LiteralControl($"No records to display for {searchText}"));
                }
            }
            else
            {
                ViewState["isSearch"] = null;
                this.bindTable();
            }
        }
    }
}