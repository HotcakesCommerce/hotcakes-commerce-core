<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DNNArticleListBaseSettings.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.ModuleSettings.DNNArticleListBaseSettings" %>
<%@ Register TagPrefix="dnn" TagName="label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="zldnn" TagName="source" Src="~/desktopmodules/DNNArticle/ctlArticleSource.ascx" %>
<%@ Register TagPrefix="zldnn" TagName="selector" Src="~/desktopmodules/DNNArticle/TemplateSelector.ascx" %>

<div class="dnnFormItem">
  <dnn:label ID="lbModuleId" runat="server" ControlName="lbTabs" />
  <asp:Label ID="ArticleModuleId" runat="server" CssClass="Normal"></asp:Label>
</div>
<zldnn:source runat="server" ID="ctlSource" />
<div class="dnnFormItem  dnnClear" runat="server" id="divFilter">
    <h2 id="H2" class="dnnFormSectionHead">
        <a href="" class="">
            <%=LocalizeString("dshFilter")%></a></h2>
    <fieldset>
        <div class="dnnFormItem">
            <dnn:label ID="lbShowHasAttachFilesOnly" runat="server" />
            <asp:CheckBox ID="chkHasAttachedFilesOnly" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="lbShowUserArticleOnly" runat="server" />
            <asp:CheckBox ID="chkShowUserArticleOnly" AutoPostBack="True" runat="server" OnCheckedChanged="chkShowUserArticleOnly_CheckedChanged" />
                             
        </div>
        <div class="dnnFormItem" runat="server" id="divUserPassin">
             <dnn:label ID="lbUserIDPassIn" runat="server" ></dnn:label>
             <asp:TextBox ID="txtUserIDPassIn" runat="server" CssClass="NormalTextBox"></asp:TextBox>
        </div>
        <div class="dnnFormItem">
             <dnn:label ID="lbShowRelatedArticle" runat="server"></dnn:label>
             <asp:CheckBox ID="chkShowRelatedArticle" runat="server" AutoPostBack="true" OnCheckedChanged="chkShowRelatedArticle_CheckedChanged">
                                    </asp:CheckBox>
        </div>
        <div class="dnnFormItem" runat="server" id="divViewModule">
            <dnn:label ID="lbViewModule" runat="server"></dnn:label>
            <asp:DropDownList ID="cboViewModule" runat="server" Width="200px">
                                    </asp:DropDownList>
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="lbShowFavoritOnly" runat="server" />
            <asp:CheckBox ID="chkShowFavoritOnly" runat="server" />
        </div>
         <div class="dnnFormItem">
             <dnn:label ID="lbFilterTags" runat="server"></dnn:label>
             <asp:CheckBox ID="chkFilterTags" runat="server" AutoPostBack="True" OnCheckedChanged="chkFilterTags_CheckedChanged">
                                    </asp:CheckBox>
        </div>
         <div class="dnnFormItem" runat="server" id="divTags">
             <dnn:label ID="lbTags" runat="server"></dnn:label>
             <asp:TextBox ID="txtTags" runat="server" Width="300px" CssClass="NormalTextBox"></asp:TextBox>
        </div>
         <div class="dnnFormItem" runat="server" id="divTagParameter">
             <dnn:label ID="lbTagParameter" runat="server"></dnn:label>
             <asp:TextBox ID="txtTagParameter" runat="server" Width="300px" CssClass="NormalTextBox"></asp:TextBox>
        </div>
        <div class="dnnFormItem">
             <dnn:label ID="lbShowRelatedEntryOnly" runat="server" />
             <asp:CheckBox ID="chkShowRelatedEntryOnly" AutoPostBack="True" runat="server" OnCheckedChanged="chkShowRelatedEntryOnly_CheckedChanged" />
                              
        </div>
        <div class="dnnFormItem" runat="server" id="divShowRelatedEntryOnly">
              <dnn:label ID="lbRelatedEntryPara" runat="server" ></dnn:label>
               <asp:TextBox ID="txtRelatedEntryPara" runat="server" CssClass="NormalTextBox"></asp:TextBox>
        </div>
        
    </fieldset>
