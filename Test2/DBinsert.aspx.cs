using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using DbAccess;
using System.Data.SqlClient;
using System.Data;
using System.Web.UI;
using System.Web.UI.HtmlControls;

//TODO: Why is data from last form showing up when switching tables. Occurs when txt box has same name/id


namespace Test2
{
    public partial class DBinsert : System.Web.UI.Page
    {
        private Db db = new Db();
        private string selectedTable;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Panel1.Style.Add("display", "none");
                addDataButton.Style.Add("display", "none");
                tableList.Items.Add(new ListItem("----", "----"));
                this.loadTablesInDropdown();
            } else
            {
                this.selectedTable = (string)ViewState["selectedTable"];
                if(selectedTable != null)
                {
                    this.createInsertForm();
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
            if (!tableList.SelectedItem.Value.ToString().Equals("----"))
            {
                this.selectedTable = tableList.SelectedItem.Value.ToString();
                Panel1.Style.Add("display", "none");
                addDataButton.Style.Add("display", "inline");
                GridView1.Style.Add("display", "inline");
                this.bindTable();
            }
            else
            {
                addDataButton.Style.Add("display", "none");
                GridView1.Style.Add("display", "none");
                this.selectedTable = string.Empty;
            }
            ViewState["selectedTable"] = this.selectedTable;
        }


        private void bindTable()
        {
            GridView1.Columns.Clear();
            try
            {
                SqlConnection conn = db.getConnection();

                conn.Open();

                List<string> colNames = db.getColumnNames(this.selectedTable, conn);

                string sql = db.getSqlSelect(colNames, this.selectedTable);

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
                }

                conn.Close();
            }
            catch (Exception err)
            {
                Response.Write(err);
            }

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

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {

        }


        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Response.Write("deleted<br/>");
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            bindTable();
        }

        protected void createInsertForm()
        {
            formTable.Controls.Clear();
            // add header
            TableHeaderRow headerRow = new TableHeaderRow();
            TableHeaderCell headerCell = new TableHeaderCell();
            HtmlGenericControl h3 = new HtmlGenericControl();
            h3.InnerHtml = "Data Entry";
            headerCell.Controls.Add(h3);

            headerRow.Cells.Add(headerCell);
            formTable.Rows.Add(headerRow);

            // add body
            Dictionary<string, Dictionary<string, string>> data = db.getTableMetadata(this.selectedTable);
            List<string> colNames = db.getColumnNames(null, null, data);
            colNames.ForEach((colName) =>
            {
                TableRow tr = new TableRow();

                TableCell labelCell = new TableCell();
                // create label 
                Label label = new Label();
                label.ID = $"lbl_{this.selectedTable}_{colName}";
                label.Text = $"Enter {colName}: ";
                // add label to label cell
                labelCell.Controls.Add(label);

                // create textbox
                TableCell inputCell = new TableCell();
                TextBox txt = new TextBox();
                txt.ID = $"txt_{this.selectedTable}_{colName}";
                txt.Style.Add("width", "250px");
                if (colName.Equals("description"))
                    txt.TextMode = TextBoxMode.MultiLine;
                txt.Text = string.Empty;

                // add textbox to input cell
                inputCell.Controls.Add(txt);

                // add label and textbox to row
                tr.Cells.Add(labelCell);
                tr.Cells.Add(inputCell);

                // create validator
                Dictionary<string, string> props = data[colName];


                // add required validator for fields that cannot be null
                TableCell reqFieldValidateCell = new TableCell();
                string isRequired = props["isRequired"];
                if (isRequired.Equals("Y"))
                {
                    RequiredFieldValidator validator = new RequiredFieldValidator();
                    validator.ID = $"validate_{this.selectedTable}_{colName}";
                    validator.ControlToValidate = txt.ID;
                    validator.Display = ValidatorDisplay.Dynamic;
                    validator.ErrorMessage = "Required Field";
                    validator.ForeColor = System.Drawing.Color.Red;

                    // add validator to input cell
                    reqFieldValidateCell.Controls.Add(validator);

                    // add validator to row
                    tr.Cells.Add(reqFieldValidateCell);
                }

                // add type validator
                TableCell typeValidateCell = new TableCell();
                string type = db.mapDbTypeToInputType(props["dataType"], colName);
                RegularExpressionValidator typeValidator = new RegularExpressionValidator();
                typeValidator.ID = $"validateType_{this.selectedTable}_{colName}";
                typeValidator.ControlToValidate = txt.ID;
                typeValidator.ErrorMessage = $"Wrong type: expected {type}";
                typeValidator.ForeColor = System.Drawing.Color.Red;

                switch (type){
                    case "int":
                        typeValidator.ValidationExpression = "^\\d+$";
                        break;
                    case "float":
                        typeValidator.ValidationExpression = "[+-]?([0-9]+([.][0-9]*)?|[.][0-9]+)";
                        break;
                    case "email":
                        typeValidator.ValidationExpression = "^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[A-Z0-9.-]+\\.[A-Z]{2,}$";
                        break;
                    case "phoneNumber":
                        typeValidator.ValidationExpression = "^\\d{3}-\\d{4}$";
                        break;
                    case "date":
                        typeValidator.ValidationExpression = "((0?[13578]|10|12)(-|\\/)((0[0-9])|([12])([0-9]?)|(3[01]?))(-|\\/)((\\d{4})|(\\d{2}))|(0?[2469]|11)(-|\\/)((0[0-9])|([12])([0-9]?)|(3[0]?))(-|\\/)((\\d{4}|\\d{2})))";
                        break;
                    case "time":
                        typeValidator.ValidationExpression = ".+";
                        break;
                    case "string":
                        typeValidator.ValidationExpression = ".+";
                        break;
                }

                // add validator to input cell
                typeValidateCell.Controls.Add(typeValidator);

                // add validator to row
                tr.Cells.Add(typeValidateCell);

                tr.Attributes.Add("TableSection", "TableBody");

                // Add row to the table.
                formTable.Rows.Add(tr);
            });

            // add footer
            TableFooterRow footerRow = new TableFooterRow();
            TableCell footerCell = new TableCell();
            Button submitButton = new Button();
            submitButton.Text = "Submit";
            submitButton.Click += SubmitButton_Click;
            footerCell.Controls.Add(submitButton);
            footerRow.Cells.Add(footerCell);
            formTable.Rows.Add(footerRow);
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            // submit form data to DB
            try
            {
                SqlConnection conn = db.getConnection();
                conn.Open();
                List<string> values = this.getInsertValues(formTable);
                List<string> cols = db.getColumnNames(this.selectedTable, conn);
                string sql = db.getSqlInsert(cols, values, this.selectedTable);
                SqlCommand command = db.getCommand(sql, conn);
                command.ExecuteNonQuery();
                db.closeOff(conn, command);
                this.bindTable();
            }
            catch (Exception err)
            {
                Response.Write(err.Message + "<br/>");
            }

            Panel1.Style.Add("display", "none");
            Panel2.Controls.Add(new LiteralControl("Data Submitted!"));
        }

        List<string> getInsertValues(Control parent)
        {
            List<string> values = new List<string>();
            foreach (Control child in parent.Controls)
            {
                if (child.GetType().ToString().Equals("System.Web.UI.WebControls.TextBox"))
                {
                    TextBox txt = (TextBox)child;
                    values.Add(txt.Text);
                }

                if (child.Controls.Count > 0)
                {
                    values.AddRange(getInsertValues(child));
                }
            }

            return values;

        }


        protected void addDataButton_Click(object sender, EventArgs e)
        {
            Panel1.Style.Add("display", "inline");
        }
    }
}