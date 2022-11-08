// Decompiled with JetBrains decompiler
// Type: WebHome.DataModels.TableExtensions
// Assembly: MessageCenter, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F7B9E225-6A49-454A-B092-9C198D8EF217
// Assembly location: G:\temp\MessageCenter.dll

using LinqToDB;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace WebHome.DataModels
{
  public static class TableExtensions
  {
    public static access_auth Find(this ITable<access_auth> table, int id) => table.FirstOrDefault<access_auth>((Expression<Func<access_auth, bool>>) (t => t.id == id));

    public static access_card Find(this ITable<access_card> table, int id) => table.FirstOrDefault<access_card>((Expression<Func<access_card, bool>>) (t => t.id == id));

    public static access_jpeg Find(this ITable<access_jpeg> table, int id) => table.FirstOrDefault<access_jpeg>((Expression<Func<access_jpeg, bool>>) (t => t.id == id));

    public static access_logger Find(this ITable<access_logger> table, int id) => table.FirstOrDefault<access_logger>((Expression<Func<access_logger, bool>>) (t => t.id == id));

    public static aclsavp Find(this ITable<aclsavp> table, string attr) => table.FirstOrDefault<aclsavp>((Expression<Func<aclsavp, bool>>) (t => t.attr == attr));

    public static ad_logger Find(this ITable<ad_logger> table, int id) => table.FirstOrDefault<ad_logger>((Expression<Func<ad_logger, bool>>) (t => t.id == id));

    public static ad_store Find(this ITable<ad_store> table, int id) => table.FirstOrDefault<ad_store>((Expression<Func<ad_store, bool>>) (t => t.id == id));

    public static ad_wakeup Find(this ITable<ad_wakeup> table, int id) => table.FirstOrDefault<ad_wakeup>((Expression<Func<ad_wakeup, bool>>) (t => t.id == id));

    public static alarm_content Find(this ITable<alarm_content> table, int id) => table.FirstOrDefault<alarm_content>((Expression<Func<alarm_content, bool>>) (t => t.id == id));

    public static alarm_defence Find(this ITable<alarm_defence> table, int id) => table.FirstOrDefault<alarm_defence>((Expression<Func<alarm_defence, bool>>) (t => t.id == id));

    public static alarm_logger Find(this ITable<alarm_logger> table, int id) => table.FirstOrDefault<alarm_logger>((Expression<Func<alarm_logger, bool>>) (t => t.id == id));

    public static alarm_sensor Find(this ITable<alarm_sensor> table, int id) => table.FirstOrDefault<alarm_sensor>((Expression<Func<alarm_sensor, bool>>) (t => t.id == id));

    public static alarm_zone Find(this ITable<alarm_zone> table, int id) => table.FirstOrDefault<alarm_zone>((Expression<Func<alarm_zone, bool>>) (t => t.id == id));

    public static configsavp Find(this ITable<configsavp> table, string attr) => table.FirstOrDefault<configsavp>((Expression<Func<configsavp, bool>>) (t => t.attr == attr));

    public static device Find(this ITable<device> table, int id) => table.FirstOrDefault<device>((Expression<Func<device, bool>>) (t => t.id == id));

    public static filtersavp Find(this ITable<filtersavp> table, string attr) => table.FirstOrDefault<filtersavp>((Expression<Func<filtersavp, bool>>) (t => t.attr == attr));

    public static group Find(this ITable<group> table, int id) => table.FirstOrDefault<group>((Expression<Func<group, bool>>) (t => t.id == id));

    public static push_id Find(this ITable<push_id> table, int id) => table.FirstOrDefault<push_id>((Expression<Func<push_id, bool>>) (t => t.id == id));

    public static routesavp Find(this ITable<routesavp> table, string attr) => table.FirstOrDefault<routesavp>((Expression<Func<routesavp, bool>>) (t => t.attr == attr));

    public static siloavp Find(this ITable<siloavp> table, string attr) => table.FirstOrDefault<siloavp>((Expression<Func<siloavp, bool>>) (t => t.attr == attr));

    public static sms_text Find(this ITable<sms_text> table, int id) => table.FirstOrDefault<sms_text>((Expression<Func<sms_text, bool>>) (t => t.id == id));

    public static sms_user Find(this ITable<sms_user> table, int id) => table.FirstOrDefault<sms_user>((Expression<Func<sms_user, bool>>) (t => t.id == id));

    public static staticregsavp Find(this ITable<staticregsavp> table, string attr) => table.FirstOrDefault<staticregsavp>((Expression<Func<staticregsavp, bool>>) (t => t.attr == attr));

    public static talk_logger Find(this ITable<talk_logger> table, int id) => table.FirstOrDefault<talk_logger>((Expression<Func<talk_logger, bool>>) (t => t.id == id));

    public static text_logger Find(this ITable<text_logger> table, int id) => table.FirstOrDefault<text_logger>((Expression<Func<text_logger, bool>>) (t => t.id == id));

    public static type Find(this ITable<type> table, int id) => table.FirstOrDefault<type>((Expression<Func<type, bool>>) (t => t.id == id));

    public static user Find(this ITable<user> table, int id) => table.FirstOrDefault<user>((Expression<Func<user, bool>>) (t => t.id == id));
  }
}
