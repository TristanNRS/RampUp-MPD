using Authentication;
using DbAccess;

using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace Test2
{
    public partial class DBinsert : System.Web.UI.Page
    {
        private Db db = new Db();
        private string selectedTable;
        private bool isAuthorized;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Start Authorization Check
            Auth auth = new Auth();
            List<string> authorizedRoles = new List<string>() { "ADMIN" };
            this.isAuthorized = auth.isAuthorized(Session["Role"].ToString(), authorizedRoles);
            // End Authorization Check

            if (!IsPostBack)
            {

                if (this.isAuthorized)
                {
                    // Initialize display for authorized users

                    authorizationPanel.Style.Add("display", "inline");

                    formPanel.Style.Add("display", "none");
                    statusPanel.Style.Add("display", "none");
                    addDataButton.Style.Add("display", "none");
                    searchPanel.Style.Add("display", "none");

                    tableList.Items.Add(new ListItem("----", "----"));
                    this.loadTablesInDropdown();
                } else
                {
                    // Initialize display for unauthorized users

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
                    if(selectedTable != null)
                    {
                        this.createInsertForm();
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

            // Reinitialize Insert form by clearing any previously added controls
            formPanel.Style.Add("display", "none");
            formTable.Controls.Clear();

            // Reinitialize Status panel by clearing any previously added status or error messages
            statusPanel.Style.Add("display", "none");
            statusPanel.Controls.Clear();

            // get all data from selected table
            if (!tableList.SelectedItem.Value.ToString().Equals("----"))
            {
                this.selectedTable = tableList.SelectedItem.Value.ToString();

                searchPanel.Style.Add("display", "inline");
                searchBox.Text = string.Empty;

                addDataButton.Style.Add("display", "block");

                GridView1.Style.Add("display", "inline");
                GridView1.PageIndex = 0;
                this.bindTable();
            }
            else
            {

                addDataButton.Style.Add("display", "none");
                searchPanel.Style.Add("display", "none");

                GridView1.Style.Add("display", "none");
                this.selectedTable = string.Empty;
            }
            ViewState["selectedTable"] = this.selectedTable;
        }

        private void bindTable(string sql = null)
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

                    List<string> colNames = db.getEditableInsertableColumnNames(this.selectedTable, conn);

                    if (colNames != null)
                    {
                        if (sql == null)
                            sql = db.getSqlSelect(colNames, this.selectedTable);

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
                    else
                    {
                        GridView1.Style.Add("display", "none");
                        addDataButton.Style.Add("display", "none");
                        statusPanel.Style.Add("display", "inline");
                        HtmlGenericControl h3 = new HtmlGenericControl("h3");
                        h3.InnerText = "Table Error";
                        statusPanel.Controls.Add(h3);
                        statusPanel.Controls.Add(new LiteralControl($"No data to show for {this.selectedTable}"));
                    }
                }
                catch (Exception err)
                {
                    statusPanel.Style.Add("display", "inline");
                    HtmlGenericControl h3 = new HtmlGenericControl("h3");
                    h3.InnerText = "Bind Table Error";
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

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            searchBox.Text = string.Empty;
            this.bindTable();
        }

        protected void createInsertForm()
        {
            /*
             * Creates asp:Table dynamically. This is done because different tables have different fields and thus require different forms.
             * 
             * Each row in the form corresponds to a different field in the table
             * 
             * Header Row: <h3> tag containing "Data Entry"
             * Body Row: contains 4 cells
             *          Label Cell (Label), Input Cell (TextBox), Required Field Validator Cell (RequiredFieldValidator), Regex Validator Cell (Regular ExpressionValidator)
             *          
             *          Label Cell: ID format is lbl_<tableName>_<fieldName>
             *          Input Cell: ID format is txt_<tableName>_<fieldName>
             *          Required Field Cell: ID format is validate_<tableName>_<fieldName>
             *          Regex Validator Cell: ID format is validateType_<tableName>_<fieldName>
             *          
             * Footer Row: contains Submit button
             * 
             * */
            // add header
            TableHeaderRow headerRow = new TableHeaderRow();
            TableHeaderCell headerCell = new TableHeaderCell();
            HtmlGenericControl h3 = new HtmlGenericControl();
            h3.InnerHtml = "Data Entry";
            h3.Style.Add("display", "inline");
            headerCell.Controls.Add(h3);

            headerRow.Cells.Add(headerCell);
            formTable.Rows.Add(headerRow);

            // add body
            Dictionary<string, Dictionary<string, string>> data = db.getTableMetadata(this.selectedTable);
            List<string> colNames = db.getEditableInsertableColumnNames(null, null, data);
            if(colNames != null)
            {
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

                    switch (type)
                    {
                        case "int":
                            typeValidator.ValidationExpression = "^\\d+$";
                            break;
                        case "float":
                            typeValidator.ValidationExpression = "[+-]?([0-9]+([.][0-9]*)?|[.][0-9]+)";
                            break;
                        case "email":
                            typeValidator.ValidationExpression = ".+";
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

        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            // submit form data to DB
            try
            {
                SqlConnection conn = db.getConnection();
                conn.Open();
                List<string> values = this.getInsertValues(formTable);
                List<string> cols = db.getEditableInsertableColumnNames(this.selectedTable, conn);
                string sql = db.getSqlInsert(cols, values, this.selectedTable);
                SqlCommand command = db.getCommand(sql, conn);
                int rowsAffected = command.ExecuteNonQuery();
                db.closeOff(conn, command);
                this.bindTable();
            }
            catch (Exception err)
            {
                statusPanel.Style.Add("display", "inline");
                HtmlGenericControl h3 = new HtmlGenericControl("h3");
                h3.InnerText = "DB Error";
                statusPanel.Controls.Add(h3);
                statusPanel.Controls.Add(new LiteralControl(err.Message));

            } finally
            {
                formPanel.Style.Add("display", "none");
            }
        }

        List<string> getInsertValues(Control parent)
        {
            // Recursively gets all the values from the form to capture data to be inserted into the selected table
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
            // Shows the form to capture data to be inserted into selected table
            formPanel.Style.Add("display", "inline");
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