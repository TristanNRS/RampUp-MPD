<%@ Page Title="View Data" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="DBviewData.aspx.cs" Inherits="Test2.DBviewData" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server" >

    <%--This panel is used to only show the page to authorized users (based on their role as defined in the Users table in DB)--%>
    <asp:Panel ID="authorizationPanel" runat="server">
        <div class="row mt-5">
            <div class="col">
                <h4>View Data</h4>
            </div>
        </div>

        <%--Dropdown list and search bar--%>
        <div class="row mt-4">
            <div class="col" style="position:relative;">
                <asp:DropDownList ID="tableList" runat="server" OnSelectedIndexChanged="tableList_SelectedIndexChanged" AutoPostBack="true" style="display:inline; position:absolute; top:0px; left: 15px;" Height="27px"></asp:DropDownList>
                <asp:Panel ID="searchPanel" runat="server">
                    <asp:TextBox ID="searchBox" runat="server" OnTextChanged="searchBox_TextChanged" AutoPostBack="true" style="display:inline; position:relative; left:125px; height:27px;"></asp:TextBox>
                    <asp:ImageButton ID="searchIcon" runat="server" ImageUrl="images/baseline_search_black_18dp.png" OnClick="searchIcon_Click" style="display:inline; position:relative; top:0px; left:125px"/>
                </asp:Panel>
            </div>
        </div>

        <%--GridView Data table--%>
        <div class="row mt-4">
            <div class="col">
                <asp:GridView ID="GridView1" runat="server" CellPadding="5" CellSpacing="5"
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

    <%--This panel is used to show errors or info about actions--%>
    <asp:Panel ID="statusPanel" runat="server"></asp:Panel>
</asp:Content>
