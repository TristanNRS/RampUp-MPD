<%@ Page Title="Insert" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="DBinsert.aspx.cs" Inherits="Test2.DBinsert" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

     <%--This panel is used to only show the page to authorized users (based on their role as defined in the Users table in DB)--%>
    <asp:Panel ID="authorizationPanel" runat="server">
        <div class="d-flex flex-column justify-content-center align-items-center mb-3">
            <div class="mt-5">
                <h4>Insert Data</h4>
            </div>

            
             <%--Dropdown list and search bar--%>
            <div class="d-flex flex-row mt-4">
                <asp:DropDownList ID="tableList" runat="server" OnSelectedIndexChanged="tableList_SelectedIndexChanged" AutoPostBack="true" style="display:inline;" Height="27px"></asp:DropDownList>
                <asp:Panel ID="searchPanel" runat="server">
                    <asp:TextBox ID="searchBox" runat="server" OnTextChanged="searchBox_TextChanged" AutoPostBack="true" style="display:inline; position:relative; left:15px; height:27px;"></asp:TextBox>
                    <asp:ImageButton ID="searchIcon" runat="server" ImageUrl="images/baseline_search_black_18dp.png" OnClick="searchIcon_Click" style="display:inline; position:relative; left:15px"/>
                </asp:Panel>
            </div>

            <asp:Button ID="addDataButton" runat="server"  CausesValidation="false"  Text="Add Data" style="margin: 15px 0px 15px 0px;" OnClick="addDataButton_Click" />

            <%--GridView Data table--%>
            <div class="mt-4">
                <asp:GridView ID="GridView1" runat="server" 
                    EnableViewState="false" 
                    GridLines="None"
                    style="margin-bottom:25px;"
                    CellPadding="10" 
                    CellSpacing="10"
                    AutoGenerateColumns="False" 
                    AllowPaging="True"  
                    PageSize="5" 
                    OnPageIndexChanging="GridView1_PageIndexChanging"
                    >
                </asp:GridView>
            </div>

            <%--Form to insert data is dynamically generated based on selected table and placed here--%>
            <div class="mt-2 mb-5">
                <asp:Panel ID="formPanel" runat="server">
                    <hr />
                    <div class="d-flex flex-column justify-content-center align-items-center">
                        <h5>Data Entry</h5>
                        <asp:Table ID="formTable" runat="server" CellPadding="5" CellSpacing="5" Width="700px"></asp:Table>
                    </div>
                </asp:Panel>
            </div>

        </div>

    </asp:Panel>
    <div class ="d-flex justify-content-center">
        <%--This panel is used to show errors or info about actions--%>
        <asp:Panel ID="statusPanel" runat="server"></asp:Panel>
    </div>
     
</asp:Content>
