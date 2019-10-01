<%@ Page Title="About Us" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="AboutUs.aspx.cs" Inherits="Test2.AboutUs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="d-flex flex-row justify-content-center mt-4">
        <asp:Image ID="Image1" runat="server" ImageUrl="~/images/ministry_logo.jpg" Height="150px" Width="250px"/>
    </div>  
    <div class="card-deck mt-5">
        <div class="card">
            <div class="card-body">
                <h5 class="card-title">What are we?</h5>
                <p class="card-text">
                    We are a key collaborative Government Ministry, providing the necessary policy guidance and development, technical support, advice and feasibility assessment for national advancement. Our principal mandate is national development concentrated on four main pillars which are economic development, social development, spatial development and environmental development. <br />
                    <br />
                    We also serve as the focal point for international and regional agencies such as the Inter-American Development Bank (IDB), the United Nations system of agencies, the European Union and the Caribbean Development Bank.

                </p>
            </div>
        </div>
        <div class="card">
            <div class="card-body">
                <h5 class="card-title">What do we do?</h5>
                <div class="card-text">
                    The Ministry of Planning and Development also facilitates national development through the following:<br />
                    <ul>
                        <li>Coordinating all stakeholders in the development of Trinidad and Tobago’s National Strategy for Development, Vision 2030;</li>
                        <li>Coordination of national statistics;</li>
                        <li>Environmental policy, planning and management;</li>
                        <li>National monitoring and evaluation;</li>
                        <li>Socio-economic planning, coordinating and monitoring;</li>
                        <li>Spatial development;</li>
                        <li>and technical cooperation on special projects and programmes inter alia.</li>
                    </ul>
                </div>
            </div>
        </div>
    </div>
    <div class="d-flex flex-row justify-content-center mt-5 mb-5 text-center" >
        <div style="width:65vw">
            There are a number of supporting statutory boards and agencies all working in unison to achieve the objectives set by our mandate. <br /> <br />
            Please browse, search and leave us feedback, also access our other electronic mediums, which are regularly updated.  The Ministry of Planning and Development is in the business of service, and we strive to deliver to our stakeholders with dedication and efficiency understanding that the people we serve are our greatest assets. 
        </div>
    </div>

</asp:Content>
