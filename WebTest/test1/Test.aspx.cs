using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using EtoolTech.MongoDB.Mapper.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace WebTest
{
    public partial class Test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {            
            Response.Write(ConfigManager.Config.ToJson());
        }
    }
}