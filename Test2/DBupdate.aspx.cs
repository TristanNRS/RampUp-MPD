using DbAccess;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Test2
{
    public partial class DBupdate : System.Web.UI.Page
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
            }
            else
            {
                this.selectedTable = (string)ViewState["selectedTable"];
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
            if (!tableList.SelectedItem.Value.ToString().Equals("----"))
            {
                this.selectedTable = tableList.SelectedItem.Value.ToString();

                statusPanel.Style.Add("display", "none");
                statusPanel.Controls.Clear();

                GridView1.Style.Add("display", "inline");
                GridView1.PageIndex = 0;
                this.bindTable();
            }
            else
            {
                GridView1.Style.Add("display", "none");
                this.selectedTable = null;
            }
            ViewState["selectedTable"] = this.selectedTable;
        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            this.bindTable();
        }

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;
            this.bindTable();
        }

        protected bool isValidated(GridViewUpdateEventArgs e)
        {
            Dictionary<string, Dictionary<string, string>> data = db.getTableMetadata(this.selectedTable);
            IEnumerator keyIterator = e.NewValues.Keys.GetEnumerator();
            IEnumerator valIterator = e.NewValues.Values.GetEnumerator();
            while (keyIterator.MoveNext() && valIterator.MoveNext())
            {
                string key = keyIterator.Current.ToString();

                var nextVal = valIterator.Current;
                string valToValidate= nextVal != null ? nextVal.ToString() : string.Empty;

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

            if(e.NewValues.Count > 0 && isValidated(e))
            {
                string pkValue = GridView1.DataKeys[e.RowIndex].Value.ToString();
                string pkName = GridView1.DataKeyNames.GetValue(0).ToString();

                IEnumerator iterator = e.NewValues.Keys.GetEnumerator();
                IEnumerator iterator2 = e.NewValues.Values.GetEnumerator();
                List<string> keys = new List<string>();
                List<string> newValues = new List<string>();
                
                while (iterator.MoveNext() && iterator2.MoveNext())
                {
                    keys.Add(iterator.Current.ToString());
                    var nextVal = iterator2.Current;
                    string valToAdd = nextVal != null ? nextVal.ToString() : string.Empty;
                    newValues.Add(valToAdd);
                }
            
                string sql = db.getSqlUpdate(keys, newValues, pkName, pkValue, this.selectedTable);
                try
                {
                    SqlConnection conn = db.getConnection();
                    conn.Open();
                    SqlCommand command = db.getCommand(sql, conn);
                    command.ExecuteNonQuery();
                    GridView1.EditIndex = -1;
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
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            bindTable();
        }

        private void bindTable()
        {
            GridView1.Columns.Clear();
            try
            {
                SqlConnection conn = db.getConnection();

                conn.Open();

                List<string> colNames = db.getAllColumnNames(this.selectedTable, conn);

                string sql = $"SELECT * FROM [dbo].[{this.selectedTable}]";

                SqlCommand cmd = db.getCommand(sql, conn);

                SqlDataAdapter ad = new SqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                ad.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    BoundField Field;
                    DataControlField Col;
                    int count = 0;
                    colNames.ForEach((colName) =>
                    {
                        if(count == 0)
                        {
                            // set name of primary key
                            GridView1.DataKeyNames = new string[1] { colName };
                        }
                        Field = new BoundField();

                        Field.DataField = Field.HeaderText = colName;
                        Field.Visible = count != 0;

                        Col = Field;
                        GridView1.Columns.Add(Col);

                        count += 1;
                    });
                    GridView1.DataSource = dt;

                    GridView1.DataBind();
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

    }
}