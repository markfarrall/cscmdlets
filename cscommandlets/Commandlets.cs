﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;

namespace cscommandlets
{

    [Cmdlet(VerbsCommon.Open, "CSConnection")]
    public class OpenCSConnectionCommand : Cmdlet
    {

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)] 
        [ValidateNotNullOrEmpty]
        public String Username { get; set; }
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public String Password { get; set; }
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage="e.g. http://server.domain/cws/")]
        [ValidateNotNullOrEmpty]
        public String ServicesDirectory { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            try
            {
                Globals.Username = Username;
                Globals.Password = Password;
                Globals.ServicesDirectory = ServicesDirectory;

                Connection connection = new Connection();

                WriteObject("Connection established");
            }
            catch (Exception e)
            {
                ErrorRecord err = new ErrorRecord(e, "00001U", ErrorCategory.NotSpecified, this);
                ThrowTerminatingError(err);
                return;
            }
        }

    }

    [Cmdlet(VerbsCommon.Add, "CSProjectWorkspace")]
    public class AddCSProjectWorkspaceCommand : Cmdlet
    {

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public String Name { get; set; }
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public Int64 ParentID { get; set; }
        [Parameter(Mandatory=false)]
        public Int64 TemplateID { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            Int64 response;

            try
            {
                // create the connection object
                if (!Globals.ConnectionOpened)
                {
                    ThrowTerminatingError(Errors.ConnectionMissing(this));
                    return;
                }
                Connection connection = new Connection();

                // create the project workspace
                response = connection.CreateContainer(Name, ParentID, Connection.ObjectType.Project);

                // if we've got a template ID then copy the config
                if (TemplateID > 0 && Convert.ToInt64(response) > 0)
                {
                    connection.UpdateProjectFromTemplate(Convert.ToInt64(response), TemplateID);
                }

                // write the output
                WriteObject(response);
            }
            catch (Exception e)
            {
                ErrorRecord err = new ErrorRecord(e, "00002U", ErrorCategory.NotSpecified, this);
                ThrowTerminatingError(err);
                return;
            }
        }

    }

    [Cmdlet(VerbsCommon.Add, "CSFolder")]
    public class AddCSFolderCommand : Cmdlet
    {

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public String Name { get; set; }
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public Int64 ParentID { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            Int64 response;

            try
            {

                // create the connection object
                if (!Globals.ConnectionOpened)
                {
                    ThrowTerminatingError(Errors.ConnectionMissing(this));
                    return;
                }
                Connection connection = new Connection();

                // create the folder
                response = connection.CreateContainer(Name, ParentID, Connection.ObjectType.Folder);

                // write the output
                WriteObject(response);
            }
            catch (Exception e)
            {
                ErrorRecord err = new ErrorRecord(e, "00003U", ErrorCategory.NotSpecified, this);
                ThrowTerminatingError(err);
            }

        }

    }

    [Cmdlet(VerbsCommon.Add, "CSUser")]
    public class AddCSUserCommand : Cmdlet
    {

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public String Login { get; set; }
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public Int64 DepartmentGroupID { get; set; }
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public String Password { get; set; }
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public String FirstName { get; set; }
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public String MiddleName { get; set; }
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public String LastName { get; set; }
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public String Email { get; set; }
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public String Fax { get; set; }
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public String OfficeLocation { get; set; }
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public String Phone { get; set; }
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public String Title { get; set; }
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public Boolean? LoginEnabled { get; set; }
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public Boolean? PublicAccessEnabled { get; set; }
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public Boolean? CreateUpdateUsers { get; set; }
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public Boolean? CreateUpdateGroups { get; set; }
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public Boolean? CanAdministerUsers { get; set; }
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public Boolean? CanAdministerSystem { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            Int64 response;

            try
            {

                // create the connection object
                if (!Globals.ConnectionOpened)
                {
                    ThrowTerminatingError(Errors.ConnectionMissing(this));
                    return;
                }
                Connection connection = new Connection();

                // set the privileges to default if null
                Boolean blnLoginEnabled = LoginEnabled ?? true;
                Boolean blnPublicAccessEnabled = PublicAccessEnabled ?? true;
                Boolean blnCreateUpdateUsers = CreateUpdateUsers ?? false;
                Boolean blnCreateUpdateGroups = CreateUpdateGroups ?? false;
                Boolean blnCanAdministerUsers = CanAdministerUsers ?? false;
                Boolean blnCanAdministerSystem = CanAdministerSystem ?? false;

                // create the user
                response = connection.CreateUser(Login, DepartmentGroupID, Password, FirstName, MiddleName, LastName, Email, Fax, OfficeLocation, 
                    Phone, Title, blnLoginEnabled, blnPublicAccessEnabled, blnCreateUpdateUsers, blnCreateUpdateGroups, blnCanAdministerUsers, blnCanAdministerSystem);

                // write the output
                WriteObject(response);
            }
            catch (Exception e)
            {
                ErrorRecord err = new ErrorRecord(e, "00004U", ErrorCategory.NotSpecified, this);
                ThrowTerminatingError(err);
            }

        }

    }

    [Cmdlet(VerbsCommon.Remove, "CSUser")]
    public class RemoveCSUserCommand : Cmdlet
    {

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public Int64 UserID { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            String response;

            try
            {

                // create the connection object
                if (!Globals.ConnectionOpened)
                {
                    ThrowTerminatingError(Errors.ConnectionMissing(this));
                    return;
                }
                Connection connection = new Connection();

                // create the user
                response = connection.DeleteUser(UserID);

                // write the output
                WriteObject(response);
            }
            catch (Exception e)
            {
                ErrorRecord err = new ErrorRecord(e, "00005U", ErrorCategory.NotSpecified, this);
                ThrowTerminatingError(err);
            }

        }

    }

    [Cmdlet(VerbsCommon.Remove, "CSNode")]
    public class RemoveCSNode : Cmdlet
    {

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public Int64 NodeID { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            String response = "";

            try
            {

                // create the connection object
                if (!Globals.ConnectionOpened)
                {
                    ThrowTerminatingError(Errors.ConnectionMissing(this));
                    return;
                }
                Connection connection = new Connection();

                // create the folder
                response = connection.DeleteNode(NodeID);

                // write the output
                WriteObject(response);
            }
            catch (Exception e)
            {
                ErrorRecord err = new ErrorRecord(e, "00004U", ErrorCategory.NotSpecified, this);
                ThrowTerminatingError(err);
            }

        }

    }

    [Cmdlet(VerbsData.ConvertTo, "CGIEncryptedPassword")]
    public class ConvertToEncryptedPasswordCommand : Cmdlet
    {

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public String Password { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            try
            {
                WriteObject(Encryption.EncryptString(Password));
            }
            catch (Exception e)
            {
                ErrorRecord err = new ErrorRecord(e, "00005U", ErrorCategory.NotSpecified, this);
                ThrowTerminatingError(err);
            }
        }

    }

    internal class Errors
    {

        internal static ErrorRecord ConnectionMissing(Object Object)
        {
            String msg = "Connection has not been opened. Please open the connection first using 'Open-CSConnection'";
            Exception exception = new Exception(msg);
            ErrorRecord err = new ErrorRecord(exception, "11111", ErrorCategory.ResourceUnavailable, Object);
            return err;
        }

    }
}
