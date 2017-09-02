using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace SCTechUtilities
{
    public partial class ActivationForm : Form
    {
        private string activationKey = "";
        string webserviceURL = @"http://electrodatallc.com/DesktopModules/IWeb/webservice.asmx";
        //string webserviceURL = @"http://webservice.sctechllc.com/sctechwebservice.asmx";
        int userID = 0;

        public string ActivationKey
        {
            get { return activationKey; }
        }

        public ActivationForm()
        {
            InitializeComponent();
        }

        private void btnActivate_Click(object sender, EventArgs e)
        {
            try
            {
                btnActivate.Enabled = false;

                if (radioButton1.Checked)//they are logging in
                {
                    if (txtUsername.Text.Trim().Length > 0 && txtPassword.Text.Trim().Length > 0)
                    {
                        if (!dnnLogin(txtUsername.Text.Trim(), txtPassword.Text.Trim()))
                        {
                            MessageBox.Show("Invalid Username or Password");
                            btnActivate.Enabled = true;

                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Username and Password are required");
                        btnActivate.Enabled = true;

                        return;
                    }
                }
                else //they are registering
                {
                    if (txtUsername.Text.Trim().Length > 0 && txtPassword.Text.Trim().Length > 0 && txtEmail.Text.Trim().Length > 0)
                    {
                        if (!dnnCreateUser(txtUsername.Text, txtFirstName.Text, txtLastName.Text, txtEmail.Text, txtPassword.Text))
                        {
                            MessageBox.Show("Error occurred during user registration.  Please try again.");
                            btnActivate.Enabled = true;

                            return;
                        }
                        else
                            MessageBox.Show("User registered successfully");
                    }
                    else
                    {
                        MessageBox.Show("All fields are required for registration");
                        btnActivate.Enabled = true;

                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("activate", ex.StackTrace);
                return;
            }

            activationKey = activate();

            btnActivate.Enabled = true;

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private bool dnnLogin(string username, string password)
        {
            bool returnStatus = false;
            electrodatallc.dnn.WebService IWeb = new electrodatallc.dnn.WebService();
            electrodatallc.dnn.UserInfo objUserInfo;

            try
            {
                IWeb.Url = webserviceURL;
                IWeb.IWebAuthendicationHeaderValue = AttachCredentials();
                objUserInfo = IWeb.GetUser(username);

                if (objUserInfo.Membership.Password == password)
                {
                    returnStatus = true ;
                    
                }
                else
                {
                    returnStatus = false;
                    
                }
            }
            catch (Exception ex)
            {
                returnStatus = false;
            }

            return returnStatus;
        }

        private bool dnnCreateUser(string username, string firstName, string lastName, string email, string password)
        {
            string returnStatus = "";
            electrodatallc.dnn.WebService IWeb = new electrodatallc.dnn.WebService();
            bool created = false;

            try
            {
                IWeb.Url = webserviceURL;
                IWeb.IWebAuthendicationHeaderValue = AttachCredentials();

                returnStatus = IWeb.CreateUser(username, firstName, lastName, username, email, password);

                if (returnStatus == "Success")
                {
                    created = true;

                    
                }
                else
                    MessageBox.Show(returnStatus);
            }
            catch (Exception ex)
            {
                created = false;
            }
            
            return created;
        }

        private int getUserID(string username)
        {
            int userID = 0;
            electrodatallc.dnn.WebService IWeb = new electrodatallc.dnn.WebService();
            electrodatallc.dnn.UserInfo objUserInfo;

            try
            {
                IWeb.Url = webserviceURL;
                IWeb.IWebAuthendicationHeaderValue = AttachCredentials();
                objUserInfo = IWeb.GetUser(username);

                userID = objUserInfo.UserID;
            }
            catch (Exception ex)
            {

            }

            return userID;
        }

        /// <summary>
        /// Activate against webservice
        /// </summary>
        /// <returns></returns>
        private string activate()
        {
            string newKey = "";

            try
            {
                //send cpuID and save returned encrypted cpuID
                SCTech.SCTVWebService sctvWS = new SCTechUtilities.SCTech.SCTVWebService();
                //GetMachineInfo getMachineInfo = new GetMachineInfo();

                newKey = sctvWS.Activate(getUserID(txtUsername.Text.Trim()), "SCTV", GetMachineInfo.GetCPUId(), txtFirstName.Text, txtLastName.Text, txtAddress.Text, txtCity.Text, txtState.Text, txtCountry.Text, txtUsername.Text, txtPassword.Text);

                if (newKey.Contains("Error") || newKey.Length == 0)
                    this.DialogResult = DialogResult.Cancel;
                else
                    this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("SCTV", "activate - "+ ex.Message);
            }

            return newKey;
        }

        /// <summary>
        /// validate the text boxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBox_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                TextBox txtSender = (TextBox)sender;

                if (txtSender.Text == "")
                {
                    if (this.Controls.Find("lbl" + txtSender.Name.Substring(3), true).Length > 0)
                    {
                        Label lblSender = (Label)this.Controls.Find("lbl" + txtSender.Name.Substring(3), true)[0];

                        MessageBox.Show("You must enter a " + lblSender.Text + ".", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        txtSender.Focus();
                        e.Cancel = true;
                    }
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("SCTV", "txtBox_Validating - " + ex.Message);
            }            
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            //disable all registration controls
            txtAddress.Enabled = !radioButton1.Checked;
            txtCity.Enabled = !radioButton1.Checked;
            txtCountry.Enabled = !radioButton1.Checked;
            txtFirstName.Enabled = !radioButton1.Checked;
            txtLastName.Enabled = !radioButton1.Checked;
            txtState.Enabled = !radioButton1.Checked;
            txtEmail.Enabled = !radioButton1.Checked;
            
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            //enable all registration controls
            txtAddress.Enabled = radioButton2.Checked;
            txtCity.Enabled = radioButton2.Checked;
            txtCountry.Enabled = radioButton2.Checked;
            txtFirstName.Enabled = radioButton2.Checked;
            txtLastName.Enabled = radioButton2.Checked;
            txtState.Enabled = radioButton2.Checked;
            txtEmail.Enabled = radioButton2.Checked;
        }

        public electrodatallc.dnn.IWebAuthendicationHeader AttachCredentials()
        {
            electrodatallc.dnn.IWebAuthendicationHeader IWebAuthendicationHeader = new electrodatallc.dnn.IWebAuthendicationHeader();

            IWebAuthendicationHeader.PortalID = System.Convert.ToInt32(5);
            IWebAuthendicationHeader.Username = "host";
            IWebAuthendicationHeader.Password = "10Soccer!";
            //IWebAuthendicationHeader.Encrypted = System.Convert.ToString(IWeb_Core_Connector.IWeb_Form.TransDefaultFormIWeb_Form.chkEncrypted.Checked);
            IWebAuthendicationHeader.Encrypted = "false";

            return IWebAuthendicationHeader;
        }

        private void ActivationForm_Load(object sender, EventArgs e)
        {
            txtUsername.Focus();
        } 
    }
}