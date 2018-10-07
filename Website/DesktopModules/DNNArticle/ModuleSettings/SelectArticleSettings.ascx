<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SelectArticleSettings.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.ModuleSettings.SelectArticleSettings" %>
<%@ Register TagPrefix="zldnn" TagName="source" Src="~/desktopmodules/DNNArticle/ctlArticleSource.ascx" %>
<%@ Register TagPrefix="dnn" TagName="label" Src="~/controls/LabelControl.ascx" %>

<div class="dnnFormItem">
     <asp:DropDownList ID="cboType" runat="server" AutoPostBack="True" Width="315px"  OnSelectedIndexChanged="cboType_SelectedIndexChanged">
                <asp:ListItem Value="0" resourcekey="lbPassin">Pass In Article ID</asp:ListItem>
                <asp:ListItem Value="1" resourcekey="lbShowExisting">Show Existing Article(Enter Article id)</asp:ListItem>
                <asp:ListItem Value="2" resourcekey="lbShowMostRecent">Show the most recent article from...</asp:ListItem>
                <asp:ListItem Value="3" resourcekey="lbPassinByDefault">Show Existing Article if there is no pass in id</asp:ListItem>
                <asp:ListItem Value="4" resourcekey="lbShowMostRecentByDefault">Show the most recent article if there is no pass in id</asp:ListItem>
            </asp:DropDownList>
</div>

<div runat="server" id="divArticle">
    <div class="dnnFormItem">
        <dnn:label ID="lbArticleId" runat="server" ></dnn:label>
        <asp:TextBox runat="server" ID="txtArticleId"></asp:TextBox><asp:CompareValidator  ControlToValidate="txtArticleId" Type="Integer"
                           ID="CompareValidator1" runat="server" resourcekey="cvArticleId" Operator="DataTypeCheck"></asp:CompareValidator>
    </div>  
    <div class="dnnFormItem">
        <asp:Label ID="Label1" runat="server" resourcekey="lbSelectArticle"></asp:Label>
    </div>  
    <div class="dnnFormItem">
         <dnn:label ID="lblModule" runat="server" ></dnn:label>
          <asp:DropDownList ID="cboModule" runat="server" Width="400px" AutoPostBack="True"
                                           OnSelectedIndexChanged="cboModule_SelectedIndexChanged">
                        </asp:DropDownList>
    </div>  
    <div class="dnnFormItem">
        <dnn:label ID="lblArticle" runat="server" Suffix=":" />
        <asp:ListBox ID="lstArticles" runat="server" Width="400px" Height="200px" 
                             AutoPostBack="True" 
                            onselectedindexchanged="lstArticles_SelectedIndexChanged">
                        </asp:ListBox>
    </div>  
    <div class="dnnFormItem">
        
    </div>  
    <div class="dnnFormItem">
         <asp:CheckBox ID="chkKeepPassInID" runat="server"  resourcekey="KeepPassInID" />
    </div>    
</div>

<zldnn:source runat="server" ID="ctlSource" />