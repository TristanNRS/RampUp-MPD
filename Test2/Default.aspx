<%@ Page Title="Home" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Test2.Default" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row hero-image">
        <div class="hero-text">
            <h1>Ministry of Planning and Development</h1>
            <h2>Trinidad and Tobago</h2>
        </div>
    </div>
    <div class="d-flex flex-row align-items-center" style="height:500px;">
        <div class="col-4 text-center">
            <asp:Image ID="v2030" runat="server" ImageUrl="~/images/v2030.png" Width="300px" Height="350px"/>
            <p>
                Vision 2030 builds the <i>pathway to the future</i> that will transform Trinidad and Tobago into a developed country, sustaining growth and development ad optimizing the quality of like of all citizens
            </p>
        </div>
        <div class="col-8 text-center">
            <div class="accordion" id="v2030Accordion">
                <div class="card">
                    <div class="card-header" id="headingOne">
                        <h2 class="mb-0">
                            <button class="btn btn-link" type="button" data-toggle="collapse" data-target="#collapseOne" aria-expanded="false" aria-controls="collapseOne">
                                Theme I
                            </button>
                        </h2>
                    </div>

                    <div id="collapseOne" class="collapse show" aria-labelledby="headingOne" data-parent="#v2030Accordion">
                        <div class="card-body">
                            Putting People First: Nurturing Our Greatest Asset
                        </div>
                    </div>
                </div>
                <div class="card">
                    <div class="card-header" id="headingTwo">
                        <h2 class="mb-0">
                            <button class="btn btn-link collapsed" type="button" data-toggle="collapse" data-target="#collapseTwo" aria-expanded="false" aria-controls="collapseTwo">
                                Theme II
                            </button>
                        </h2>
                    </div>
                    <div id="collapseTwo" class="collapse" aria-labelledby="headingTwo" data-parent="#v2030Accordion">
                        <div class="card-body">
                            Delivering Good Governance and Service Excellence
                        </div>
                    </div>
                </div>
                <div class="card">
                    <div class="card-header" id="headingThree">
                        <h2 class="mb-0">
                            <button class="btn btn-link collapsed" type="button" data-toggle="collapse" data-target="#collapseThree" aria-expanded="false" aria-controls="collapseThree">
                                Theme III
                            </button>
                        </h2>
                    </div>
                    <div id="collapseThree" class="collapse" aria-labelledby="headingThree" data-parent="#v2030Accordion">
                        <div class="card-body">
                            Improving Productivity through Quality Infrastructure and Transport
                        </div>
                    </div>
                </div>
                <div class="card">
                    <div class="card-header" id="headingFour">
                        <h2 class="mb-0">
                            <button class="btn btn-link collapsed" type="button" data-toggle="collapse" data-target="#collapseFour" aria-expanded="false" aria-controls="collapseFour">
                                Theme IV
                            </button>
                        </h2>
                    </div>
                    <div id="collapseFour" class="collapse" aria-labelledby="headingFour" data-parent="#v2030Accordion">
                        <div class="card-body">
                            Building Globally Competitive Businesses
                        </div>
                    </div>
                </div>
                <div class="card">
                    <div class="card-header" id="headingFive">
                        <h2 class="mb-0">
                            <button class="btn btn-link collapsed" type="button" data-toggle="collapse" data-target="#collapseFive" aria-expanded="false" aria-controls="collapseFive">
                                Theme V
                            </button>
                        </h2>
                    </div>
                    <div id="collapseFive" class="collapse" aria-labelledby="headingFive" data-parent="#v2030Accordion">
                        <div class="card-body">
                            Placing the Environment at the Centre of Social and Economic Development
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="d-flex flex-row justify-content-center">
        Carousel
    </div>
    <div class="d-flex flex-row justify-content-center align-items-center text-center" style="height:450px;">
        <div class="col-3">
            <div class="card bg-light">
                <img class="card-img-top mx-auto d-block mt-3" src="images/Paula_Mae_Weekes.jpg" alt="Honourable Minister" style="width:200px; height:150px"/>
                <div class="card-body">
                    <h5 class="card-title">The President</h5>
                    <p class="card-text">Her Excellency Paula-Mae Weekes</p>
                </div>
            </div>
        </div>

        <div class="col-3">
            <div class="card bg-light">
                <img class="card-img-top mx-auto d-block mt-3" src="images/MPDminister.jpg" alt="Honourable Minister" style="width:200px; height:150px"/>
                <div class="card-body">
                    <h5 class="card-title">The Honourable Minister</h5>
                    <p class="card-text">The Honourable Camille Robinson-Regis</p>
                </div>
            </div>
        </div>
        <div class="col-3">
            <div class="card bg-light">
                <img class="card-img-top mx-auto d-block mt-3" src="images/PS_Deoraj.jpg" alt="Honourable Minister" style="width:200px; height:150px"/>
                <div class="card-body">
                    <h5 class="card-title">The Permanent Secretary</h5>
                    <p class="card-text">Mrs. Joanne Deoraj</p>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
