<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="AdminControlBar.ascx.cs" Inherits="Hotcakes.Modules.ControlPanel.AdminControlBar" %>

<%@ Import namespace="System.Data" %>
<%@ Import Namespace="DotNetNuke.Security.Permissions" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<%@ Register Src="/DesktopModules/Hotcakes/Core/Admin/Controls/Languages.ascx" TagPrefix="hcc" TagName="Languages" %>

<div id="ControlBar_ControlPanel">
    <asp:Panel ID="ControlPanel" runat="server">
	    <div id="ControlBar">
		    <div class="ControlContainer">
			    <div class="ServiceIcon">
				    <asp:Image ID="conrolbar_logo" runat="server" AlternateText="HCClogo" ViewStateMode="Disabled" />
				    <asp:Image ID="updateService" runat="server" AlternateText="" ViewStateMode="Disabled" />
			    </div>
			    <% if (UserController.Instance.GetCurrentUserInfo().IsInRole(PortalSettings.AdministratorRoleName)) {%>
				   <ul id="ControlNav">
                   
                        <li class="controlBar_ArrowMenu">
                            <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/default.aspx"><%= GetString("Dashboard") %></a>
                        </li>

                        <li class="controlBar_ArrowMenu">
                            <a style="color: white !important;" href="javascript:void(0);"> <%= GetString("Catalog") %></a>
                            <div class="subNav" style="display: none;">
                                <dl>
                                    <dd>
                                        <ul>
                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/catalog/default.aspx"><%= GetString("Products") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/catalog/Categories.aspx"><%= GetString("Categories") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/content/Columns.aspx"><%= GetString("Content Columns") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/catalog/ProductSharedChoices.aspx"><%= GetString("Shared Choices") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/catalog/ProductTypes.aspx"><%= GetString("Product Types") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/catalog/ProductTypeProperties.aspx"><%= GetString("Product Type Properties") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/catalog/MembershipProductTypes.aspx"><%= GetString("Membership Product Types") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/catalog/ReviewsToModerate.aspx"><%= GetString("Reviews") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/catalog/FileVault.aspx"><%= GetString("File Vault") %></a>
                                            </li>
                                        </ul>
                                    </dd>
                                </dl>
                            </div>
                        </li>

                        <li class="controlBar_ArrowMenu">
                            <a style="color: white !important;" href="javascript:void(0);"><%= GetString("Marketing") %></a>
                            <div class="subNav" style="display: none;">
                                <dl>
                                    <dd>
                                        <ul>
                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/marketing/promotions.aspx"><%= GetString("Promotion") %> </a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/marketing/rewardspoints.aspx"><%= GetString("Reward Points") %> </a>
                                            </li>
                                        </ul>
                                    </dd>
                                </dl>
                            </div>
                        </li>

                        <li class="controlBar_ArrowMenu">
                            <a style="color: white !important;" href="javascript:void(0);"><%= GetString("People") %></a>
                            <div class="subNav" style="display: none;">
                                <dl>
                                    <dd>
                                        <ul>
                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/people/default.aspx"><%= GetString("Customers") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/people/PriceGroups.aspx"><%= GetString("Price Groups") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/people/manufacturers.aspx"><%= GetString("Manufacturers") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/people/vendors.aspx"><%= GetString("Vendors") %></a>
                                            </li>
                                        </ul>
                                    </dd>
                                </dl>
                            </div>
                        </li>

                        <li class="controlBar_ArrowMenu">
                            <a style="color: white !important;" href="javascript:void(0);"><%= GetString("Orders") %></a>
                            <div class="subNav" style="display: none;">
                                <dl>
                                    <dd>
                                        <ul>
                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/orders/default.aspx?p=1&amp;mode=0"><%= GetString("Order Manager") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/orders/default.aspx?p=1&amp;mode=0">
                                                    <hr>
                                                </a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/orders/createorder.aspx"><%= GetString("Add Order") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/orders/default.aspx?p=1&amp;mode=0">
                                                    <hr>
                                                </a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/orders/default.aspx?p=1&amp;mode=1"><%= GetString("New") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/orders/default.aspx?p=1&amp;mode=5"><%= GetString("On Hold") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/orders/default.aspx?p=1&amp;mode=2"><%= GetString("Ready for Payment") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/orders/default.aspx?p=1&amp;mode=3"><%= GetString("Ready for Shipping") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/orders/default.aspx?p=1&amp;mode=4"><%= GetString("Completed") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/orders/default.aspx?p=1&amp;mode=6"><%= GetString("Cancelled") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/orders/default.aspx?p=1&amp;mode=0"><%= GetString("All Orders") %></a>
                                            </li>
                                        </ul>
                                    </dd>
                                </dl>
                            </div>
                        </li>

                        <li class="controlBar_ArrowMenu">
                            <a style="color: white !important;" href="javascript:void(0);"><%= GetString("Reports") %></a>
                            <div class="subNav" style="display: none;">
                                <dl>
                                    <dd>
                                        <ul>
                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/reports/Summary Report/view.aspx"><%= GetString("Summary Report") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/reports/Daily Sales/view.aspx"><%= GetString("Transactions by Day") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/reports/Daily Product Sales/view.aspx"><%= GetString("Product Sales") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/reports/Sales By Date/view.aspx"><%= GetString("Orders by Date") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/reports/Daily CC Activity/view.aspx"><%= GetString("Credit Card Activity") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/people/affiliates.aspx"><%= GetString("Affiliate Report") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/people/affiliatepayments.aspx"><%= GetString("Affiliate Payments") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/reports/Sales By Coupon/view.aspx"><%= GetString("Sales by Coupon") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/reports/Sales Tax Report/view.aspx"><%= GetString("Sales Tax Report") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/reports/Top Products/view.aspx"><%= GetString("Top Products") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/reports/AbandonedCarts/view.aspx"><%= GetString("Abandoned Carts") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/catalog/GiftCards.aspx"><%= GetString("Gift Cards") %></a>
                                            </li>
                                        </ul>
                                    </dd>
                                </dl>
                            </div>
                        </li>

                        <li class="controlBar_ArrowMenu">
                            <a style="color: white !important;" href="javascript:void(0);"><%= GetString("Admin") %></a>
                            <div class="subNav" style="display: none;">
                                <dl>
                                    <dd>
                                        <ul>
                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/configuration/Affiliates.aspx"><%= GetString("Affiliates") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/configuration/Analytics.aspx"><%= GetString("Analytics") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/configuration/Api.aspx"><%= GetString("API") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/configuration/AddressValidation.aspx"><%= GetString("Address Validation") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/content/EmailTemplates.aspx"><%= GetString("E-Mail Templates") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/configuration/Fraud.aspx"><%= GetString("Fraud Screening") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/configuration/Orders.aspx"><%= GetString("Orders") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/configuration/Shipping_Methods.aspx"><%= GetString("Shipping Methods") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/configuration/Shipping_Zones.aspx"><%= GetString("Shipping Zones") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/configuration/ShippingHandling.aspx"><%= GetString("Shipping and Handling") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/configuration/Extensibility.aspx"><%= GetString("Extensibility") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/configuration/ViewsManager.aspx"><%= GetString("Views Manager") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/configuration/RoleAdministration.aspx"><%= GetString("Role Administration") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/configuration/GiftCardConfiguration.aspx"><%= GetString("Gift Card Settings") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/SetupWizard/SetupWizard.aspx?step=1"><%= GetString("Setup Wizard") %></a>
                                            </li>
                                        </ul>
                                    </dd>
                                </dl>
                            </div>
                        </li>

                        <li class="controlBar_ArrowMenu">
                            <a style="color: white !important;" href="javascript:void(0);"><%= GetString("Settings") %></a>
                            <div class="subNav" style="display: none;">
                                <dl>
                                    <dd>
                                        <ul>
                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/configuration/General.aspx"><%= GetString("Store Name and Logo") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/configuration/StoreInfo.aspx"><%= GetString("Store's Address") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/configuration/GeoLocation.aspx"><%= GetString("Geo Location") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/configuration/countries.aspx"><%= GetString("Countries") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/configuration/Email.aspx"><%= GetString("E-Mail Addresses") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/configuration/Payment.aspx"><%= GetString("Payment Methods") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/configuration/ProductReviews.aspx"><%= GetString("Product Reviews") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/configuration/SocialMedia.aspx"><%= GetString("Social Media") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/configuration/TaxClasses.aspx"><%= GetString("Taxes") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/configuration/Caching.aspx"><%= GetString("Caching") %></a>
                                            </li>

                                            <li>
                                                <a style="color: white !important;" href="/DesktopModules/Hotcakes/Core/Admin/configuration/PageConfiguration.aspx"><%= GetString("Page Configuration") %></a>
                                            </li>
                                        </ul>
                                    </dd>
                                </dl>
                            </div>
                        </li>
                        <li class="controlBar_ArrowMenu">
                            <a style="color: white !important;" href="https://hotcakes.org/community" target="_blank"><%= GetString("Help") %></a>
                        </li>
                    </ul>


				    <ul id="ControlNavQuickLink">
					    <li>
						    <a id="GoToStore" href="<%=Hotcakes.Commerce.Urls.HccUrlBuilder.RouteHccUrl(Hotcakes.Commerce.Urls.HccRoute.Home) %>"> <img src="/DesktopModules/Hotcakes/ControlPanel/controlbarimages/Store-icon.png"/> <%= GetString("ViewShop") %></a>
					    </li>
					    <li>
						    <hcc:Languages runat="server" ID="ucLanguages" />
					    </li>
					    <li>
						    <a runat="server" id="aHostAdmin"><%= GetString("superuser")%></a>
					    </li>
					    <li>
						    <a href="<%=Hotcakes.Commerce.Urls.HccUrlBuilder.RouteHccUrl(Hotcakes.Commerce.Urls.HccRoute.Logoff) %>"><%= GetString("LogOut") %></a>
					    </li>
				    </ul>
			
			    <%} %>
		    </div>
	    </div>
    </asp:Panel>
</div>
<script type="text/javascript">
	if (typeof dnn === 'undefined') dnn = {};
	dnn.controlBarSettings = {
		searchInputId: 'ControlBar_SearchModulesInput',
		makeCopyCheckboxId: 'ControlBar_Module_chkCopyModule',
    	yesText: '<%= DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(Localization.GetString("Yes.Text", Localization.SharedResourceFile)) %>',
		noText: '<%= DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(Localization.GetString("No.Text", Localization.SharedResourceFile)) %>',
		titleText: '<%= DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(Localization.GetString("Confirm.Text", Localization.SharedResourceFile)) %>',
		defaultCategoryValue: 'All',

		loadingModulesId: 'ControlBar_ModuleListWaiter_LoadingMessage'
	};

</script>