</div>
<div class="dnnFormItem">
    <dnn:label ID="lbNumber" runat="server" ></dnn:label>
    <asp:TextBox ID="txtNumber" runat="server" Width="200px" CssClass="NormalTextBox"></asp:TextBox>
            <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="txtNumber"
                ErrorMessage="Integer" Operator="DataTypeCheck" Type="Integer" CssClass="NormalRed"></asp:CompareValidator>
</div>
<div class="dnnFormItem">
    <dnn:label ID="lbRecent" runat="server" ></dnn:label>
    <asp:TextBox ID="txtRecent" runat="server" Width="200px" CssClass="NormalTextBox"></asp:TextBox>
            <asp:CompareValidator ID="CompareValidator3" runat="server" ControlToValidate="txtRecent"
                ErrorMessage="Integer" Operator="DataTypeCheck" Type="Integer" CssClass="NormalRed"></asp:CompareValidator>
</div>
<div class="dnnFormItem">
    <dnn:label ID="lbTabs" runat="server" ControlName="lbTabs" />
    <asp:DropDownList ID="cboTabs" runat="server" Width="200px">
            </asp:DropDownList>
</div>
<div class="dnnFormItem">
    <dnn:label ID="lbShowItemFrom" runat="server" ></dnn:label>
    <asp:TextBox ID="txtShowItemFrom" runat="server" Width="200px" CssClass="NormalTextBox"></asp:TextBox>
            <asp:CompareValidator ID="CompareValidator4" runat="server" ControlToValidate="txtShowItemFrom"
                ErrorMessage="Integer" Operator="DataTypeCheck" Type="Integer" CssClass="NormalRed"></asp:CompareValidator>
</div>
<div class="dnnFormItem">
    <dnn:label ID="lblDisplayAllArticles" runat="server"></dnn:label>
    <asp:DropDownList ID="cboDisplayArticles" runat="server" CssClass="Normal">
                <asp:ListItem Value="0">All</asp:ListItem>
                <asp:ListItem Value="1">Active only</asp:ListItem>
                <asp:ListItem Value="2">Expired only</asp:ListItem>
                <asp:ListItem Value="3">To be published only</asp:ListItem>
                <asp:ListItem Value="4">Not Expired</asp:ListItem>
                <asp:ListItem Value="5">Published(Activate and Expired)</asp:ListItem>
            </asp:DropDownList>
</div>
<div class="dnnFormItem">
    <dnn:label ID="lbFeaturedDisplayType" runat="server" />
    <asp:DropDownList ID="cboFeaturedDisplayType" runat="server" CssClass="Normal">
                <asp:ListItem Value="0">All</asp:ListItem>
                <asp:ListItem Value="1">Featured only</asp:ListItem>
                <asp:ListItem Value="2">Not Featured only</asp:ListItem>
            </asp:DropDownList>
</div>
<div class="dnnFormItem">
    <dnn:label ID="lbApprovedDisplayType" runat="server" />
    <asp:DropDownList ID="cboApprovedDisplayType" runat="server" CssClass="Normal">
                <asp:ListItem Value="0">All</asp:ListItem>
                <asp:ListItem Value="1">Approved only</asp:ListItem>
                <asp:ListItem Value="2">Not Approved only</asp:ListItem>
            </asp:DropDownList>
</div>
<div class="dnnFormItem">
     <dnn:label ID="lbCheckPermission" runat="server" />
     <asp:CheckBox ID="chkCheckPermission" runat="server" />
</div>
<div class="dnnFormItem" runat="server" id="divShowEditIcon">
    <dnn:label ID="lbShowEditIcon" runat="server" />
    <asp:CheckBox ID="chkShowEditIcon" runat="server" />
</div>
<div class="dnnFormItem"  runat="server" id="divEnableRSS">
    <dnn:label ID="lbEnableRSS" runat="server" ></dnn:label>
    <asp:CheckBox ID="chkEnableRSS" runat="server"></asp:CheckBox>
