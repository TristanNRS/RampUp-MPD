<%@ Page Title="Insert" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="DBinsert.aspx.cs" Inherits="Test2.DBinsert" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:Panel ID="authorizationPanel" runat="server">
        <div class="row mt-5">
            <div class="col" style="position:relative;">
                <asp:DropDownList ID="tableList" runat="server" OnSelectedIndexChanged="tableList_SelectedIndexChanged" AutoPostBack="true" style="display:inline; position:absolute; top:0px; left: 15px;" Height="27px"></asp:DropDownList>
                <asp:Panel ID="searchPanel" runat="server">
                    <asp:TextBox ID="searchBox" runat="server" OnTextChanged="searchBox_TextChanged" AutoPostBack="true" style="display:inline; position:relative; left:125px; height:27px;"></asp:TextBox>
                    <asp:ImageButton ID="searchIcon" runat="server" ImageUrl="images/baseline_search_black_18dp.png" OnClick="searchIcon_Click" style="display:inline; position:relative; top:0px; left:125px"/>
                    <asp:Button ID="addDataButton" runat="server"  CausesValidation="false"  Text="Add Data" style="margin: 15px 0px 15px 0px;" OnClick="addDataButton_Click" />
                </asp:Panel>
            </div>
        </div>
        <div class="row mt-4">
            <div class="col">
                <asp:GridView ID="GridView1" runat="server" EnableViewState="false" GridLines="None"
                    Style="margin-bottom:25px;"
                    CellPadding="10" 
                    AutoGenerateColumns="False" 
                    AllowPaging="True"  
                    PageSize="5" 
                    OnPageIndexChanging="GridView1_PageIndexChanging"
                    ></asp:GridView>
            </div>
        </div>
        <div class="row mt-3">
            <div class="col">
                <asp:Panel ID="formPanel" runat="server">
                    <asp:Table ID="formTable" runat="server" CellPadding="5"></asp:Table>
                </asp:Panel>
            </div>
        </div>
    </asp:Panel>

    <asp:Panel ID="statusPanel" runat="server"></asp:Panel>
</asp:Content>
