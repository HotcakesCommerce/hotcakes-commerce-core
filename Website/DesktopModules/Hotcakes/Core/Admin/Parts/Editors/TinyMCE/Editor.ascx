<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Parts.Editors.TinyMCE.Editor" Codebehind="Editor.ascx.cs" %>
<script src="/DesktopModules/Hotcakes/Core/Scripts/tinymce/jquery.tinymce.js" type="text/javascript"></script>
<script src="/DesktopModules/Hotcakes/Core/Scripts/tinymce/init.js" type="text/javascript"></script>
<asp:TextBox ID="EditorField" CssClass="tinymce"  runat="server" Height="120px" TextMode="MultiLine"
    Width="300px" Wrap="False" CausesValidation="false"></asp:TextBox>