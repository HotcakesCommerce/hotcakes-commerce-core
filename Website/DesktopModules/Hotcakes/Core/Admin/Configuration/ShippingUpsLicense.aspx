<%@ Page Title="" Language="C#" MasterPageFile="../Admin.master" AutoEventWireup="true" CodeBehind="ShippingUpsLicense.aspx.cs" Inherits="Hotcakes.Modules.Core.Admin.Configuration.ShippingUpsLicense" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>

<%--<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        <%=Localization.GetString("UPSOnlineApplication") %>
    </h1>
    <hcc:MessageBox ID="msg" runat="server" />
    <div class="hcBlock" style="padding:1em;">
        <h3><%=Localization.GetString("LicenseAgreement") %></h3>
        <div class="hcPrintLicense" style="overflow: scroll; width:100%; height:400px;">
            <asp:Label ID="lblLicense" runat="server" />
        </div>
        <div class="hcClear" style="margin-top:0.5em;">
            <a onclick="javascript:if (window.print) { window.print(); } else { alert('<%=Localization.GetString("PrintAlert") %>'); }" href="#" class="hcSecondaryAction"><%=Localization.GetString("Print") %></a>
        </div>
    </div>

    <div class="hcForm" style="width: 100%;">
        <div class="hcFormItemHor">
            <label class="hcLabel"><%=Localization.GetString("PrimaryContactName") %></label>
            <asp:TextBox ID="inName" MaxLength="35" runat="server" />
        </div>
        <div class="hcFormItemHor">
            <label class="hcLabel"><%=Localization.GetString("ContactEmail") %></label>
            <asp:TextBox ID="inEmail" MaxLength="255" runat="server" />
            <asp:RequiredFieldValidator runat="server" ID="rfvEmail" CssClass="hcFormError" Display="Dynamic" ControlToValidate="inEmail" />
        </div>
        <div class="hcFormItemHor">
            <label class="hcLabel"><%=Localization.GetString("ContactTitle") %></label>
            <asp:TextBox ID="inTitle" MaxLength="35" runat="server" />
        </div>
        <div class="hcFormItemHor">
            <label class="hcLabel"><%=Localization.GetString("CompanyName") %></label>
            <asp:TextBox ID="inCompany" MaxLength="35" runat="server" />
        </div>
        <div class="hcFormItemHor">
            <label class="hcLabel"><%=Localization.GetString("CompanyWebsite") %></label>
            <asp:TextBox ID="inURL" MaxLength="254" runat="server" />
        </div>
        <div class="hcFormItemHor">
            <label class="hcLabel"><%=Localization.GetString("AddressLine1") %></label>
            <asp:TextBox ID="inAddress1" MaxLength="35" runat="server" />
        </div>
        <div class="hcFormItemHor">
            <label class="hcLabel"><%=Localization.GetString("AddressLine2") %></label>
            <asp:TextBox ID="inAddress2" MaxLength="35" runat="server" />
        </div>
        <div class="hcFormItemHor">
            <label class="hcLabel"><%=Localization.GetString("AddressLine3") %></label>
            <asp:TextBox ID="inAddress3" MaxLength="35" runat="server" />
        </div>
        <div class="hcFormItemHor">
            <label class="hcLabel"><%=Localization.GetString("City") %></label>
            <asp:TextBox ID="inCity" MaxLength="30" runat="server" />
        </div>
        <div class="hcFormItemHor">
            <label class="hcLabel"><%=Localization.GetString("Region") %>
                <i class="hcIconInfo">
                    <span class="hcFormInfo" style="display: none"><%=Localization.GetString("RegionHelp") %></span>
                </i>
            </label>
            <asp:DropDownList ID="inState" runat="server"/>
        </div>
        <div class="hcFormItemHor">
            <label class="hcLabel"><%=Localization.GetString("PostalCode") %></label>
            <asp:TextBox ID="inZip" MaxLength="9" runat="server" />
        </div>
        <div class="hcFormItemHor">
            <label class="hcLabel"><%=Localization.GetString("Country") %></label>
            <asp:DropDownList ID="inCountry" runat="server" AutoPostBack="true" OnSelectedIndexChanged="inCountry_SelectedIndexChanged" />
        </div>
        <div class="hcFormItemHor">
            <label class="hcLabel"><%=Localization.GetString("PhoneNumber") %></label>
            <asp:TextBox ID="inPhone" MaxLength="14" runat="server" />
            <asp:RequiredFieldValidator runat="server" ID="rfvPhoneNumber" CssClass="hcFormError" Display="Dynamic" ControlToValidate="inPhone" />
        </div>
        <div class="hcFormItemHor">
            <label class="hcLabel"><%=Localization.GetString("UPSAccountNumber") %>
                <i class="hcIconInfo">
                    <span class="hcFormInfo" style="display: none"><%=Localization.GetString("UPSAccountNumber") %></span>
                </i>
            </label>
            <asp:TextBox ID="inUPSAccountNumber" MaxLength="100" runat="server" />
        </div>
        <div class="hcFormItemHor">
            <%=Localization.GetString("OpenAccount") %>
        </div>
        <div class="hcFormItemHor">
            <%=Localization.GetString("UPSRepContactMe") %>
        </div>
        <div class="hcFormItemHor">
            <label class="hcLabel"></label>
            <asp:RadioButton ID="rbContactYes" resourcekey="rbContactYes" GroupName="rbContact" Checked="False" runat="server" />
            <asp:RadioButton ID="rbContactNo" resourcekey="rbContactNo" GroupName="rbContact" Checked="False" runat="server" />
        </div>
    </div>
    <ul class="hcActions">
        <li>
            <asp:LinkButton ID="btnAccept" resourcekey="btnAccept" runat="server" CssClass="hcPrimaryAction" onclick="btnAccept_Click" />
        </li>
        <li>
            <asp:LinkButton ID="btnCancel" resourcekey="btnCancel" CausesValidation="False" runat="server" CssClass="hcSecondaryAction" onclick="btnCancel_Click" />
        </li>
    </ul>
</asp:Content>--%>