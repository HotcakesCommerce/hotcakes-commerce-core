<%@ Control AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.UI.Containers.Container" %>
<%@ Register TagPrefix="dnn" TagName="TITLE" Src="~/Admin/Containers/Title.ascx" %>
<div class="Container015">
	<h2>
		<dnn:TITLE runat="server" id="dnnTITLE1" CSSClass="title1" />
    </h2>
	<div class="title-icon">
	  <em class="titleline"></em>
	  <em class="icon-flower"></em>
	  <em class="titleline"></em>
	</div>
	<div class="contentmain"> 
		<div class="contentpane" id="ContentPane" runat="server"></div>
		<div class="c_footer">
		</div>
	</div>
</div>















