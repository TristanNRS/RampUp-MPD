﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="DBupdate.aspx.cs" Inherits="Test2.DBupdate" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    Update values from:<br />
    <asp:DropDownList ID="tableList" runat="server" OnSelectedIndexChanged="tableList_SelectedIndexChanged" AutoPostBack="true" style="margin: 15px 0px"></asp:DropDownList> 
    <asp:GridView ID="GridView1" runat="server"
        style="margin-bottom:25px;"
        CellPadding="10" 
        AutoGenerateColumns="False" 
        OnRowEditing="GridView1_RowEditing" 
        OnRowUpdating="GridView1_RowUpdating" 
        OnRowCancelingEdit="GridView1_RowCancelingEdit"  
        AllowPaging="True"  
        PageSize="5" 
        OnPageIndexChanging="GridView1_PageIndexChanging"
        AutoGenerateEditButton="True" 
        >
    </asp:GridView>

    <asp:Panel ID="statusPanel" runat="server"></asp:Panel>
</asp:Content>
