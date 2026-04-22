<%@ Page Title="Clients" Async="true" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Clients.aspx.cs" Inherits="SchoolSystem.Web.Clients" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<div class="container mt-4">

    <h2 class="mb-4">Clients Management 123</h2>

    <!-- Filters -->
    <div class="card mb-4">
        <div class="card-body">

            <div class="row">

                <!-- Search -->
                <div class="col-md-3 mb-3">
                    <label>Search</label>
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control"
                        placeholder="Name / Username"></asp:TextBox>
                </div>

                <!-- Gender -->
                <div class="col-md-2 mb-3">
                    <label>Gender</label>
                    <asp:DropDownList ID="ddlGender" runat="server" CssClass="form-control">
                        <asp:ListItem Text="All" Value=""></asp:ListItem>
                    </asp:DropDownList>
                </div>

                <!-- Type -->
                <div class="col-md-2 mb-3">
                    <label>User Type</label>
                    <asp:DropDownList ID="ddlType" runat="server" CssClass="form-control">
                        <asp:ListItem Text="All" Value=""></asp:ListItem>
                    </asp:DropDownList>
                </div>

                <!-- Date From -->
                <div class="col-md-2 mb-3">
                    <label>Date From</label>
                    <asp:TextBox ID="txtDateFrom" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                </div>

                <!-- Date To -->
                <div class="col-md-2 mb-3">
                    <label>Date To</label>
                    <asp:TextBox ID="txtDateTo" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                </div>
            </div>

        </div>
    </div>

    <!-- Table -->
    <div class="table-responsive">

       <asp:GridView ID="gvClients" runat="server"   CssClass="table table-bordered table-hover table-striped" >

            <Columns>

                <asp:BoundField DataField="Id" HeaderText="ID" />

                <asp:BoundField DataField="FullName" HeaderText="Full Name" />

                <asp:BoundField DataField="UserName" HeaderText="Username" />

                <asp:BoundField DataField="Classes" HeaderText="Classes" />

                <asp:BoundField DataField="UserType" HeaderText="User Type" />

            </Columns>

            <EmptyDataTemplate>
                No Data Found
            </EmptyDataTemplate>

        </asp:GridView>

    </div>

</div>

</asp:Content>