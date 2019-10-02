<%@ Page Title="View Data" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="DBviewData.aspx.cs" Inherits="Test2.DBviewData" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server" >

    <%--This panel is used to only show the page to authorized users (based on their role as defined in the Users table in DB)--%>
    <asp:Panel ID="authorizationPanel" runat="server">
        <div class="d-flex justify-content-center align-items-center flex-column">

            <div class="mt-5">
                <h4>View Data</h4>
            </div>

            <%--Dropdown list and search bar--%>
            <div class="d-flex flex-row mt-4">
                <asp:DropDownList ID="tableList" runat="server" OnSelectedIndexChanged="tableList_SelectedIndexChanged" AutoPostBack="true" Height="27px"></asp:DropDownList>
                <asp:Panel ID="searchPanel" runat="server">
                    <asp:TextBox ID="searchBox" runat="server" OnTextChanged="searchBox_TextChanged" AutoPostBack="true" style="display:inline; position:relative; left:15px; height:27px;"></asp:TextBox>
                    <asp:ImageButton ID="searchIcon" runat="server" ImageUrl="images/baseline_search_black_18dp.png" OnClick="searchIcon_Click" style="display:inline; position:relative; left:15px"/>
                </asp:Panel>
            </div>

            <%--GridView Data table--%>
            <div class="mt-4">
                <asp:GridView ID="GridView1" runat="server" 
                    CellPadding="10" 
                    CellSpacing="10"
                    EnableViewState="false" 
                    GridLines="None"
                    AutoGenerateColumns="false" 
                    AllowPaging="True" 
                    PageSize="5" 
                    OnPageIndexChanging="GridView1_PageIndexChanging">
                    
                </asp:GridView>
            </div>
        </div>
    </asp:Panel>

    <div class ="d-flex justify-content-center">
        <%--This panel is used to show errors or info about actions--%>
        <asp:Panel ID="statusPanel" runat="server"></asp:Panel>
    </div>
</asp:Content>
