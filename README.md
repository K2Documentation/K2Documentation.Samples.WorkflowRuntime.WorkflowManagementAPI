# K2Documentation.Samples.WorkflowRuntime.WorkflowManagementAPI
Sample code that demonstrates how to perform workflow administration tasks programmatically. 

This project contains sample code that demonstrates how to perform workflow administration tasks programmatically, such as:  
* connecting to the K2 workflow server's management interface
* retrieving a list of workflwo errors
* repairing workflow errors
* setting workflwo permissions
* managing worklist items such as redirectign and releasing tasks
* using LIve Instance Management to migrate a running workflow from one process definition version to another
* managing active process instances to perform operations such as GoToActivity, Stop and Resume

Find more information about this API in the K2 Developers Reference at
https://help.k2.com/onlinehelp/k2five/DevRef/current/default.htm#Runtime/WF-Manage/WFManage-Intro.htm

## Prerequisites
The sample code has the following dependencies: 
* .NET Assemblies (both assemblies are included with K2 client-side tools install and are also included in the project's References folder)
  * SourceCode.Workflow.Management.dll 
  * SourceCode.HostClientAPI.dll 

## Getting started
* Use these code snippets to learn how to perform common administration tasks with K2 workflows through the workflow management API. 
* Note that these projects may compile, but will not actually run as-is, since they are intended as sample code only. You will need to edit the code snippets to work in your environment and with your artifacts. 
* Fetch or Pull the appropriate branch of this project for your version of K2. 
* The Master branch is considered the latest, up-to-date version of the sample project. Older versions will be branched. For example, there may be a branch called K2-Five-5.0 that is specific to K2 Five version 5.0. There may be another branch called K2-Five-5.1 that is specific to K2 Five version 5.3. Assume that the master branch is configured for the latest release version of K2 Five. 
* The Visual Studio project contains a folder called "References" where you can find the referenced .NET assemblies or other uncommon assemblies. By default, try to reference these assemblies from your own environment for consistency, but we provide the referenced assemblies as a convenience in case you are not able to locate or use the referenced assemblies in your environment. 
* The Visual Studio project contains a folder called "Resources". This folder contains addiitonal resources that may be required to use the same code, such as K2 deployment packages, sample files, SQL scripts and so on. 
   
## License
This project is licensed under the MIT license, which can be found in LICENSE.

## Notes
 * The sample code is provided as-is without warranty.
 * These sample code projects are not supported by K2 product support. 
 * The sample code is not necessarily comprehensive for all operations, features or functionality. 
 * We only accept code contributions that are compatible with the MIT license (essentially, MIT and Public Domain).
