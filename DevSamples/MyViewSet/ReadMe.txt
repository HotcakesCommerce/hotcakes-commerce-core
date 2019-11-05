Here is the short version. We highly recommend you check out our documentation on hotcakes.org, though. 
We spent a lot of time on it. It would make us all feel really good about it - also we think it would help you. 

BEFORE YOU GET STARTED - Do not edit the default views. If you do, you will lose your work the next time you upgrade. 

1) Copy the project files over your existing local dev environment's CMS website folder
2) Open the MyCompany.MyViewSet Visual Studio project file
3) Update the the web.config with the strongly-typed Razor section as outlined in the web.config.readme.txt file. 
4) Build your project. This will give you intellisense during your development.
5) Use the custom Razor file as an example for custom views here: Portals\_default\HotcakesViews\MyViewSet\Views\MyCustomView\
6) Create your own custom Controller using this file as an example: MyCustomViewController.cs
7) Add custom views to your Hotcakes views using this format: @Html.Action("Index", "MyCustomView", new { anyParameter = "Any Value Can Be Passed Here - I would pass the Model, though. Not this unusable string..."})
(We integrated a custom partial view here for you to use as an example: Products\Index.cshtml)
8) For your own good, use a qualifier to keep name of your custom controllers unique. Otherwise there might be some conflicts and that would be a pain for you to troubleshoot. 

Extra Steps For Visual Studio 2015 (Get MVC Intelligence)
1) Open the MyCompany.MyViewSet Visual Studio project file in VS 2015
2) Right click on project file and open Properties
3) Select reference Path
4) Add reference path $\Website\Reference\ folder
5) Update the the web.config with the strongly-typed Razor section as outlined in the web.config.readme.txt file for visual studio 2015. 

Documentation:  https://hotcakescommerce.zendesk.com/hc/en-us/articles/205426325-Creating-Custom-Viewsets