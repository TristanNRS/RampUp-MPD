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
        private bool isAuthorized;

        protected void Page_Load(object sender, EventArgs e)
        {
            Auth auth = new Auth();
            List<string> authorizedRoles = new List<string>() { "ADMIN", "USER" };
            this.isAuthorized = auth.isAuthorized(Session["Role"].ToString(), authorizedRoles);

            if (!IsPostBack)
            {

                if (this.isAuthorized)
                {
                    authorizationPanel.Style.Add("display", "inline");

                    statusPanel.Style.Add("display", "none");
                    searchPanel.Style.Add("display", "none");
                    tableList.Items.Add(new ListItem("----", "----"));
                    this.loadTablesInDropdown();
                } else
                {
                    authorizationPanel.Style.Add("display", "none");

                    statusPanel.Style.Add("display", "inline");
                    HtmlGenericControl h3 = new HtmlGenericControl("h3");
                    h3.InnerText = "Unauthorized Access";
                    statusPanel.Controls.Add(h3);
                    statusPanel.Controls.Add(new LiteralControl("Current user does not have authorization to access this page"));
                }
            } else
            {
                if (this.isAuthorized)
                {
                    this.selectedTable = (string)ViewState["selectedTable"];
                    if (this.selectedTable != null)
                    {
                        this.bindTable();
                    }
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

                searchPanel.Style.Add("display", "inline");
                searchBox.Text = string.Empty;

                GridView1.Style.Add("display", "inline");
                GridView1.PageIndex = 0;
                this.bindTable();
            } else
            {
                GridView1.Style.Add("display", "none");

                searchPanel.Style.Add("display", "none");
            }

            ViewState["selectedTable"] = this.selectedTable;
        }

        private void bindTable(string sql = null)
        {
            if(this.selectedTable == null)
                this.selectedTable = ViewState["selectedTable"].ToString();

            if(this.selectedTable != null)
            {
                GridView1.Columns.Clear();
                try
                {
                    SqlConnection conn = db.getConnection();

                    conn.Open();
                    if (sql == null)
                        sql = $"SELECT * FROM {this.selectedTable}";

                    SqlCommand cmd = db.getCommand(sql, conn);

                    SqlDataAdapter ad = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable();
                    ad.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        List<string> colNames = db.getAllColumnNames(this.selectedTable, conn);
                        List<string> headerNames = db.getFormattedColNames(colNames);
                        BoundField Field;
                        DataControlField Col;
                        for(int i = 0; i < colNames.Count; i += 1)
                        {
                            Field = new BoundField();

                            Field.DataField = colNames[i];
                            Field.HeaderText = headerNames[i];

                            Col = Field;

                            GridView1.Columns.Add(Col);
                        }
                       
                        GridView1.DataSource = dt;

                        GridView1.DataBind();
                    }
                    else
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
            } else
            {
                statusPanel.Style.Add("display", "inline");
                HtmlGenericControl h3 = new HtmlGenericControl("h3");
                h3.InnerText = "Selected table Error";
                statusPanel.Controls.Add(h3);
                statusPanel.Controls.Add(new LiteralControl("Cannot find selected table"));
            }
        }

        public void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            searchBox.Text = string.Empty;
            this.bindTable();
        }

        protected void searchBox_TextChanged(object sender, EventArgs e)
        {
            if (this.selectedTable == null)
                this.selectedTable = ViewState["selectedTable"].ToString();

            if (this.selectedTable != null)
            {
                string searchText = searchBox.Text;

                if (!searchText.Length.Equals(string.Empty))
                {
                    List<string> cols = db.getEditableInsertableColumnNames(this.selectedTable);
                    string sql = db.getSqlSearch(searchText, cols, this.selectedTable);
                    this.bindTable(sql);

                    if (GridView1.DataSource == null)
                    {
                        statusPanel.Style.Add("display", "inline");
                        HtmlGenericControl h3 = new HtmlGenericControl("h3");
                        h3.InnerText = "Search Status";
                        statusPanel.Controls.Add(h3);
                        statusPanel.Controls.Add(new LiteralControl($"No records to display for {searchText}"));
                    }
                    else
                    {

                    }
                }
                else
                {
                    this.bindTable();
                }
            } else
            {
                statusPanel.Style.Add("display", "inline");
                HtmlGenericControl h3 = new HtmlGenericControl("h3");
                h3.InnerText = "Selected table Error";
                statusPanel.Controls.Add(h3);
                statusPanel.Controls.Add(new LiteralControl("Cannot find selected table"));
            }
        }

        protected void searchIcon_Click(object sender, ImageClickEventArgs e)
        {
            searchBox_TextChanged(sender, e);
        }
    }
}