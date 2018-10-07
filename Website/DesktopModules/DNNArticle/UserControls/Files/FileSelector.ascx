<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FileSelector.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.UserControls.Files.FileSelector" %>


<%@Register TagPrefix="imm" Tagname="SelectImage" Src="~/DesktopModules/ZLDNNFileSearch/Controls/SelectImage.ascx" %>
<imm:selectimage runat="server" MaxSize="100" ID="mySelectImage" />
