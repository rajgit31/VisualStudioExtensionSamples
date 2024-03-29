using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using System.Resources;
using System.Reflection;
using System.Globalization;
using System.Windows.Forms;

namespace IDEContextMenuTwoPopupItems
{
	/// <summary>The object for implementing an Add-in.</summary>
	/// <seealso class='IDTExtensibility2' />
    public class Connect : IDTExtensibility2, IDTCommandTarget
    {
        private const string m_NAME_COMMAND1 = "MyCommand1";
        private const string m_NAME_COMMAND2 = "MyCommand2";

        private EnvDTE.DTE m_dte;
        private EnvDTE.AddIn m_addIn;

        private CommandBarPopup m_commandBarPopup;
        private CommandBarControl m_commandBarControl1;
        private CommandBarControl m_commandBarControl2;

        public void OnConnection(object application, Extensibility.ext_ConnectMode connectMode,
           object addInInst, ref System.Array custom)
        {
            try
            {
                m_dte = (EnvDTE.DTE)application;
                m_addIn = (EnvDTE.AddIn)addInInst;

                switch (connectMode)
                {
                    case ext_ConnectMode.ext_cm_UISetup:

                        // Create commands in the UI Setup phase. This phase is called only once when the add-in is deployed.
                        CreateCommands();
                        break;

                    case ext_ConnectMode.ext_cm_AfterStartup:

                        InitializeAddIn();
                        break;

                    case ext_ConnectMode.ext_cm_Startup:

                        // Do nothing yet, wait until the IDE is fully initialized (OnStartupComplete will be called)
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void OnStartupComplete(ref System.Array custom)
        {
            InitializeAddIn();
        }

        private void CreateCommands()
        {
            object[] contextUIGuids = new object[] { };

            m_dte.Commands.AddNamedCommand(m_addIn, m_NAME_COMMAND1, "MyCommand 1", "MyCommand 1", true, 59,
               ref contextUIGuids, (int)vsCommandStatus.vsCommandStatusSupported);

            m_dte.Commands.AddNamedCommand(m_addIn, m_NAME_COMMAND2, "MyCommand 2", "MyCommand 2", true, 59,
               ref contextUIGuids, (int)vsCommandStatus.vsCommandStatusSupported);
        }

        private void InitializeAddIn()
        {
            CommandBarControl myCommandBarControl;
            CommandBar codeWindowCommandBar;
            Command myCommand1;
            Command myCommand2;
            CommandBars commandBars;

            // Retrieve commands created in the ext_cm_UISetup phase of the OnConnection method
            myCommand1 = m_dte.Commands.Item(m_addIn.ProgID + "." + m_NAME_COMMAND1, -1);
            myCommand2 = m_dte.Commands.Item(m_addIn.ProgID + "." + m_NAME_COMMAND2, -1);

            // Retrieve the context menu of code windows
            commandBars = (CommandBars)m_dte.CommandBars;
            codeWindowCommandBar = commandBars["Code Window"];

            // Add a popup command bar
            myCommandBarControl = codeWindowCommandBar.Controls.Add(MsoControlType.msoControlPopup,
               System.Type.Missing, System.Type.Missing, System.Type.Missing, System.Type.Missing);

            m_commandBarPopup = (CommandBarPopup)myCommandBarControl;

            // Change its caption
            m_commandBarPopup.Caption = "My popup";

            // Add controls to the popup command bar
            m_commandBarControl1 = (CommandBarControl)myCommand1.AddControl(m_commandBarPopup.CommandBar,
               m_commandBarPopup.Controls.Count + 1);

            m_commandBarControl2 = (CommandBarControl)myCommand2.AddControl(m_commandBarPopup.CommandBar,
               m_commandBarPopup.Controls.Count + 1);
        }

        public void OnBeginShutdown(ref System.Array custom)
        {
        }

        public void OnDisconnection(Extensibility.ext_DisconnectMode disconnectMode, ref System.Array custom)
        {
            try
            {
                if (m_commandBarPopup != null)
                {
                    m_commandBarPopup.Delete(true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void OnAddInsUpdate(ref System.Array custom)
        {
        }

        public void QueryStatus(string CmdName, vsCommandStatusTextWanted NeededText,
           ref vsCommandStatus StatusOption, ref object CommandText)
        {
            if (CmdName == m_addIn.ProgID + "." + m_NAME_COMMAND1)
            {
                StatusOption = vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
            }
            else if (CmdName == m_addIn.ProgID + "." + m_NAME_COMMAND2)
            {
                StatusOption = vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
            }
        }

        public void Exec(string CmdName, vsCommandExecOption ExecuteOption, ref object VariantIn,
           ref object VariantOut, ref bool Handled)
        {
            if (CmdName == m_addIn.ProgID + "." + m_NAME_COMMAND1)
            {
                MessageBox.Show("MyCommand1 executed");
            }
            else if (CmdName == m_addIn.ProgID + "." + m_NAME_COMMAND2)
            {
                MessageBox.Show("MyCommand2 executed");
            }
        }
    }
}