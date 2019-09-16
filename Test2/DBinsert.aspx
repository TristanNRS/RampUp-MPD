﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="DBinsert.aspx.cs" Inherits="Test2.DBinsert" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    Insert values into:<br />
    <asp:DropDownList ID="tableList" runat="server" OnSelectedIndexChanged="tableList_SelectedIndexChanged" AutoPostBack="true" style="margin: 15px 0px"></asp:DropDownList> 
    <asp:Button ID="addDataButton" runat="server"  CausesValidation="false"  Text="Add Data" style="margin: 15px 0px 15px 10px" OnClick="addDataButton_Click" />
    <asp:GridView ID="GridView1" runat="server"
        style="margin-bottom:25px;"
        CellPadding="10" 
        AutoGenerateColumns="False" 
        OnRowDeleting="GridView1_RowDeleting" 
        OnRowEditing="GridView1_RowEditing" 
        OnRowUpdating="GridView1_RowUpdating" 
        OnRowCancelingEdit="GridView1_RowCancelingEdit"  
        AllowPaging="True"  
        PageSize="5" 
        OnPageIndexChanging="GridView1_PageIndexChanging"
        AutoGenerateDeleteButton="True" 
        AutoGenerateEditButton="True" 
        ></asp:GridView>

    <asp:Panel ID="Panel1"  runat="server">
        <asp:Table ID="formTable" runat="server" CellPadding="5"></asp:Table>
    </asp:Panel>
    <asp:Panel ID="Panel2" runat="server"></asp:Panel>

</asp:Content>
