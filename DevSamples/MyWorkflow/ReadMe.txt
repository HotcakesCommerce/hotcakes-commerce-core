Here is the short version. We highly recommend you check out our documentation on hotcakes.org, though. 
We spent a lot of time on it. It would make us all feel really good about it - also we think it would help you. 

1) Create a class inherited from DnnWorkflowFactory
2) Create new task classes that inherit from the Task or OrderTask classes
3) Override the method in your new Workflow class that creates a list of tasks and insert your new tasks in it
4) We provide our default workflow. You can uncomment any section to have an override available to you with our default implementation
All you have to do then is insert your new task in any position between the default existing ones
5) Once you've done your custom work, you can deploy your compiled library to the production website's bin folder
6) Once that's done, you can select your new workflow on the Admin -> Extensibility page in the Admin UI

Documentation:  https://hotcakescommerce.zendesk.com/hc/en-us/articles/204725929--Example-Custom-Order-Workflow-Solution-