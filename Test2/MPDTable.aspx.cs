using Authentication;
using DbAccess;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Test2
{
    public partial class MPDTable : System.Web.UI.Page
    {
        private Db db = new Db();
        private string selectedTable;
        private bool isAuthorized;

        // Used to store the index of the row to delete 
        private int rowToDelete = -1;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Start Authorization check
            Auth auth = new Auth();
            List<string> authorizedRoles = new List<string>() { "ADMIN" };
            this.isAuthorized = auth.isAuthorized(Session["Role"].ToString(), authorizedRoles);
            // End Authorization check

            if (!IsPostBack)
            {
                if (this.isAuthorized)
                {
                    // Initialize display for authorized users

                    authorizationPanel.Style.Add("display", "inline");

                    statusPanel.Style.Add("display", "none");

                    searchPanel.Style.Add("display", "none");

                    tableList.Items.Add(new ListItem("----", "----"));
                    this.loadTablesInDropdown();
                }
                else
                {
                    // Initialize display for unauthorized users

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

            // Reinitialize Status panel by clearing any previously added status or error messages
            statusPanel.Style.Add("display", "none");
            statusPanel.Controls.Clear();

            // get all data from selected table
            if (!tableList.SelectedItem.Value.ToString().Equals("----"))
            {
                this.selectedTable = tableList.SelectedItem.Value.ToString();

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
            /**
            * Shows data from db for selected table not including primary key
            * */

            if (this.selectedTable == null)
                this.selectedTable = ViewState["selectedTable"].ToString();

            if (this.selectedTable != null)
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

                            if (primaryKeys != null && primaryKeys.Contains(colNames[i]))
                            {
                                Field.Visible = false;
                                if (foreignKeys != null && foreignKeys.Contains(colNames[i]))
                                    Field.Visible = true;

                            }
                            else
                                Field.Visible = true;


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

            }
            else
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

            if (this.selectedTable != null)
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
            else
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

        protected void cancelButton_Click(object sender, EventArgs e)
        {
            ModalExtender.Hide();
            ViewState["rowToDelete"] = null;
            string sql = (string)ViewState["isSearch"];
            this.bindTable(sql);
        }

        protected void deleteButton_Click(object sender, EventArgs e)
        {
            // Delete button in Confirm Delete Modal is clicked
            // Deletes the row specified in this.rowToDelete

            if (this.rowToDelete != -1)
            {
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

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            string sql = (string)ViewState["isSearch"];
            this.bindTable(sql);
        }

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;
            string sql = (string)ViewState["isSearch"];
            this.bindTable(sql);
        }

        protected bool isValidated(GridViewUpdateEventArgs e)
        {
            /**
             * Validates data entered through edit using NewValues which contains the keys and all values which changed
             * 
             * In the case of values that did not change, the NewValues.Values will not contain the unchanged value so a check must be made to ensure 
             * a null value is not passed to the validateInput function
             * */

            Dictionary<string, Dictionary<string, string>> data = db.getTableMetadata(this.selectedTable);
            IEnumerator keyIterator = e.NewValues.Keys.GetEnumerator();
            IEnumerator valIterator = e.NewValues.Values.GetEnumerator();

            //Iterates through every key and corresponding NewValue to ensure every field is validated
            while (keyIterator.MoveNext() && valIterator.MoveNext())
            {
                string key = keyIterator.Current.ToString();

                var nextVal = valIterator.Current;

                // toString called here because if valIterator.Current == null then an exception will be thrown
                string valToValidate = nextVal != null ? nextVal.ToString() : string.Empty;

                string validationResult = db.validateInput(valToValidate, key, data[key]);
                if (!validationResult.Equals("Success"))
                {
                    statusPanel.Style.Add("display", "inline");
                    HtmlGenericControl h3 = new HtmlGenericControl("h3");
                    h3.InnerText = "Validation Error";
                    statusPanel.Controls.Add(h3);
                    statusPanel.Controls.Add(new LiteralControl($"In column '{key}': {validationResult}"));
                    return false;
                }

            }
            return true;
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {

            if (e.NewValues.Count > 0 && isValidated(e))
            {
                int numPrimaryKeys = GridView1.DataKeyNames.Length;

                Dictionary<string, string> primaryKeys = new Dictionary<string, string>();

                for (int i = 0; i < numPrimaryKeys; i += 1)
                {
                    primaryKeys.Add(GridView1.DataKeyNames.GetValue(i).ToString(), GridView1.DataKeys[e.RowIndex].Values[i].ToString());
                }

                IEnumerator iterator = e.NewValues.Keys.GetEnumerator();
                IEnumerator iterator2 = e.NewValues.Values.GetEnumerator();
                List<string> keys = new List<string>();
                List<string> newValues = new List<string>();

                while (iterator.MoveNext() && iterator2.MoveNext())
                {
                    keys.Add(iterator.Current.ToString());
                    var nextVal = iterator2.Current;

                    // toString called here because if valIterator.Current == null then an exception will be thrown
                    string valToAdd = nextVal != null ? nextVal.ToString() : string.Empty;
                    newValues.Add(valToAdd);
                }

                string sql = db.getSqlUpdate(keys, newValues, primaryKeys, this.selectedTable);
                try
                {
                    SqlConnection conn = db.getConnection();
                    conn.Open();
                    SqlCommand command = db.getCommand(sql, conn);
                    command.ExecuteNonQuery();
                    GridView1.EditIndex = -1;
                    string sqlSearch = (string)ViewState["isSearch"];
                    this.bindTable(sqlSearch);
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
        }

        protected void GridView1_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            statusPanel.Style.Add("display", "inline");
            HtmlGenericControl h3 = new HtmlGenericControl("h3");
            h3.InnerText = "Success";
            statusPanel.Controls.Add(h3);
            statusPanel.Controls.Add(new LiteralControl($"Record successfully updated"));
        }
    }
}