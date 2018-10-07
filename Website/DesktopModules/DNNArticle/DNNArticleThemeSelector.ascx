<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DNNArticleThemeSelector.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.DNNArticleThemeSelector" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Register TagPrefix="zldnn" TagName="selector" Src="TemplateSelector.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div>
    <% if (ZLDNN.Modules.DNNArticle.pubModule.IsVersion6())
       {%>
    <script>

        $(function () {

            $("#divSetTheme<%=ModuleId.ToString(CultureInfo.InvariantCulture) %>").draggable({ handle: "#divSelectTheme<%=ModuleId.ToString(CultureInfo.InvariantCulture) %>", cursor: "move" });

        });

    </script>
    <% } %>
    <asp:LinkButton ID="cmdSetTheme" runat="server" resourcekey="SetTheme" CssClass="SetTheme"
        OnClick="cmdSetTheme_Click"></asp:LinkButton>
    <div id="divSetTheme<%=ModuleId.ToString(CultureInfo.InvariantCulture) %>" class="divSetTheme"
        style="display: <%=DisplayStatus%>">
        <div id="divSelectTheme<%=ModuleId.ToString(CultureInfo.InvariantCulture) %>" <% if (ZLDNN.Modules.DNNArticle.pubModule.IsVersion6())
       {%>style="cursor:move" <% } %>>
            <div>
               <asp:Label ID="Label1" runat="server" resourcekey="lblSelectTheme"></asp:Label>
            </div>
            <div>
                <asp:DropDownList ID="cboTheme" runat="server" AutoPostBack="True" 
                    onselectedindexchanged="cboTheme_SelectedIndexChanged">
                </asp:DropDownList>
            </div>
            <div>
                <asp:LinkButton ID="cmdUpdateTheme" resourcekey="cmdUpdateTheme" 
                    runat="server"  CssClass="link" onclick="cmdUpdateTheme_Click"></asp:LinkButton>  
                <asp:LinkButton ID="cmdSaveAsNew" resourcekey="cmdSaveAsNew" runat="server"  
                    CssClass="link" onclick="cmdSaveAsNew_Click"></asp:LinkButton>
                <div runat="server" id="divSave" Visible="False">
                   
                    <asp:Label ID="Label2" runat="server" resourcekey="lbThemeName"></asp:Label>
                    <asp:TextBox ID="txtName" runat="server"></asp:TextBox>
                    <asp:LinkButton ID="cmdSaveTheme" resourcekey="cmdSaveTheme" runat="server"  
                        CssClass="link" onclick="cmdSaveTheme_Click"></asp:LinkButton>
                        
                        <asp:LinkButton ID="cmdCancel" resourcekey="cmdCancel" runat="server"  
                        CssClass="link" onclick="cmdCancel_Click" ></asp:LinkButton>
                </div>
            </div>
        </div>
        <hr />
        <asp:Label ID="Label9" runat="server" resourcekey="lbDisplaySettings" CssClass="bordlabel"  ></asp:Label>
        <div>
            <div>
                <asp:Label ID="Label3" runat="server" resourcekey="lblCSS"></asp:Label>
                <asp:DropDownList ID="ddlCSS" runat="server">
                </asp:DropDownList>
            </div>
        </div>
        <div>
            <div>
                <asp:Label ID="Label8" runat="server" resourcekey="lbShowEditIcon"></asp:Label>
                <asp:CheckBox ID="chkShowEditIcon" runat="server" />
                <asp:Label ID="Label7" runat="server" resourcekey="lbLayout"></asp:Label>
                <asp:DropDownList ID="cboRepeatLayout" runat="server">
                    <asp:ListItem Value="0">Table</asp:ListItem>
                    <asp:ListItem Value="1">Flow</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <div>
           
                <div style="clear: both">
                    <asp:Label ID="Label4" runat="server" resourcekey="lblTemplate"></asp:Label>
                </div>
                <div>
                    <zldnn:selector runat="server" ID="ctlTemplateSelector" />
                </div>
           
        </div>
        <div>
            <div style="clear:both">
                <asp:Label ID="Label5" runat="server" resourcekey="lbHeaderTemplate"></asp:Label>
            </div>
            <div>
                <zldnn:selector runat="server" ID="ctlHeaderTemplateSelector" />
            </div>
        </div>
        <div>
            <div style="clear:both">
                <asp:Label ID="Label6" runat="server" resourcekey="lbFooterTemplate"></asp:Label>
            </div>
            <div>
                <zldnn:selector runat="server" ID="ctlFooterTemplateSelector" />
            </div>
        </div>
        <div style="clear:both">
            <asp:LinkButton ID="cmdPreview" resourcekey="cmdPreview" runat="server" CssClass="link"
                OnClick="cmdPreview_Click"></asp:LinkButton>
            <asp:LinkButton ID="cmdApply" resourcekey="cmdApply" runat="server" CssClass="link"
                OnClick="cmdApply_Click"></asp:LinkButton>
            <asp:LinkButton ID="cmdRestore" resourcekey="cmdRestore" runat="server" CssClass="link"
                OnClick="cmdRestore_Click"></asp:LinkButton>
            <asp:LinkButton ID="cmdClose" resourcekey="cmdClose" runat="server" CssClass="link"
                OnClick="cmdClose_Click"></asp:LinkButton>
        </div>
    </div>
</div>
