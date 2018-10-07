<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TextControl.ascx.cs" Inherits="Mandeeps.DNN.Modules.LiveSlider.Controls.TextControl" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="label" Src="~/controls/LabelControl.ascx" %>
<style>
    .xEditor 
    {
        padding: 5px 15px 15px;   
    }
    .xEditor a.mbutton
    {
        font-family: Helvetica,Arial,sans-serif;
        padding: 5px 10px;
        background: #818181;
        filter: progid:DXImageTransform.Microsoft.gradient(startColorstr=#818181, endColorstr=#656565);
        -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorstr=#818181, endColorstr=#656565)";
        background: -webkit-gradient(linear, left top, left bottom, from(#818181), to(#656565));
        background: -moz-linear-gradient(center top , #818181 0%, #656565 100%) repeat scroll 0 0 transparent;
        border-color: #FFFFFF;
        border-radius: 3px;
        color: #FFFFFF;
        font-weight: bold;
        text-decoration: none;
        color: #fff;
        text-shadow: 0 1px 1px #000000;
        display: inline-block;
    }
    .xEditor a.mbutton:hover
    {
        background: #4E4E4E;
        color: #ffffff;
        text-decoration: none;
    }
    .xEditor .values 
    {
        background-color: #FFFFFF;
        border: 1px solid #DDDDDD;
        box-shadow: 0 1px 2px rgba(0, 0, 0, 0.07) inset;
        color: #333333;
        transition: border-color 0.05s ease-in-out 0s;
        padding: 3px 5px;
        box-sizing: border-box;
        -moz-box-sizing: border-box;
        -webkit-box-sizing: border-box;
        border-radius: 3px;
    }
</style>
<div class="xEditor">    
<asp:TextBox ID="tbEditor" CssClass="values" runat="server"></asp:TextBox>
<br /><br />
<asp:LinkButton ID="btnOK" class="mbutton" runat="server" Text="OK"></asp:LinkButton>
</div>

