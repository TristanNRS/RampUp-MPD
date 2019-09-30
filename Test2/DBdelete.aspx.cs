using Authentication;
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
        private bool isAuthorized;
        private int rowToDelete = -1;

        protected void Page_Load(object sender, EventArgs e)
        {
            Auth auth = new Auth();
            List<string> authorizedRoles = new List<string>() { "ADMIN" };
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
            }
            else
            {
                if (this.isAuthorized)
                {
                    this.selectedTable = (string)ViewState["selectedTable"];
                    if (this.selectedTable != null)
                    {
                        string sql = (string)ViewState["isSearch"];
                        this.bindTable(sql);
                    }

                    this.rowToDelete = ViewState["rowToDelete"] == null ? -1 : (int)ViewState["rowToDelete"];

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

                searchPanel.Style.Add("display", "inline");
                searchBox.Text = string.Empty;

                GridView1.Style.Add("display", "inline");
                GridView1.PageIndex = 0;
                this.bindTable();
            }
            else
            {
                searchPanel.Style.Add("display", "none");

                GridView1.Style.Add("display", "none");
                this.selectedTable = null;
            }
            ViewState["selectedTable"] = this.selectedTable;
        }

        protected void bindTable(string sql = null)
        {
            if (this.selectedTable == null)
                this.selectedTable = ViewState["selectedTable"].ToString();

            if(this.selectedTable != null)
            {
                GridView1.Columns.Clear();
                try
                {
                    SqlConnection conn = db.getConnection();

                    conn.Open();

                    List<string> colNames = db.getAllColumnNames(this.selectedTable, conn);
                    List<string> primaryKeys = db.getPrimaryKeys(this.selectedTable);

                    // set name of primary key
                    if (primaryKeys != null)
                        GridView1.DataKeyNames = primaryKeys.ToArray();

                    if (sql == null)
                        sql = $"SELECT * FROM [dbo].[{this.selectedTable}]";

                    SqlCommand cmd = db.getCommand(sql, conn);

                    SqlDataAdapter ad = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable();
                    ad.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        List<string> headerNames = db.getFormattedColNames(colNames);
                        BoundField Field;
                        DataControlField Col;
                        for (int i = 0; i < colNames.Count; i += 1)
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

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            ViewState["rowToDelete"] = e.RowIndex;
            ModalExtender.Show();

            string sql = (string)ViewState["isSearch"];
            this.bindTable(sql);
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            searchBox.Text = string.Empty;
            this.bindTable();
        }

        protected void searchBox_TextChanged(object sender, EventArgs e)
        {
            if (this.selectedTable == null)
                this.selectedTable = ViewState["selectedTable"].ToString();

            if(this.selectedTable != null)
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
            } else
            {
                statusPanel.Style.Add("display", "inline");
                HtmlGenericControl h3 = new HtmlGenericControl("h3");
                h3.InnerText = "Selected table Error";
                statusPanel.Controls.Add(h3);
                statusPanel.Controls.Add(new LiteralControl("Cannot find selected table"));
            }
        }

        protected void cancelButton_Click(object sender, EventArgs e)
        {
            ModalExtender.Hide();
            ViewState["rowToDelete"] = null;
            string sql = (string)ViewState["isSearch"];
            this.bindTable(sql);
        }

        protected void deleteButton_Click(object sender, EventArgs e)
        {
            if(this.rowToDelete != -1)
            {
                // delete record
                int numPrimaryKeys = GridView1.DataKeyNames.Length;

                Dictionary<string, string> primaryKeys = new Dictionary<string, string>();

                for (int i = 0; i < numPrimaryKeys; i += 1)
                {
                    primaryKeys.Add(GridView1.DataKeyNames.GetValue(i).ToString(), GridView1.DataKeys[this.rowToDelete].Values[i].ToString());
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

            ModalExtender.Hide();
            ViewState["rowToDelete"] = null;
        }

        protected void searchIcon_Click(object sender, ImageClickEventArgs e)
        {
            searchBox_TextChanged(sender, e);
        }
    }
}