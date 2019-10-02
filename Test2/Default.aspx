<%@ Page Title="Home" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Test2.Default" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <%--Hero image --%>
    <div class="row hero-image">
        <div class="hero-text">
            <h1>Ministry of Planning and Development</h1>
            <h2>Trinidad and Tobago</h2>
        </div>
    </div>

    <%--Vision 2030 Accordion--%>
    <div class="d-flex flex-row align-items-center" style="height:500px;">
        <div class="col-4 text-center">
            <a href="https://www.planning.gov.tt/sites/default/files/Vision%202030-%20The%20National%20Development%20Strategy%20of%20Trinidad%20and%20Tobago%202016-2030.pdf" target="_blank">
                <asp:Image ID="v2030" runat="server" ImageUrl="~/images/v2030.png" Width="300px" Height="350px"/>
            </a>
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

    <%--Carousel--%>
    <div class="row">
        <!--Carousel Wrapper-->
        <div id="carousel" class="carousel slide carousel-fade d-flex justify-content-center align-items-center" data-ride="carousel" style="width:100vw; height:75vh">
            <!--Indicators-->
            <ol class="carousel-indicators">
                <li data-target="#carousel" data-slide-to="0" class="active"></li>
                <li data-target="#carousel" data-slide-to="1"></li>
                <li data-target="#carousel" data-slide-to="2"></li>
                <li data-target="#carousel" data-slide-to="3"></li>
            </ol>
            <!--/.Indicators-->
            <!--Slides-->
            <div class="carousel-inner" role="listbox">
                <!--First slide-->
                <div class="carousel-item active" >
                    <div class="view">
                        <img id="img1" class="d-block w-100" src="images/presentationScarletIbis.jpg" alt="First slide"  style="height:75vh;"/>
                        <div class="mask rgba-black-strong"></div>
                    </div>
                    <div class="carousel-caption text-light bg-dark">
                        <h3 class="h3-responsive">Designation of the Scarlet Ibis as an Environmentally Sensitive Species</h3>
                        <p>Remarks by the Hon. Camille Robinson-Regis</p>
                    </div>
                </div>
                <!--/First slide-->

                <!--Second slide-->
                <div class="carousel-item">
                    <div class="view">
                        <video id="video1" class="video-fluid d-block mx-auto" muted="muted" loop="loop">
                            <source src="videos/Our Scarlet Ibis an Environmentally Sensitive Species.mp4" type="video/mp4" style="height:100%; width:100%"/>
                        </video>
                    </div>
                </div>
                <!--/Second slide-->

                <!--Third slide-->
                <div class="carousel-item">
                    <div class="view">
                        <img id="img2" class="d-block w-100" src="images/greenLeafAwards.jpg" alt="Third slide"  style="height:75vh; width: 75vw;"/>
                        <div class="mask rgba-black-strong"></div>
                    </div>
                    <div class="carousel-caption text-light bg-dark">
                        <h3 class="h3-responsive">EMA's 2018 Green leaf Awards</h3>
                        <p>Address by the Hon. Camille Robinson-Regis</p>
                    </div>
                </div>
                <!--/Third slide-->

                <div class="carousel-item">
                    <div class="view">
                        <video id="video2" class="video-fluid d-block mx-auto" loop="loop" muted="muted">
                            <source src="videos/100 volunteers for 1000 mangroves.mp4" type="video/mp4" style="height:75vh; width:100%"/>
                        </video>
                    </div>
                </div>

            </div>
            <!--/.Slides-->
            <!--Controls-->
            <a class="carousel-control-prev" href="#carousel" role="button" data-slide="prev">
                <span class="carousel-control-prev-icon" aria-hidden="true"></span>
                <span class="sr-only">Previous</span>
            </a>
            <a class="carousel-control-next" href="#carousel" role="button" data-slide="next">
                <span class="carousel-control-next-icon" aria-hidden="true"></span>
                <span class="sr-only">Next</span>
            </a>
            <!--/.Controls-->
        </div>
        <!--/.Carousel Wrapper-->
    </div>

    <%--VIP Cards --%>
    <div class="d-flex flex-row justify-content-center align-items-center text-center mb-5" style="height:450px;">
        <div class="col">
            <div class="card bg-light">
                <img class="card-img-top mx-auto d-block mt-3" src="images/Paula_Mae_Weekes.jpg" alt="Honourable Minister" style="width:200px; height:150px"/>
                <div class="card-body">
                    <h5 class="card-title">The President</h5>
                    <p class="card-text">Her Excellency Paula-Mae Weekes</p>
                </div>
            </div>
        </div>

        <div class="col">
            <div class="card bg-light">
                <img class="card-img-top mx-auto d-block mt-3" src="images/MPDminister.jpg" alt="Honourable Minister" style="width:200px; height:150px"/>
                <div class="card-body">
                    <h5 class="card-title">The Honourable Minister</h5>
                    <p class="card-text">The Honourable Camille Robinson-Regis</p>
                </div>
            </div>
        </div>
        <div class="col">
            <div class="card bg-light">
                <img class="card-img-top mx-auto d-block mt-3" src="images/PS_Deoraj.jpg" alt="Honourable Minister" style="width:200px; height:150px"/>
                <div class="card-body">
                    <h5 class="card-title">The Permanent Secretary</h5>
                    <p class="card-text">Mrs. Joanne Deoraj</p>
                </div>
            </div>
        </div>
    </div>

    <%--Script to pause and restart vid in carousel--%>
    <script type="text/javascript">
        $(document).ready(function () {
            var carouselItems = {
                "0": {
                    "type": "img",
                    "id": "img1"
                },
                "1": {
                    "type": "vid",
                    "id": "video1"
                },
                "2": {
                    "type": "img",
                    "id": "img2"
                },
                "3": {
                    "type": "vid",
                    "id": "video2"
                },
            }
            $("#carousel").carousel()
                .on('slide.bs.carousel', function (e) {
                    var slideFrom = $(this).find('.active').index();
                    var slideTo = $(e.relatedTarget).index();
                    if (carouselItems[slideFrom].type == "vid") {
                        // pause video
                        document.getElementById(carouselItems[slideFrom].id).pause();
                    }
                    if (carouselItems[slideTo].type == "vid") {
                        // play video from start
                        var vid = document.getElementById(carouselItems[slideTo].id);
                        vid.play();
                    }
                });

        });

    </script>
</asp:Content>
