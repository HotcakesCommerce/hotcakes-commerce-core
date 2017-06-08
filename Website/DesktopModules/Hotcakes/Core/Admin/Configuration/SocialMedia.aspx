<%@ Page Title="" Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="true" CodeBehind="SocialMedia.aspx.cs" Inherits="Hotcakes.Modules.Core.Admin.Configuration.SocialMedia" %>

<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>

<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu runat="server" ID="NavMenu" BaseUrl="configuration-settings/" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <h1><%=Localization.GetString("SocialMediaSettings") %></h1>
    <hcc:MessageBox ID="msg" runat="server" />

    <div class="hcColumnLeft" style="width: 50%">
        <div class="hcForm">
            <div class="hcFormItem">
                <%=Localization.GetString("SocialMediaSettingsIntro") %>
            </div>
        </div>
        <div class="hcForm">
            <div class="hcFormItem">
                    <asp:CheckBox ID="chkUseFaceBook" resourcekey="chkUseFaceBook" runat="server" />
            </div>
        </div>
        <div class="hcForm">
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("FacebookApplicationId") %></label>
                <asp:TextBox ID="FaceBookAppIdField" runat="server" MaxLength="500"/>
            </div>
        </div>
        <div class="hcForm">
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("FacebookAdmins") %>
                    <i class="hcIconInfo">
                        <span class="hcFormInfo" style="display: none"><%=Localization.GetString("FacebookAdminsHelp") %></span>
                    </i>
                </label>
                <asp:TextBox ID="FaceBookAdminsField" runat="server" MaxLength="1000"/>
            </div>
        </div>
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:CheckBox ID="chkUseTwitter" resourcekey="chkUseTwitter" runat="server" />
            </div>
        </div>
        <div class="hcForm">
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("TwitterHandle") %>
                    <i class="hcIconInfo">
                        <span class="hcFormInfo" style="display: none"><%=Localization.GetString("TwitterHandleHelp") %></span>
                    </i>
                </label>
                <asp:TextBox ID="TwitterHandleField" runat="server" MaxLength="100"/>
            </div>
        </div>
        <div class="hcForm">
            <div class="hcFormItem">
                <label class="hcLabel"><%=Localization.GetString("DefaultTweetText") %>
                    <i class="hcIconInfo">
                        <span class="hcFormInfo" style="display: none"><%=Localization.GetString("DefaultTweetTextHelp") %></span>
                    </i>
                </label>
                <asp:TextBox ID="DefaultTweetTextField" TextMode="MultiLine" Columns="50" Rows="3" MaxLength="140" runat="server"/>
            </div>
        </div>
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:CheckBox ID="chkUseGooglePlus" resourcekey="chkUseGooglePlus" runat="server" />
            </div>
        </div>
        <div class="hcForm">
            <div class="hcFormItem">
                <asp:CheckBox ID="chkUsePinterest" resourcekey="chkUsePinterest" runat="server" />
            </div>
        </div>
    </div>

    <ul class="hcActions">
        <li>
            <asp:LinkButton ID="btnSave" resourcekey="btnSave" CssClass="hcPrimaryAction" runat="server" OnClick="btnSave_OnClick" />
        </li>
    </ul>
</asp:Content>