<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ValueWithLabel.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.ExtraFields.ValueWithLabel" %>

<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<div class="dnnFormItem">
     <dnn:Label ID="lbTitle" runat="server" ControlName="lbTitle" />
    <asp:PlaceHolder ID="pl" runat="server"></asp:PlaceHolder>
</div>