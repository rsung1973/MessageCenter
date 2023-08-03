// Decompiled with JetBrains decompiler
// Type: WebHome.DataModels.dnakeDB
// Assembly: MessageCenter, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F7B9E225-6A49-454A-B092-9C198D8EF217
// Assembly location: G:\temp\MessageCenter.dll

using LinqToDB;
using LinqToDB.Data;

namespace WebHome.DataModels
{
  public class dnakeDB : DataConnection
  {
    public ITable<WebHome.DataModels.access_auth> access_auth => this.GetTable<WebHome.DataModels.access_auth>();

    public ITable<WebHome.DataModels.access_card> access_card => this.GetTable<WebHome.DataModels.access_card>();

    public ITable<WebHome.DataModels.access_jpeg> access_jpeg => this.GetTable<WebHome.DataModels.access_jpeg>();

    public ITable<WebHome.DataModels.access_logger> access_logger => this.GetTable<WebHome.DataModels.access_logger>();

    public ITable<aclsavp> aclsavps => this.GetTable<aclsavp>();

    public ITable<WebHome.DataModels.ad_logger> ad_logger => this.GetTable<WebHome.DataModels.ad_logger>();

    public ITable<WebHome.DataModels.ad_store> ad_store => this.GetTable<WebHome.DataModels.ad_store>();

    public ITable<WebHome.DataModels.ad_wakeup> ad_wakeup => this.GetTable<WebHome.DataModels.ad_wakeup>();

    public ITable<WebHome.DataModels.alarm_content> alarm_content => this.GetTable<WebHome.DataModels.alarm_content>();

    public ITable<WebHome.DataModels.alarm_defence> alarm_defence => this.GetTable<WebHome.DataModels.alarm_defence>();

    public ITable<WebHome.DataModels.alarm_logger> alarm_logger => this.GetTable<WebHome.DataModels.alarm_logger>();

    public ITable<WebHome.DataModels.alarm_sensor> alarm_sensor => this.GetTable<WebHome.DataModels.alarm_sensor>();

    public ITable<WebHome.DataModels.alarm_zone> alarm_zone => this.GetTable<WebHome.DataModels.alarm_zone>();

    public ITable<configsavp> configsavps => this.GetTable<configsavp>();

    public ITable<device> devices => this.GetTable<device>();

    public ITable<filtersavp> filtersavps => this.GetTable<filtersavp>();

    public ITable<group> groups => this.GetTable<group>();

    public ITable<WebHome.DataModels.push_id> push_id => this.GetTable<WebHome.DataModels.push_id>();

    public ITable<routesavp> routesavps => this.GetTable<routesavp>();

    public ITable<siloavp> siloavps => this.GetTable<siloavp>();

    public ITable<WebHome.DataModels.sms_text> sms_text => this.GetTable<WebHome.DataModels.sms_text>();

    public ITable<WebHome.DataModels.sms_user> sms_user => this.GetTable<WebHome.DataModels.sms_user>();

    public ITable<staticregsavp> staticregsavps => this.GetTable<staticregsavp>();

    public ITable<WebHome.DataModels.talk_logger> talk_logger => this.GetTable<WebHome.DataModels.talk_logger>();

    public ITable<WebHome.DataModels.text_logger> text_logger => this.GetTable<WebHome.DataModels.text_logger>();

    public ITable<type> types => this.GetTable<type>();

    public ITable<user> users => this.GetTable<user>();

    public dnakeDB()
    {
    }

    public dnakeDB(string configuration)
      : base(configuration)
    {
    }
  }
}
