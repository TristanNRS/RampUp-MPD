<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="DBinsert.aspx.cs" Inherits="Test2.DBinsert" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    Insert values into:<br />
    <asp:DropDownList ID="tableList" runat="server" OnSelectedIndexChanged="tableList_SelectedIndexChanged" AutoPostBack="true" style="margin: 15px 0px"></asp:DropDownList> 
    <asp:TextBox ID="searchBox" runat="server" OnTextChanged="searchBox_TextChanged" AutoPostBack="true" style="margin: 15px 0px 15px 10px"></asp:TextBox>
    <asp:Button ID="addDataButton" runat="server"  CausesValidation="false"  Text="Add Data" style="margin: 15px 0px 15px 0px;" OnClick="addDataButton_Click" />
    <asp:GridView ID="GridView1" runat="server" EnableViewState="false" GridLines="None"
        Style="margin-bottom:25px;"
        CellPadding="10" 
        AutoGenerateColumns="False" 
        AllowPaging="True"  
        PageSize="5" 
        OnPageIndexChanging="GridView1_PageIndexChanging"
        ></asp:GridView>

    <asp:Panel ID="formPanel" runat="server">
        <asp:Table ID="formTable" runat="server" CellPadding="5"></asp:Table>
    </asp:Panel>
    <asp:Panel ID="statusPanel" runat="server"></asp:Panel>
</asp:Content>
