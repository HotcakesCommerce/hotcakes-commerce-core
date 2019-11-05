Here is the short version. We highly recommend you check out our documentation on hotcakes.org, though. 
We spent a lot of time on it. It would make us all feel really good about it - also we think it would help you. 

In order to get started:
1) Create a class that implements ICartIntegration, ICheckoutIntegration or the IProductIntegration interface
2) Your class can have any custom logic and should return the IntegrationResult object
3) Deploy your compiled library to your website's bin folder
4) Tell our software to use your custom workflow by selecting it on the Admin -> Extensibility page in the Admin UI

Documentation: https://hotcakescommerce.zendesk.com/hc/en-us/articles/204725949-Example-Action-Delegate-Pipeline-Integration-Project