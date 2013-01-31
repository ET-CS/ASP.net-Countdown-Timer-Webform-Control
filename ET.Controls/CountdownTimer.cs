using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;

namespace ET.Controls
{
    public enum EndAction{
    doStop = 0,
    doRefresh = 1,
    doPost = 2,
    doPartialPost = 3}

    /// <summary>
    /// ET.Controls.CountdownTimer Control
    /// </summary>
    [ToolboxData("<{0}:CountdownTimer runat=server />")]
    public class CountdownTimer : ScriptControl, INamingContainer, ICallbackEventHandler
    {

        #region "properties"
            private TimeSpan timelengthvalue;
            [Category("Behavior")]        
            [Description("Total TimeSpan to countdown")]
            public TimeSpan TimeLength
            {
                get { return timelengthvalue; }
                set { timelengthvalue = value; }
            }

            private EndAction endactionvalue = ET.Controls.EndAction.doPartialPost;
            [Category("Behavior")]
            [DefaultValue(EndAction.doPartialPost)]
            [Description("End action to perform when time out")]
            public EndAction EndAction
            {
                get { return endactionvalue; }
                set { endactionvalue = value; }
            }

            private bool onlinevalue = true;
            [Category("Behavior")]
            [DefaultValue(true)]
            [Description("Enable or Disable JS timeOut On Countdown")]
            public bool Online
            {
                get { return onlinevalue; }
                set { onlinevalue = value; }
            }

            private string timeoutvalue;
            [Category("Behavior")]
            [DefaultValue("")]
            [UrlProperty]
            [Description("URL to redirect the user, in the event of a membership session timeout")]
            public string TimeoutURL
            {
                get { return timeoutvalue; }
                set { timeoutvalue = value; }
            }
        #endregion

        #region "Callback"
        public void RaiseCallbackEvent(String eventArgument)
        {
            // All we're doing here is resetting the sliding expiration
            // of the forms authentication.  No additional code is needed.
        }

        public String GetCallbackResult()
        {
            // return an emtpy string.  We're not really interested
            // in the return value.
            return String.Empty;
        }
        #endregion

        public CountdownTimer()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        protected override IEnumerable<ScriptDescriptor>
                GetScriptDescriptors()
        {
            ScriptControlDescriptor descriptor = new ScriptControlDescriptor("ET.Controls.CountdownTimer", this.ClientID);
            
            // Add our TimeLength property to be passed down to the client
            descriptor.AddProperty("timelength", this.timelengthvalue.Ticks);
            descriptor.AddProperty("date", DateTime.UtcNow.Add(this.timelengthvalue));
            descriptor.AddProperty("endaction", this.endactionvalue);
            descriptor.AddProperty("enabled", this.Online);

            yield return descriptor;
        }

        // Generate the script reference
        protected override IEnumerable<ScriptReference>
                GetScriptReferences()
        {
            yield return new ScriptReference("ET.Controls.CountdownTimer.js", this.GetType().Assembly.FullName);
        }
        
        /// <summary>
        /// returns the number of milliseconds since Jan 1, 1970 (useful for converting C# dates to JS dates)
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        Double UnixTicks(DateTime dt) {
            DateTime d1 = new DateTime(1970, 1, 1);
            DateTime d2 = dt.ToUniversalTime();
            TimeSpan ts = new TimeSpan(d2.Ticks - d1.Ticks);
            return ts.TotalMilliseconds;
        }

#region "Render Control"

    void InitialiseControls() {
        Label lbl = new Label();
        lbl.ClientIDMode = ClientIDMode.AutoID;
        lbl.Text = TimeLength.Ticks.ToString();
        this.Controls.Add(lbl);
    }

    protected override void OnPreRender(EventArgs e)
    {
        if (!this.DesignMode)
        {
            if (ScriptManager.GetCurrent(Page) == null)
            {
                throw new HttpException("A ScriptManager control must exist on the current page.");
            }
            InitialiseControls();
            
            String cbReference =
            Page.ClientScript.GetCallbackEventReference(this,
                                                "arg", "ReceiveServerData", "context");
            String callbackScript;
            callbackScript = "function CallServer(arg, context)" +
                             "{ " + cbReference + ";}";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(),
                                                        "CallServer", callbackScript, true);
        }        
        base.OnPreRender(e);
    }    
    
#endregion

    }
}