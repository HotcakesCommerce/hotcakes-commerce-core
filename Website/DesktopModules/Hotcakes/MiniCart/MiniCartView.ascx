<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MiniCartView.ascx.cs" Inherits="Hotcakes.Modules.MiniCart.MiniCartView" %>
<%@ Register TagPrefix="dnn" TagName="MiniCartSpa" Src="../MiniCartSkinObject/MiniCartView.ascx" %>


<dnn:MiniCartSpa runat="server" ViewName="<% ViewName %>" id="MiniCartSpa" />