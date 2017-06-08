<%@ Page ValidateRequest="false" Title="" Language="C#" MasterPageFile="../Admin_old.master" AutoEventWireup="true" CodeBehind="Categories_EditDrillDown.aspx.cs" Inherits="MerchantTribeStore.BVAdmin.Catalog.Categories_EditDrillDown" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="uc3" %>
<%@ Register Src="../Controls/CategoryBreadCrumbTrail.ascx" TagName="CategoryBreadCrumbTrail"
    TagPrefix="uc2" %>

<asp:Content ID="headcontent" ContentPlaceHolderID="headcontent" runat="server">
    <script src="categories_editdrilldown.js" language="javascript" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <h1>
        Edit Drill-Down Category</h1>    
    <uc3:MessageBox ID="MessageBox1" runat="server" />
    <asp:Label ID="lblError" runat="server" CssClass="errormessage"></asp:Label>
    <uc2:CategoryBreadCrumbTrail ID="CategoryBreadCrumbTrail1" runat="server" /><br />    
    <asp:Panel ID="pnlMain" runat="server">
        <div class="controlarea2 padded">
        <table class="formtable" border="0" cellspacing="0" cellpadding="3" width="100%">
            <tr>
                <td class="formlabel">
                    Name:</td>
                <td class="formfield">
                    <asp:TextBox ID="NameField" runat="server" ClientIDMode="Static" MaxLength="100" TabIndex="2000"
                        Width="650px"></asp:TextBox><asp:RequiredFieldValidator ID="valName" runat="server"
                            ErrorMessage="Please enter a name" ControlToValidate="NameField">*</asp:RequiredFieldValidator></td>
                <td><asp:PlaceHolder ID="inStore" runat="Server"></asp:PlaceHolder></td>
            </tr>
            <tr>
                <td class="formlabel">
                    Description:<br />(optional)</td>
                <td colspan="2" class="formfield"><asp:TextBox ID="DescriptionField" runat="server" TextMode="MultiLine"
                width="650px" Height="150px" />
                </td>
            </tr>            
            <tr>
                <td class="formlabel">Page Name:
                    </td>
                <td colspan="2" class="formfield">
                    /<asp:TextBox ID="RewriteUrlField" ClientIDMode="Static" runat="server" Width="700px" TabIndex="2022"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="formlabel">Meta Title:</td>
                <td colspan="2" class="formfield">
                    <asp:TextBox ID="MetaTitleField" runat="server" MaxLength="512" TabIndex="2002" Width="750px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="formlabel">Meta Desc:</td>
                <td colspan="2" class="formfield">
                  <asp:TextBox ID="MetaDescriptionField" runat="server" MaxLength="255" TabIndex="2003" Width="750px" ></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="formlabel">Meta Keywords:</td>
                <td colspan="2" class="formfield">
                  <asp:TextBox ID="MetaKeywordsField" runat="server" MaxLength="255" TabIndex="2004" Width="750px"></asp:TextBox>
                </td>
            </tr>
        </table>
        </div>
        <div class="editorcontrols">
        <asp:ImageButton ID="btnCancel" TabIndex="2500" runat="server" ImageUrl="../images/buttons/Cancel.png"
                        CausesValidation="False" onclick="btnCancel_Click"></asp:ImageButton>
<asp:ImageButton ID="UpdateButton" TabIndex="2501" runat="server" 
                        ImageUrl="../images/buttons/Update.png" onclick="UpdateButton_Click">
                    </asp:ImageButton>
                    <asp:ImageButton ID="btnSaveChanges" runat="server" ImageUrl="../images/buttons/SaveChanges.png"
                        TabIndex="2502" onclick="btnSaveChanges_Click" />
        </div>
    </asp:Panel>
    <asp:HiddenField ID="BvinField" runat="server" />
    <asp:HiddenField ID="ParentIDField" runat="Server" />

    <div class="controlarea2">
    <table class="formtable" cellpadding="3">
        <tr>
            <td><asp:ImageButton ID="btnNew" runat="server" AlternateText="Add New Filter Property" ImageUrl="~/HCC/Admin/Images/Buttons/new.png"
                    EnableViewState="False" onclick="btnNew_Click" /> </td>
            <td><asp:DropDownList id="lstProperty" runat="server">                
            </asp:DropDownList></td>
            <td> depending on </td>
            <td><asp:DropDownList ID="lstParents" runat="server"></asp:DropDownList></td>
        </tr>
    </table>
    </div><br />
    &nbsp;
    <asp:Literal ID="litMain" runat="server" EnableViewState="false"></asp:Literal>       

</asp:Content>
