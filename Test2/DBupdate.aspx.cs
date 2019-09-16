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
                GridView1.Style.Add("display", "inline");
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

        //protected bool isValidated(GridViewUpdateEventArgs e)
        //{
        //    Dictionary<string, Dictionary<string, string>> data = db.getTableMetadata(this.selectedTable);
        //    IEnumerator iterator = e.NewValues.Values.GetEnumerator();
        //    IEnumerator values = data.Values;
        //    while (iterator.MoveNext() && iterator2.MoveNext())
        //    {
        //        if (db.validateInput(iterator.Current.ToString(), iterator2.Current))
        //        {

        //        }
        //    }
        //}

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {

            if(e.NewValues.Count > 0)
            {
                string pkValue = GridView1.DataKeys[e.RowIndex].Value.ToString();
                string pkName = GridView1.DataKeyNames.GetValue(0).ToString();

                IEnumerator iterator = e.NewValues.Keys.GetEnumerator();
                List<string> keys = new List<string>();
                List<string> newValues = new List<string>();

                while (iterator.MoveNext())
                {
                    keys.Add(iterator.Current.ToString());
                    try
                    {
                        if (e.NewValues[iterator.Current.ToString()].ToString() != null && e.NewValues[iterator.Current.ToString()].ToString() != string.Empty)
                            newValues.Add(e.NewValues[iterator.Current.ToString()].ToString());
                    } catch(Exception err)
                    {
                        Response.Write("<br/>" + err.Message + "<br/>");
                        newValues.Add(null);
                    }
                    
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
                    Response.Write(err.Message);
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
                Response.Write(err);
            }

        }

    }
}