<%@ Control AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.UI.Containers.Container" %>
<%@ Register TagPrefix="dnn" TagName="TITLE" Src="~/Admin/Containers/Title.ascx" %>
<div class="Container007">
	<div class="dnntitle">
	  <div class="title-bg BackgroundColor">
	    <dnn:TITLE runat="server" id="dnnTITLE1" CSSClass="title" />
	  </div>
    </div>
	<div class="contentmain"> 
		<div class="contentpane" id="ContentPane" runat="server"></div>
		<div class="c_footer">
		</div>
	</div>
</div>








