</div>
<div class="dnnFormItem">
     <dnn:label ID="lbDisplayRandom" runat="server" ></dnn:label>
     <asp:CheckBox ID="chkDisplayRandom" runat="server"></asp:CheckBox>
</div>
<div class="dnnFormItem">
    <dnn:label ID="lbPageSize" runat="server"></dnn:label>
    <asp:TextBox ID="txtPageSize" runat="server" CssClass="NormalTextBox"></asp:TextBox>
            <asp:RangeValidator ID="RangeValidator2" runat="server" ControlToValidate="txtPageSize"
                ErrorMessage="(1-100)" Type="Integer" MaximumValue="100" MinimumValue="1"></asp:RangeValidator>
</div>
<div class="dnnFormItem" runat="server" id="divEnablePage">
     <dnn:label ID="lbEnablePage" runat="server"></dnn:label>
     <asp:CheckBox ID="chkEnablePage" runat="server"></asp:CheckBox>
</div>
<div class="dnnFormItem" runat="server" id="divRepeatColumn">
    <dnn:label ID="lbRepeatColumn" runat="server" ></dnn:label>
    <asp:TextBox ID="txtRepeatColumn" runat="server" CssClass="NormalTextBox"></asp:TextBox><asp:RangeValidator
                ID="RangeValidator4" runat="server" ControlToValidate="txtRepeatColumn" Type="Integer"
                MinimumValue="1" MaximumValue="40" ErrorMessage="(1-40)"></asp:RangeValidator>
</div>
<div class="dnnFormItem" runat="server" id="divRepeatLayout">
    <dnn:label ID="lblRepeatLayout" runat="server"></dnn:label>
    <asp:DropDownList ID="cboRepeatLayout" runat="server">
                <asp:ListItem Value="0">Table</asp:ListItem>
                <asp:ListItem Value="1">Flow</asp:ListItem>
            </asp:DropDownList>
</div>
<div class="dnnFormItem" runat="server" id="divRepeatDirection">
    <dnn:label ID="lbRepeatDirection" runat="server" ></dnn:label>
    <asp:DropDownList ID="cboRepeatDirection" runat="server" Width="152px" CssClass="Normal">
                    <asp:ListItem Value="0">Horizontal</asp:ListItem>
                    <asp:ListItem Value="1">Vertical</asp:ListItem>
                </asp:DropDownList>
</div>
<div class="dnnFormItem" runat="server" id="divPageControlType">
    <dnn:label ID="lbPageControlType" runat="server" ></dnn:label>
    <asp:DropDownList ID="cboPageControlType" runat="server" Width="152px" CssClass="Normal">
                    <asp:ListItem Value="0" resourcekey="PageDropDownList"></asp:ListItem>
                    <asp:ListItem Value="1" resourcekey="PageNumberList"></asp:ListItem>
                    <asp:ListItem Value="2" resourcekey="PageNumberListWithoutPostBack"></asp:ListItem>
                </asp:DropDownList>
</div>
<div class="dnnFormItem" runat="server" id="divEnableAjax">
     <dnn:label ID="lbEnableAjax" runat="server"></dnn:label>
     <asp:CheckBox ID="chkEnableAjax" runat="server"></asp:CheckBox>
</div>

<div class="dnnFormItem">
    <dnn:label ID="lbOrderField" runat="server" ></dnn:label>
    <asp:DropDownList ID="cboOrderField" runat="server" Width="103px" CssClass="Normal">
                <asp:ListItem Value="VIEWORDER" Selected="True">ViewOrder</asp:ListItem>
                <asp:ListItem Value="TITLE">Title</asp:ListItem>
                <asp:ListItem Value="CREATEDDATE">Created Date</asp:ListItem>
                <asp:ListItem Value="PUBLISHDATE">Publish Date</asp:ListItem>
                <asp:ListItem Value="LASTVIEWDATE">Last View Date</asp:ListItem>
                <asp:ListItem Value="UPDATEDATE">Update Date</asp:ListItem>
                <asp:ListItem Value="EXPIREDATE">Expire Date</asp:ListItem>
                <asp:ListItem Value="CLICKS">View count</asp:ListItem>
                <asp:ListItem Value="COMMENTNUMBER">Number of Comments</asp:ListItem>
                <asp:ListItem Value="RATING">Rating</asp:ListItem>
                <asp:ListItem Value="ITEMID">Articel Id</asp:ListItem>
            </asp:DropDownList>
            <asp:DropDownList ID="cboOrder" runat="server" Width="62px" CssClass="Normal">
                <asp:ListItem Value="ASC">ASC</asp:ListItem>
                <asp:ListItem Value="DESC" Selected="True">DESC</asp:ListItem>
            </asp:DropDownList>
