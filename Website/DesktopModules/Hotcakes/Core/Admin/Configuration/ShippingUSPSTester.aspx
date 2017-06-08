<%@ Page Title="" Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="true" CodeBehind="ShippingUSPSTester.aspx.cs" Inherits="Hotcakes.Modules.Core.Admin.Configuration.ShippingUSPSTester" %>

<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="uc1" %>

<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu ID="NavMenu1" runat="server" BaseUrl="configuration-admin/" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h1>
        <%=Localization.GetString("USPSDomesticRatesTester") %>
    </h1>
    
    <uc1:MessageBox ID="MessageBox1" runat="server" EnableViewState="false" />
    
    <div class="hcForm">
        <div class="hcColumnLeft" style="width: 49%">
            <div class="hcFormItemHor">
                <label class="hcLabel"><%=Localization.GetString("FromZipCode") %></label>
                <asp:TextBox ID="FromZipField" runat="server" ValidationGroup="RateTest" />
            </div>
            <div class="hcFormItemHor">
                <label class="hcLabel"><%=Localization.GetString("ToZipCode") %></label>
                <asp:TextBox ID="ToZipField" runat="server" ValidationGroup="RateTest" />
            </div>
            <div class="hcFormItemHor">
                <label class="hcLabel"><%=Localization.GetString("ServiceType") %></label>
                <asp:DropDownList ID="lstServiceTypes" runat="server" ValidationGroup="RateTest" />
            </div>
            <div class="hcFormItemHor">
                <label class="hcLabel"><%=Localization.GetString("PackagingType") %></label>
                <asp:DropDownList ID="lstPackagingType" runat="server" ValidationGroup="RateTest"/>
            </div>
        </div>
        <div class="hcColumnRight" style="width: 49%">
            <div class="hcFormItemLabel">
                <label class="hcLabel"><%=Localization.GetString("Weight") %></label>
            </div>
            <div class="hcFormItem hcFormItem33p">
                <asp:TextBox ID="WeightPoundsField" runat="server" value="0" ValidationGroup="RateTest"/><span class="hcInset"><%=Localization.GetString("Pounds") %></span>
            </div>
            <div class="hcFormItem hcFormItem33p">
                <asp:TextBox ID="WeightOuncesField" runat="server" value="0" ValidationGroup="RateTest"/><span class="hcInset"><%=Localization.GetString("Ounces") %></span>
            </div>
            <div class="hcClear"></div>
            <div class="hcFormItemLabel">
                <label class="hcLabel"><%=Localization.GetString("Dimensions") %></label>
            </div>
            <div class="hcFormItem hcFormItem33p">
                <asp:TextBox ID="LengthField" runat="server" value="0" ValidationGroup="RateTest"/><span class="hcInset"><%=Localization.GetString("Length") %></span>
            </div>
            <div class="hcFormItem hcFormItem33p">
                <asp:TextBox ID="WidthField" runat="server" value="0" ValidationGroup="RateTest"/><span class="hcInset"><%=Localization.GetString("Width") %></span>
            </div>
            <div class="hcFormItem hcFormItem33p">
                <asp:TextBox ID="HeightField" runat="server" value="0" ValidationGroup="RateTest"/><span class="hcInset"><%=Localization.GetString("Height") %></span>
            </div>
        </div>
    </div>
    
    <ul class="hcActions">
        <li>
            <asp:LinkButton ID="btnGetRates" resourcekey="btnGetRates" CssClass="hcPrimaryAction" runat="server" OnClick="btnGetRates_Click" ValidationGroup="RateTest" />
        </li>
    </ul>
    
    <asp:Panel runat="server" ID="pnlRates" CssClass="hcBlock" Visible="False">
        <div style="padding: 1.0em;">
    
            <h3><%=Localization.GetString("Results") %></h3>
            
            <div class="hcClear">
                <asp:Literal ID="litRates" runat="server"/>
            </div>

            <div class="hcClear" style="overflow: auto; height: 500px; width: 100%;">
                <div class="hcClear">
                    <asp:Literal ID="litMessages" runat="server"/>
                </div>
                <div class="hcClear">
                    <asp:Literal ID="litXml" runat="server"/>
                </div>
            </div>
            
        </div>
    </asp:Panel>

</asp:Content>


