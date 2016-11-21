using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using EtoolTech.MongoDB.Mapper.Configuration;
using MongoDB.Bson;

namespace WebTest.test2
{
    public partial class Test2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Write(ConfigManager.Config.ToJson());
        }
    }
}