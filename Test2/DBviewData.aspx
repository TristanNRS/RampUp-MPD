<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="DBviewData.aspx.cs" Inherits="Test2.DBviewData" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    View data from:<br />
    <asp:DropDownList ID="tableList" runat="server" OnSelectedIndexChanged="tableList_SelectedIndexChanged" AutoPostBack="true" style="margin: 15px 0px;"></asp:DropDownList>
    <asp:GridView ID="GridView1" runat="server" 
        CellPadding="10" 
        AutoGenerateColumns="false" 
        AllowPaging="True" 
         PageSize="5" 
        OnPageIndexChanging="GridView1_PageIndexChanging"></asp:GridView>

    <asp:Panel ID="statusPanel" runat="server"></asp:Panel>
</asp:Content>
