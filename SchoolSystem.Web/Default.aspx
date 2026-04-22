<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SchoolSystem.Web._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <main>

        <!-- Hero Section -->
        <section class="jumbotron text-center">
            <h1>School Management System</h1>
            <p class="lead">
                A complete system for managing students, teachers, classes, attendance, and reports.
            </p>
        </section>

        <!-- Features -->
        <div class="row text-center">

            <section class="col-md-12">
                <h2>Clients</h2>
                <p>
                    Manage clients information, students, and teachers.
                </p>
                <p>
                    <a href="/clients" class="btn btn-outline-success">Open</a>
                </p>
            </section>

        </div>

        <hr />

        <!-- Statistics -->
        <section class="text-center">
            <h2>System Overview</h2>
            <p>
                Fast and secure management system for daily school operations.
            </p>
        </section>

    </main>

</asp:Content>