</div>
<div class="dnnFormItem" runat="server" id="divUserSort">
    <dnn:label ID="lbUserSort" runat="server" ></dnn:label>
    <asp:CheckBox ID="chkUserSort" runat="server"></asp:CheckBox>
</div>
<div class="dnnFormItem"  runat="server" id="divOptMode">
    <dnn:label ID="lbOptMode" runat="server"></dnn:label>
    <asp:CheckBox ID="chkOptMode" runat="server"></asp:CheckBox>
</div>
<div class="dnnFormItem" runat="server" id="divHideMeIfNoRecord">
    <dnn:label ID="lbHideMeIfNoRecord" runat="server" />
    <asp:CheckBox ID="chkHideMeIfNoRecord" runat="server" />
</div>
<div class="dnnFormItem"  runat="server" id="divShowMessageIfNoRecord">
    <dnn:label ID="lbShowMessageIfNoRecord" runat="server" />
    <asp:CheckBox ID="chkShowMessageIfNoRecord" runat="server" />
</div>
<div class="dnnFormItem"  runat="server" id="divMessageofNoRecord">
    <dnn:label ID="lbMessageofNoRecord" runat="server" />
     <asp:TextBox ID="txtMessageofNoRecord" runat="server"></asp:TextBox>
</div>
<div class="dnnFormItem">
    <dnn:label ID="lblCSS" runat="server" Suffix=":"></dnn:label>
    <asp:DropDownList ID="ddlCSS" runat="server">
</asp:DropDownList>
</div>
<div class="dnnFormItem">
     <dnn:label ID="lblTemplate" runat="server" ControlName="txtTemplate" Suffix=":">
     </dnn:label>
<zldnn:selector runat="server" ID="ctlTemplateSelector" />
</div>
<div class="dnnFormItem">
    <dnn:label ID="lbShowFeaturedFirst" runat="server"></dnn:label>
    <asp:CheckBox ID="chkShowFeaturedFirst" runat="server"></asp:CheckBox>
</div>
<div class="dnnFormItem">
    <dnn:label ID="lblFeaturedTemplate" runat="server" ControlName="txtTemplate" Suffix=":">
            </dnn:label>
 <zldnn:selector runat="server" ID="ctlFeaturedTemplateSelector" />
</div>
<div class="dnnFormItem">
    <dnn:label ID="lblTopTemplateNumber" runat="server" ></dnn:label>
    <asp:TextBox ID="txtTopTemplateNumber" runat="server" Width="120px" CssClass="NormalTextBox"></asp:TextBox>
            <asp:CompareValidator ID="CompareValidator2" runat="server" ControlToValidate="txtTopTemplateNumber"
                ErrorMessage="Integer" Operator="DataTypeCheck" Type="Integer" CssClass="NormalRed"></asp:CompareValidator>
</div>
<div class="dnnFormItem">
    <dnn:label ID="lblTopTemplate" runat="server" ControlName="txtTemplate" Suffix=":">
            </dnn:label>
<zldnn:selector runat="server" ID="ctlTopTemplateSelector" />
</div>
<div class="dnnFormItem">
    <dnn:label ID="lbHeaderTemplate" runat="server" />
    <zldnn:selector runat="server" ID="ctlHeaderTemplateSelector" />
</div>
<div class="dnnFormItem">
    <dnn:label ID="lbFooterTemplate" runat="server" />
    <zldnn:selector runat="server" ID="ctlFooterTemplateSelector" />
</div>