using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//SourceCode.Workflow.Management is used to administer workflows and workflow-related settings on a K2 Server
//this assembly can be added from the file system at
//C:\Program Files (x86)\K2 blackpearl\Bin\SourceCode.Workflow.Management.dll
using SourceCode.Workflow.Management;
using SourceCode.Workflow.Management.Criteria;
//SourceCode.Hosting.Client is used to construct connection strings 
//this assembly can be added from the .NET references tab as SourceCode.HostClientAPI or from the file system at
//C:\Program Files (x86)\K2 blackpearl\Bin\SourceCode.HostClientAPI.dll
using SourceCode.Hosting.Client;


namespace K2Documentation.Samples.WorkflowRuntime.WorkflowManagementAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            //no implementation. This code will not run, it is only intended as sample code
        }

        
        //sample that demonstrates opening a connection to the K2 server using the management API
        public void OpenManagementConnection()
        {
            WorkflowManagementServer K2Mgmt = new WorkflowManagementServer();
            //construct a connection string with the hosting client API
            SourceCode.Hosting.Client.BaseAPI.SCConnectionStringBuilder builder =
                    new SourceCode.Hosting.Client.BaseAPI.SCConnectionStringBuilder();
            builder.Integrated = true; //use the current user's security credentials
            builder.IsPrimaryLogin = true;
            builder.Authenticate = true;
            builder.EncryptedPassword = false;
            builder.Host = "localhost";
            builder.Port = 5555; //you must use port 5555 when connecting with the management API
            builder.SecurityLabelName = "K2"; //this sample uses the Active Directory security provider

            //open the connection using the constructed connection string
            K2Mgmt.Open(builder.ToString());       

            //do something in the management connection

            //close the connection when you are done
            K2Mgmt.Connection.Close();

            
        }

        //sample that shows how to use Out-of-Office in the workflow management API
        //in this example, we just want to output all users that are currently out-of-office
        public void ListOutOfOfficeUsers()
        {
            //establish the connection
            WorkflowManagementServer K2Mgmt = new WorkflowManagementServer();
            K2Mgmt.CreateConnection();
            K2Mgmt.Connection.Open("connectionstring");
            SourceCode.Workflow.Management.OOF.Users OOFUsers = K2Mgmt.GetUsers(ShareType.OOF);
            foreach (SourceCode.Workflow.Management.OOF.User OOFUser in OOFUsers)
            {
                WorklistShares wlss = K2Mgmt.GetCurrentSharingSettings(OOFUser.FQN, ShareType.OOF);
                foreach (WorklistShare wls in wlss)
                {
                    //do something with each "share" record
                    Console.WriteLine("User Name: " + OOFUser.FQN);
                    Console.WriteLine("Start Date: " + wls.StartDate.ToString());
                    Console.WriteLine("End Date: " + wls.EndDate.ToString());
                }
            }

            //close the connection
            K2Mgmt.Connection.Close();
        }

        //sample that shows how to set workflow permissions
        //in this sample, we are giving a dummy user account rights to a dummy process
        public void SetWorkflowPermissions()
        {
            //establish the connection
            WorkflowManagementServer K2Mgmt = new WorkflowManagementServer();
            K2Mgmt.CreateConnection();
            K2Mgmt.Connection.Open("connectionstring");

            //get the proc set ID of the workflow whose permissions we need to set
            SourceCode.Workflow.Management.Criteria.ProcessCriteriaFilter filter = new SourceCode.Workflow.Management.Criteria.ProcessCriteriaFilter();
            Processes procs = null;
            filter.AddRegularFilter(ProcessFields.ProcessFullName, Comparison.Equals, "processFullName");
            procs = K2Mgmt.GetProcesses(filter);
            int procSetId = procs[0].ProcSetID;

            //now that we have the proc set ID, we can apply permissions
            //build up a collection of permissions to apply to the workflow
            Permissions permissions = new Permissions();
            //create a ProcSetPermission object - this object will be added to the Permissions collection
            ProcSetPermissions procPermissions = new ProcSetPermissions();
            procPermissions.ProcSetID = procSetId;
            procPermissions.UserName = @"domain\username"; //could also be ProcPermissions.GroupName for a group
            procPermissions.Admin = true;
            procPermissions.Start = true;
            procPermissions.View = true;
            procPermissions.ViewPart = false;
            procPermissions.ServerEvent = false;
            //add the permission to the permissions collection
            permissions.Add(procPermissions);
            procPermissions = null;

            //now we can apply the updated set of permissions to the process
            K2Mgmt.UpdateProcPermissions(procSetId, permissions);

            //several other methods are also available, listed below. Check product documentation for more information
            //K2Mgmt.UpdateGroupPermissions();
            //K2Mgmt.UpdateOrAddProcUserPermissions();
            //K2Mgmt.UpdateProcGroupPermissions();
            //K2Mgmt.UpdateProcUserPermissions();

            //close the connection
            K2Mgmt.Connection.Close();
        }

        //sample that shows how to list workflows in error state
        //in this sample, we just want to output all workflows that are in error state
        public void ListErrors()
        {
            //establish the connection
            WorkflowManagementServer K2Mgmt = new WorkflowManagementServer();
            K2Mgmt.CreateConnection();
            K2Mgmt.Connection.Open("connectionstring");

            //first get the error profile ID., In this case, we will use the default "All" profile
            int errorProfileId = K2Mgmt.GetErrorProfile("All").ID;

            ErrorLogs K2Errors = K2Mgmt.GetErrorLogs(errorProfileId); //you can also construct a criteria filter to filter the error profile further.
            
            foreach (ErrorLog K2Error in K2Errors)
            {
                //Do something with the error log entry
                Console.WriteLine(K2Error.Description);
            }

            //close the connection
            K2Mgmt.Connection.Close();
        }

        //sample that shows how to repair error on a K2 server
        //in this sample, we want to attempt the "Retry" statement on all workflows currently in Error state
        //be careful doing this on a server with many errored process instances, since executing more than about 20 Retry statements in a very short interval can cause 
        //the K2 server to slow down significantly
        public void RepairErrors()
        {
            //establish the connection
            WorkflowManagementServer K2Mgmt = new WorkflowManagementServer();
            K2Mgmt.CreateConnection();
            K2Mgmt.Connection.Open("connectionstring");

            //first get the error profile ID., In this case, we will use the default "All" profile
            int errorProfileId = K2Mgmt.GetErrorProfile("All").ID;

            ErrorLogs K2Errors = K2Mgmt.GetErrorLogs(errorProfileId); //you can also construct a criteria filter to filter the error profile further.

            foreach (ErrorLog K2Error in K2Errors)
            {
                //Do something with the error log entry
                K2Mgmt.RetryError(K2Error.ProcInstID, K2Error.ID, @"domain\username");
            }

            //close the connection
            K2Mgmt.Connection.Close();
        }

        //sample that shows how to manage worklist items.
        //In this sample, we want to redirect all tasks from one user to another user
        public void ManageWorklistItems()
        {
            //establish the connection
            WorkflowManagementServer K2Mgmt = new WorkflowManagementServer();
            K2Mgmt.CreateConnection();
            K2Mgmt.Connection.Open("connectionstring");

            //build up a filter for the list of worklist items. Here, we want to return all the worklist items for a specific user
            WorklistCriteriaFilter wlCritFilter = new WorklistCriteriaFilter();
            wlCritFilter.AddRegularFilter(WorklistFields.Destination, Comparison.Like, "%user1%");
            WorklistItems wlItems = K2Mgmt.GetWorklistItems(wlCritFilter);
            foreach (WorklistItem wlItem in wlItems)
            {
                K2Mgmt.RedirectWorklistItem("user1","user2",wlItem.ProcInstID, wlItem.ActInstDestID, wlItem.ID);
            }

            //close the connection
            K2Mgmt.Connection.Close();
        }

        //sample that shows how to manage process instances
        //in this sample, we want to stop all instances of a particular workflow
        public void ManageProcessInstances()
        {
            //establish the connection
            WorkflowManagementServer K2Mgmt = new WorkflowManagementServer();
            K2Mgmt.CreateConnection();
            K2Mgmt.Connection.Open("connectionstring");

            //first, get the ID's of all the  process instances of the workflow
            ProcessInstances procInstances = K2Mgmt.GetProcessInstancesAll("workflowfullname", "", ""); //leaving parameters blank effectively ignores that parameter
            foreach (ProcessInstance procInst in procInstances)
            {
                if (procInst.Status == "Active")
                {
                    K2Mgmt.StopProcessInstances(procInst.ID);
                }
            }

            //close the connection
            K2Mgmt.Connection.Close();
        }

        //sample that shows how to perform Live INstance Management
        //in this example, we want to migrate all active instances of a workflow to the latest version
        //Note: take due care when migrating active process instances since not all migration scenarios are supported
        public void LiveInstanceManagementSample()
        {
            //establish the connection
            WorkflowManagementServer K2Mgmt = new WorkflowManagementServer();
            K2Mgmt.CreateConnection();
            K2Mgmt.Connection.Open("connectionstring");

            //get the proc set ID of the workflow so that we can get the latest version of the proc set
            SourceCode.Workflow.Management.Criteria.ProcessCriteriaFilter filter = new SourceCode.Workflow.Management.Criteria.ProcessCriteriaFilter();
            Processes procs = null;
            filter.AddRegularFilter(ProcessFields.ProcessFullName, Comparison.Equals, "processFullName");
            procs = K2Mgmt.GetProcesses(filter);
            int procSetId = procs[0].ProcSetID;

            //now get the latest version of the procset
            int latestVersion = 0;
            int latestVersionId = 0;
            Processes procVersions = K2Mgmt.GetProcessVersions(procSetId);
            foreach (Process procVersion in procVersions)
            {
                if (procVersion.VersionNumber > latestVersion)
                {
                    latestVersion = procVersion.VersionNumber;
                    latestVersionId = procVersion.ProcID;
                }
            }

            //now that we have the latest version of the workflow, 
            //get the ID's of all the  process instances of the workflow
            ProcessInstances procInstances = K2Mgmt.GetProcessInstancesAll("processFullName", "", ""); //leaving parameters blank effectively ignores that parameter
            foreach (ProcessInstance procInst in procInstances)
            {
                //no need to migrate ones that are already on this version
                if (procInst.ExecutingProcID != latestVersionId)
                {
                    //must stop a non-stopped instance before attempting migration
                    if (procInst.Status != "Stopped")
                    {
                        K2Mgmt.StopProcessInstances(procInst.ID);
                    }
                    //now migrate the instance to the latest version
                    K2Mgmt.SetProcessInstanceVersion(procInst.ID, latestVersion);
                    //restart the process 
                    K2Mgmt.StartProcessInstances(procInst.ID);
                }
            }

            //close the connection
            K2Mgmt.Connection.Close();
        }


    }
}
