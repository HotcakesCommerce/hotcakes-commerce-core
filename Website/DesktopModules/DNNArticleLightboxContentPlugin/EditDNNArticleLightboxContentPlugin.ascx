<%@ Control Language="C#" Inherits="ZLDNN.Modules.DNNArticleLightboxContentPlugin.EditDNNArticleLightboxContentPlugin" AutoEventWireup="true" Explicit="True" Codebehind="EditDNNArticleLightboxContentPlugin.ascx.cs" %>
<%@ Register TagPrefix="Portal" TagName="URL" Src="~/controls/URLControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="TextEditor" Src="~/controls/TextEditor.ascx"%>



<table width="650" cellspacing="0" cellpadding="0" border="0" summary="Edit Table" class="dnnFormItem">
    <tr runat="server" id="trCode">
        <td class="SubHead" nowrap width="217">
            <dnn:label ID="lbCode" runat="server" Suffix=":" ></dnn:label>
        </td>
        <td class="SubHead">
            <asp:TextBox ID="txtCode" runat="server" Width="304px"   ReadOnly="true" CssClass="NormalTextBox"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="SubHead" nowrap width="217">
            <dnn:label ID="lbMediaType" Suffix=":" runat="server"></dnn:label>
        </td>
        <td class="SubHead">
            <asp:DropDownList ID="cboMediaType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="cboMediaType_SelectedIndexChanged">
                <asp:ListItem Text="Image" Value="Image"></asp:ListItem>
                <asp:ListItem Text="FLV Video" Value="FLV"></asp:ListItem>
                <asp:ListItem Text="Flash(SWF)" Value="SWF"></asp:ListItem>
                <asp:ListItem Text="HTML" Value="HTML"></asp:ListItem>
                <asp:ListItem Text="Embed Resource" Value="Embed"></asp:ListItem>
                <asp:ListItem Text="Module" Value="Module"></asp:ListItem>
                <asp:ListItem Text="Web address" Value="URL"></asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
    <tr runat="server" id="trMedia">
        <td class="SubHead" nowrap width="217">
            <dnn:label ID="lbMediaURL" Suffix=":" runat="server"></dnn:label>
        </td>
        <td class="SubHead">
            <portal:url id="ctlMediaURL" runat="server" width="250" showtabs="False" showurls="False" showtrack="False"
                        showlog="False"  Required="False"  showupload="True"  shownewwindow="False" />
        </td>
    </tr>
    <tr valign="top" runat="server" id="trContent">
        <td class="SubHead" width="125"><dnn:label id="lbContent" runat="server" controlname="lblContent" suffix=":"></dnn:label></td>
        <td>
            <dnn:texteditor id="txtContent" runat="server" height="300" width="600" />
        </td>
    </tr>
    <tr  runat="server" id="trURL" >
        <td class="SubHead" nowrap width="217">
            <dnn:label ID="lbURL" runat="server" Suffix=":" ></dnn:label>
        </td>
        <td class="SubHead">
            <asp:TextBox ID="txtURL" runat="server" Width="304px" CssClass="NormalTextBox"></asp:TextBox>
        </td>
    </tr>
    <tr runat="server" id="trEmbedResource" >
        <td class="SubHead" valign="top" width="217">
            <dnn:label ID="lbEmbedResource" runat="server" Suffix=":">
            </dnn:label>
        </td>
        <td class="SubHead">
            <asp:TextBox ID="txtEmbedResource" runat="server" Width="304px" TextMode="MultiLine" CssClass="NormalTextBox"
                         Height="144px"></asp:TextBox></td>
    </tr>
    <tr runat="server" id="trModuleTab" >
        <td class="SubHead" valign="top" width="217">
            <dnn:label ID="lbTab" runat="server" Suffix=":" >
            </dnn:label>
        </td>
        <td class="SubHead">
            <asp:DropDownList ID="cboTabs" runat="server" AutoPostBack="true" OnSelectedIndexChanged="cboTabs_SelectedIndexChanged">   </asp:DropDownList>
        </td>
    </tr>
    <tr runat="server" id="trModule" >
        <td class="SubHead" valign="top" width="217">
            <dnn:label ID="lbModule" runat="server" Suffix=":" >
            </dnn:label>
        </td>
        <td class="SubHead">
            <asp:DropDownList ID="cboModule" runat="server">   </asp:DropDownList>
        </td>
    </tr>
    
    <tr>
        <td class="SubHead" nowrap width="217">
            <dnn:label ID="lbTitle" runat="server" Suffix=":" ></dnn:label>
        </td>
        <td class="SubHead">
            <asp:TextBox ID="txtTitle" runat="server" Width="304px" CssClass="NormalTextBox"></asp:TextBox>
        </td>
    </tr>
    
    <tr id="trThumb" runat="server">
        <td class="SubHead" width="160" height="35">
            <dnn:label ID="lbThumbURL" Suffix=":" ControlName="ctlThumbURL" runat="server"></dnn:label>
        </td>
        <td width="365" height="35">
            <portal:url id="ctlThumbURL" runat="server" width="250" showtabs="False" showurls="False" showtrack="False"
                        showlog="False"  Required="False"  showupload="True"  shownewwindow="False" />
        </td>
    </tr>
    
    <tr>
        <td class="SubHead" valign="top" nowrap width="217">
            <dnn:label ID="lbDescription" runat="server" Suffix=":" >
            </dnn:label>
        </td>
        <td class="SubHead">
            <asp:TextBox ID="txtDescription" runat="server" Width="304px" TextMode="MultiLine" CssClass="NormalTextBox"
                         Height="144px"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="SubHead" nowrap>
            <dnn:label ID="lbViewOrder" runat="server"  Suffix=":"></dnn:label>
        </td>
        <td class="SubHead">
            <asp:TextBox ID="txtViewOrder" runat="server" Width="128px" CssClass="NormalTextBox"></asp:TextBox>
            <asp:CompareValidator ID="CompareValidator3" runat="server" CssClass="NormalRed"
                                  ErrorMessage="Invalid Integer" Operator="DataTypeCheck" ControlToValidate="txtViewOrder"
                                  Type="Integer" Display="Dynamic"></asp:CompareValidator></td>
    </tr>
   
    <tr id="trWidth" runat="server">
        <td class="SubHead" width="160">
            <dnn:label ID="lbMediaWidth"  Suffix=":" runat="server"></dnn:label>
        </td>
        <td width="365">
            <asp:TextBox ID="txtMediaWidth" runat="server" Width="128px" CssClass="NormalTextBox"></asp:TextBox>
            <asp:CompareValidator ID="CompareValidator2" runat="server" CssClass="NormalRed"
                                  Display="Dynamic" Type="Integer" ControlToValidate="txtMediaWidth" Operator="DataTypeCheck"
                                  ErrorMessage="Invalid Integer"></asp:CompareValidator></td>
    </tr>
    <tr id="trHeight" runat="Server">
        <td class="SubHead" width="160">
            <dnn:label ID="lbMediaHeight" ControlName="ctlMediaURL" Suffix=":" runat="server"></dnn:label>
        </td>
        <td width="365">
            <asp:TextBox ID="txtMediaHeight" runat="server" Width="128px" CssClass="NormalTextBox"></asp:TextBox>
            <asp:CompareValidator ID="CompareValidator4" runat="server" CssClass="NormalRed"
                                  Display="Dynamic" Type="Integer" ControlToValidate="txtMediaHeight" Operator="DataTypeCheck"
                                  ErrorMessage="Invalid Integer"></asp:CompareValidator></td>
    </tr>
    
</table>
<p>
    <asp:linkbutton cssclass="CommandButton" id="cmdUpdate" resourcekey="cmdUpdate" 
                    runat="server" borderstyle="none" text="Update" onclick="cmdUpdate_Click"></asp:linkbutton>&nbsp;
    <asp:linkbutton cssclass="CommandButton" id="cmdCancel" resourcekey="cmdCancel" 
                    runat="server" borderstyle="none" text="Cancel" causesvalidation="False" 
                    onclick="cmdCancel_Click"></asp:linkbutton>&nbsp;
    <asp:linkbutton cssclass="CommandButton" id="cmdDelete" resourcekey="cmdDelete" 
                    runat="server" borderstyle="none" text="Delete" causesvalidation="False" 
                    onclick="cmdDelete_Click"></asp:linkbutton>&nbsp;
</p>