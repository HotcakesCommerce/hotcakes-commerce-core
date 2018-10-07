<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ctlArticleSource.ascx.cs"
            Inherits=" ZLDNN.Modules.DNNArticle.ctlArticleSource" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="label" Src="~/controls/LabelControl.ascx" %>
<%--<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>--%>
<%@ Register TagPrefix="zldnn" TagName="cbocategoryselector" Src="~/desktopmodules/dnnarticle/usercontrols/cbocategoryselector.ascx" %>

<style>
    .RadTreeView .rtChk {
    float: none;
    
}
</style>
<div class="dnnFormItem  dnnClear">
    <h2 id="H2" class="dnnFormSectionHead">
        <a href="" class=""><asp:Label ID="lbSource" runat="server"></asp:Label> </a></h2>
    <fieldset>
        <div class="dnnFormItem" runat="server" id="divEnableMultiInstance">
            <asp:CheckBox ID="chkEnableMultiInstance" runat="server" AutoPostBack="true" 
                resourcekey="chkEnableMultiInstances"  CssClass="Normal"
                oncheckedchanged="chkEnableMultiInstance_CheckedChanged" ></asp:CheckBox>
        </div>
        <div runat="server" id="divOneInstance">
            
            <div class="dnnFormItem">
                <dnn:Label ID="lbPortal" runat="server" ></dnn:Label>
                <asp:DropDownList ID="cboPortals" runat="server" AutoPostBack="True"   OnSelectedIndexChanged="cboPortals_SelectedIndexChanged">
                        </asp:DropDownList>
            </div>
            <div class="dnnFormItem" runat="server" id="divMultiModules">
                <dnn:label ID="lbMultiModules" runat="server" ></dnn:label>
                <asp:CheckBox ID="chkMultiModules" runat="server" AutoPostBack="true"  OnCheckedChanged="chkMultiModules_CheckedChanged"></asp:CheckBox>
            </div>
            <div class="dnnFormItem">
                <dnn:label ID="lbModule" runat="server" ></dnn:label>
                <asp:DropDownList ID="cboModule" runat="server"  AutoPostBack="True"  OnSelectedIndexChanged="cboModule_SelectedIndexChanged">
                        </asp:DropDownList><br />
                        <asp:CheckBoxList ID="chkModules" runat="server" >
                        </asp:CheckBoxList>
            </div>
            <div class="dnnFormItem" runat="server" id="divSelectSubCategories" Visible="False">
                 <dnn:label ID="lbSelectSubCategories" runat="server" Suffix=":" />
                 <asp:CheckBox ID="chkSelectSubCategories" runat="server"  ></asp:CheckBox>
            </div>
            <div class="dnnFormItem" runat="server" id="divOrCategory">
                <dnn:label ID="lblOrCategory" runat="server" Suffix=":" />
                <%--<telerik:RadTreeView ID="radtvCategories" runat="server" Visible="False" OnNodeExpand="RadtvCategory_NodeExpand"  Width="500px" Height="150px" BorderColor="#cccccc" BorderWidth="1px" BorderStyle="Solid" />--%>
                   <zldnn:cbocategoryselector runat="server" ID="orCategories" />
            </div>
            <div class="dnnFormItem" runat="server" id="divAndCategory">
                <dnn:label ID="lblAndCategory" runat="server" Suffix=":" />
                <%--<telerik:RadTreeView ID="radtvAndCategories" runat="server" Visible="False" OnNodeExpand="RadtvCategory_NodeExpand"  Width="500px" Height="150px" BorderColor="#cccccc" BorderWidth="1px" BorderStyle="Solid" />--%>
                       <zldnn:cbocategoryselector runat="server" ID="andCategories" />
            </div>
            <div class="dnnFormItem" runat="server" id="divExcludeCategory" visible="false" >
               <dnn:label ID="lbExcludeCategory" runat="server" Suffix=":" />
                <%--<telerik:RadTreeView ID="radtvExculdeCategories" runat="server" Visible="False " OnNodeExpand="RadtvCategory_NodeExpand"  Width="500px" Height="150px" BorderColor="#cccccc" BorderWidth="1px" BorderStyle="Solid" />--%>
                       <zldnn:cbocategoryselector runat="server" ID="excludeCategories" />
            </div>
      </div>
      <div runat="server" id="divMultiInstances">
             <table cellspacing="3" cellpadding="3" border="0" summary="DNNArticle Settings Design Table">
                <tr>
                    <td  >
                        <dnn:Label ID="lbPortals" runat="server" >
                        </dnn:Label></td>
                    <td>
                     <dnn:Label ID="lbModules" runat="server" >
                        </dnn:Label>
                    </td>
                    <td>
                     <dnn:Label ID="lbCategory" runat="server" >
                        </dnn:Label>
                    </td>
                </tr>
                <tr>
                    <td   >
                      <asp:DropDownList ID="cboMPortals" runat="server" AutoPostBack="True" Width="200" 
                             onselectedindexchanged="cboMPortals_SelectedIndexChanged" >
                        </asp:DropDownList>
                    </td>
                    <td>
                         <asp:DropDownList ID="cboMModules" runat="server" AutoPostBack="True" 
                             Width="200"  
                             onselectedindexchanged="cboMModules_SelectedIndexChanged" >
                        </asp:DropDownList></td>
                         <td>
                         <asp:DropDownList ID="cboMCategories" runat="server"  Width="200"  >
                        </asp:DropDownList></td>
                </tr>
                <tr>
                    <td colspan="3">
                        <asp:LinkButton ID="cmdAdd" resourcekey="cmdAdd" runat="server" 
                            onclick="cmdAdd_Click"></asp:LinkButton>
                    </td>
                </tr>
                <tr >
                    <td colspan="3">
                        <asp:DataList ID="lstCategory" runat="server" OnItemCommand="lst_ItemCommand" OnItemDataBound="lst_OnItemDataBound">
                            <ItemTemplate>
                                <table cellspacing="4" cellpadding="4" border="0" width="100%">
                                    <tr>
                                        <td class="Normal" style="width: 25px; text-align: left">
                                            <asp:LinkButton CausesValidation="false" CommandName="Delete" ID="cmdDelete" runat="server">
                                                <asp:Image ID="Image1" runat="server" ImageUrl="~/images/delete.gif" AlternateText="Delete" />
                                                 </asp:LinkButton>
                                             </td>
                                        <td class="Normal" style="width: 90%; text-align: right">
                                            <asp:Label ID="lbTitle"  runat="server">
                                            </asp:Label> <asp:TextBox runat="server" ID="txtid" Visible="False"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                        </asp:DataList>
                    </td>
                </tr>
                
            </table>
        </div>
       
    </fieldset>
</div>

