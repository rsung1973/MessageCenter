//---------------------------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated by T4Model template for T4 (https://github.com/linq2db/t4models).
//    Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//---------------------------------------------------------------------------------------------------
using System;
using System.Linq;

using LinqToDB;
using LinqToDB.Mapping;

namespace WebHome.DataModels
{
	/// <summary>
	/// Database       : dnake
	/// Data Source    : localhost
	/// Server Version : 5.5.28
	/// </summary>
	public partial class dnakeDB : LinqToDB.Data.DataConnection
	{
		public ITable<access_auth>   access_auth    { get { return this.GetTable<access_auth>(); } }
		public ITable<access_card>   access_card    { get { return this.GetTable<access_card>(); } }
		public ITable<access_jpeg>   access_jpeg    { get { return this.GetTable<access_jpeg>(); } }
		public ITable<access_logger> access_logger  { get { return this.GetTable<access_logger>(); } }
		public ITable<aclsavp>       aclsavps       { get { return this.GetTable<aclsavp>(); } }
		public ITable<ad_logger>     ad_logger      { get { return this.GetTable<ad_logger>(); } }
		public ITable<ad_store>      ad_store       { get { return this.GetTable<ad_store>(); } }
		public ITable<ad_wakeup>     ad_wakeup      { get { return this.GetTable<ad_wakeup>(); } }
		public ITable<alarm_content> alarm_content  { get { return this.GetTable<alarm_content>(); } }
		public ITable<alarm_defence> alarm_defence  { get { return this.GetTable<alarm_defence>(); } }
		public ITable<alarm_logger>  alarm_logger   { get { return this.GetTable<alarm_logger>(); } }
		public ITable<alarm_sensor>  alarm_sensor   { get { return this.GetTable<alarm_sensor>(); } }
		public ITable<alarm_zone>    alarm_zone     { get { return this.GetTable<alarm_zone>(); } }
		public ITable<configsavp>    configsavps    { get { return this.GetTable<configsavp>(); } }
		public ITable<device>        devices        { get { return this.GetTable<device>(); } }
		public ITable<filtersavp>    filtersavps    { get { return this.GetTable<filtersavp>(); } }
		public ITable<group>         groups         { get { return this.GetTable<group>(); } }
		public ITable<push_id>       push_id        { get { return this.GetTable<push_id>(); } }
		public ITable<routesavp>     routesavps     { get { return this.GetTable<routesavp>(); } }
		public ITable<siloavp>       siloavps       { get { return this.GetTable<siloavp>(); } }
		public ITable<sms_text>      sms_text       { get { return this.GetTable<sms_text>(); } }
		public ITable<sms_user>      sms_user       { get { return this.GetTable<sms_user>(); } }
		public ITable<staticregsavp> staticregsavps { get { return this.GetTable<staticregsavp>(); } }
		public ITable<talk_logger>   talk_logger    { get { return this.GetTable<talk_logger>(); } }
		public ITable<text_logger>   text_logger    { get { return this.GetTable<text_logger>(); } }
		public ITable<type>          types          { get { return this.GetTable<type>(); } }
		public ITable<user>          users          { get { return this.GetTable<user>(); } }

		public dnakeDB()
		{
			InitDataContext();
		}

		public dnakeDB(string configuration)
			: base(configuration)
		{
			InitDataContext();
		}

		partial void InitDataContext();
	}

	[Table("access_auth")]
	public partial class access_auth
	{
		[PrimaryKey, Identity   ] public int       id   { get; set; } // int(11)
		[Column,     NotNull    ] public string    card { get; set; } // varchar(64)
		[Column,     NotNull    ] public string    user { get; set; } // varchar(64)
		[Column,        Nullable] public int?      auth { get; set; } // int(11)
		[Column,        Nullable] public int?      done { get; set; } // int(11)
		[Column,        Nullable] public DateTime? ts   { get; set; } // datetime
	}

	[Table("access_card")]
	public partial class access_card
	{
		[PrimaryKey, Identity   ] public int       id      { get; set; } // int(11)
		[Column,     NotNull    ] public string    card    { get; set; } // varchar(64)
		[Column,        Nullable] public int?      gid     { get; set; } // int(11)
		[Column,        Nullable] public string    name    { get; set; } // varchar(64)
		[Column,        Nullable] public int?      build   { get; set; } // int(11)
		[Column,        Nullable] public int?      unit    { get; set; } // int(11)
		[Column,        Nullable] public int?      floor   { get; set; } // int(11)
		[Column,        Nullable] public int?      room    { get; set; } // int(11)
		[Column,        Nullable] public string    tel     { get; set; } // varchar(64)
		[Column,        Nullable] public DateTime? timeout { get; set; } // datetime
	}

	[Table("access_jpeg")]
	public partial class access_jpeg
	{
		[PrimaryKey, Identity   ] public int       id   { get; set; } // int(11)
		[Column,     NotNull    ] public string    user { get; set; } // varchar(64)
		[Column,     NotNull    ] public string    url  { get; set; } // varchar(128)
		[Column,        Nullable] public DateTime? ts   { get; set; } // datetime
	}

	[Table("access_logger")]
	public partial class access_logger
	{
		[PrimaryKey, Identity   ] public int       id   { get; set; } // int(11)
		[Column,     NotNull    ] public string    card { get; set; } // varchar(64)
		[Column,     NotNull    ] public string    user { get; set; } // varchar(64)
		[Column,        Nullable] public DateTime? ts   { get; set; } // datetime
	}

	[Table("aclsavp")]
	public partial class aclsavp
	{
		[PrimaryKey, NotNull    ] public string attr  { get; set; } // varchar(255)
		[Column,        Nullable] public string value { get; set; } // varchar(4096)
	}

	[Table("ad_logger")]
	public partial class ad_logger
	{
		[PrimaryKey, Identity   ] public int       id     { get; set; } // int(11)
		[Column,     NotNull    ] public string    user   { get; set; } // varchar(64)
		[Column,        Nullable] public int?      enable { get; set; } // int(11)
		[Column,     NotNull    ] public string    url    { get; set; } // varchar(512)
		[Column,        Nullable] public int?      done   { get; set; } // int(11)
		[Column,        Nullable] public DateTime? ts     { get; set; } // datetime
	}

	[Table("ad_store")]
	public partial class ad_store
	{
		[PrimaryKey, Identity   ] public int       id     { get; set; } // int(11)
		[Column,        Nullable] public int?      store  { get; set; } // int(11)
		[Column,     NotNull    ] public string    name   { get; set; } // varchar(128)
		[Column,     NotNull    ] public string    url    { get; set; } // varchar(128)
		[Column,        Nullable] public int?      expire { get; set; } // int(11)
		[Column,        Nullable] public DateTime? start  { get; set; } // datetime
		[Column,        Nullable] public DateTime? stop   { get; set; } // datetime
	}

	[Table("ad_wakeup")]
	public partial class ad_wakeup
	{
		[PrimaryKey, Identity   ] public int       id      { get; set; } // int(11)
		[Column,     NotNull    ] public string    name    { get; set; } // varchar(128)
		[Column,        Nullable] public int?      timeout { get; set; } // int(11)
		[Column,        Nullable] public int?      store   { get; set; } // int(11)
		[Column,     NotNull    ] public string    url     { get; set; } // varchar(512)
		[Column,        Nullable] public DateTime? ts      { get; set; } // datetime
	}

	[Table("alarm_content")]
	public partial class alarm_content
	{
		[PrimaryKey, Identity] public int    id      { get; set; } // int(11)
		[Column,     NotNull ] public string user    { get; set; } // varchar(64)
		[Column,     NotNull ] public int    zone    { get; set; } // int(11)
		[Column,     NotNull ] public string content { get; set; } // varchar(8192)
	}

	[Table("alarm_defence")]
	public partial class alarm_defence
	{
		[PrimaryKey, Identity   ] public int       id      { get; set; } // int(11)
		[Column,     NotNull    ] public string    user    { get; set; } // varchar(64)
		[Column,     NotNull    ] public int       defence { get; set; } // int(11)
		[Column,        Nullable] public string    io      { get; set; } // varchar(512)
		[Column,        Nullable] public DateTime? ts      { get; set; } // datetime
	}

	[Table("alarm_logger")]
	public partial class alarm_logger
	{
		[PrimaryKey, Identity   ] public int       id          { get; set; } // int(11)
		[Column,     NotNull    ] public string    user        { get; set; } // varchar(64)
		[Column,     NotNull    ] public int       zone        { get; set; } // int(11)
		[Column,     NotNull    ] public int       data        { get; set; } // int(11)
		[Column,        Nullable] public DateTime? ts          { get; set; } // datetime
		[Column,        Nullable] public int?      confirm     { get; set; } // int(11)
		[Column,        Nullable] public DateTime? confirmTs   { get; set; } // datetime
		[Column,        Nullable] public string    confirmUser { get; set; } // varchar(64)
		[Column,        Nullable] public string    confirmText { get; set; } // varchar(512)
	}

	[Table("alarm_sensor")]
	public partial class alarm_sensor
	{
		[PrimaryKey, Identity   ] public int    id     { get; set; } // int(11)
		[Column,        Nullable] public int?   sensor { get; set; } // int(11)
		[Column,     NotNull    ] public string name   { get; set; } // varchar(256)
	}

	[Table("alarm_zone")]
	public partial class alarm_zone
	{
		[PrimaryKey, Identity   ] public int    id     { get; set; } // int(11)
		[Column,     NotNull    ] public string user   { get; set; } // varchar(64)
		[Column,     NotNull    ] public int    zone   { get; set; } // int(11)
		[Column,     NotNull    ] public string name   { get; set; } // varchar(256)
		[Column,        Nullable] public int?   level  { get; set; } // int(11)
		[Column,        Nullable] public int?   sensor { get; set; } // int(11)
	}

	[Table("configsavp")]
	public partial class configsavp
	{
		[PrimaryKey, NotNull    ] public string attr  { get; set; } // varchar(255)
		[Column,        Nullable] public string value { get; set; } // varchar(4096)
	}

	[Table("devices")]
	public partial class device
	{
		[PrimaryKey, Identity   ] public int       id        { get; set; } // int(11)
		[Column,     NotNull    ] public string    user      { get; set; } // varchar(64)
		[Column,        Nullable] public int?      idx       { get; set; } // int(11)
		[Column,        Nullable] public string    url       { get; set; } // varchar(128)
		[Column,        Nullable] public string    proxy_url { get; set; } // varchar(128)
		[Column,        Nullable] public int?      proxy_ok  { get; set; } // int(11)
		[Column,        Nullable] public DateTime? heartbeat { get; set; } // datetime
	}

	[Table("filtersavp")]
	public partial class filtersavp
	{
		[PrimaryKey, NotNull    ] public string attr  { get; set; } // varchar(255)
		[Column,        Nullable] public string value { get; set; } // varchar(4096)
	}

	[Table("groups")]
	public partial class group
	{
		[PrimaryKey, Identity   ] public int    id     { get; set; } // int(11)
		[Column,        Nullable] public int?   parent { get; set; } // int(11)
		[Column,     NotNull    ] public string name   { get; set; } // varchar(256)
	}

	[Table("push_id")]
	public partial class push_id
	{
		[PrimaryKey, Identity   ] public int    id   { get; set; } // int(11)
		[Column,     NotNull    ] public string user { get; set; } // varchar(64)
		[Column,        Nullable] public int?   idx  { get; set; } // int(11)
		[Column,     NotNull    ] public string sid  { get; set; } // varchar(128)
	}

	[Table("routesavp")]
	public partial class routesavp
	{
		[PrimaryKey, NotNull    ] public string attr  { get; set; } // varchar(255)
		[Column,        Nullable] public string value { get; set; } // varchar(4096)
	}

	[Table("siloavp")]
	public partial class siloavp
	{
		[PrimaryKey, NotNull    ] public string attr  { get; set; } // varchar(255)
		[Column,     NotNull    ] public string attr2 { get; set; } // varchar(255)
		[Column,        Nullable] public string value { get; set; } // varchar(20315)
	}

	[Table("sms_text")]
	public partial class sms_text
	{
		[PrimaryKey, Identity   ] public int       id   { get; set; } // int(11)
		[Column,     NotNull    ] public string    user { get; set; } // varchar(64)
		[Column,     NotNull    ] public string    tel  { get; set; } // varchar(64)
		[Column,     NotNull    ] public string    text { get; set; } // varchar(256)
		[Column,        Nullable] public int?      done { get; set; } // int(11)
		[Column,        Nullable] public DateTime? ts   { get; set; } // datetime
	}

	[Table("sms_user")]
	public partial class sms_user
	{
		[PrimaryKey, Identity] public int    id   { get; set; } // int(11)
		[Column,     NotNull ] public string user { get; set; } // varchar(64)
		[Column,     NotNull ] public string name { get; set; } // varchar(64)
		[Column,     NotNull ] public string tel  { get; set; } // varchar(64)
	}

	[Table("staticregsavp")]
	public partial class staticregsavp
	{
		[PrimaryKey, NotNull    ] public string attr  { get; set; } // varchar(255)
		[Column,        Nullable] public string value { get; set; } // varchar(4096)
	}

	[Table("talk_logger")]
	public partial class talk_logger
	{
		[PrimaryKey, Identity   ] public int       id      { get; set; } // int(11)
		[Column,     NotNull    ] public string    user    { get; set; } // varchar(64)
		[Column,     NotNull    ] public string    to_user { get; set; } // varchar(64)
		[Column,        Nullable] public int?      mode    { get; set; } // int(11)
		[Column,        Nullable] public DateTime? ts      { get; set; } // datetime
	}

	[Table("text_logger")]
	public partial class text_logger
	{
		[PrimaryKey, Identity   ] public int       id   { get; set; } // int(11)
		[Column,     NotNull    ] public string    user { get; set; } // varchar(64)
		[Column,        Nullable] public int?      type { get; set; } // int(11)
		[Column,     NotNull    ] public string    text { get; set; } // varchar(512)
		[Column,        Nullable] public int?      done { get; set; } // int(11)
		[Column,        Nullable] public DateTime? ts   { get; set; } // datetime
	}

	[Table("types")]
	public partial class type
	{
		[PrimaryKey, Identity] public int    id   { get; set; } // int(11)
		[Column,     NotNull ] public int    tid  { get; set; } // int(11)
		[Column,     NotNull ] public string name { get; set; } // varchar(256)
	}

	[Table("users")]
	public partial class user
	{
		[Column(),       PrimaryKey,  Identity] public int       id             { get; set; } // int(11)
		[Column("user"), NotNull              ] public string    user_Column    { get; set; } // varchar(64)
		[Column(),          Nullable          ] public string    passwd         { get; set; } // varchar(32)
		[Column(),          Nullable          ] public string    name           { get; set; } // varchar(256)
		[Column(),          Nullable          ] public string    forwardAddress { get; set; } // varchar(128)
		[Column(),          Nullable          ] public DateTime? heartbeat      { get; set; } // datetime
		[Column(),          Nullable          ] public int?      online         { get; set; } // int(11)
		[Column(),          Nullable          ] public int?      gid            { get; set; } // int(11)
		[Column(),          Nullable          ] public int?      tid            { get; set; } // int(11)
		[Column(),          Nullable          ] public int?      build          { get; set; } // int(11)
		[Column(),          Nullable          ] public int?      unit           { get; set; } // int(11)
		[Column(),          Nullable          ] public int?      floor          { get; set; } // int(11)
		[Column(),          Nullable          ] public int?      room           { get; set; } // int(11)
		[Column(),          Nullable          ] public int?      idx            { get; set; } // int(11)
	}

	public static partial class TableExtensions
	{
		public static access_auth Find(this ITable<access_auth> table, int id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static access_card Find(this ITable<access_card> table, int id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static access_jpeg Find(this ITable<access_jpeg> table, int id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static access_logger Find(this ITable<access_logger> table, int id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static aclsavp Find(this ITable<aclsavp> table, string attr)
		{
			return table.FirstOrDefault(t =>
				t.attr == attr);
		}

		public static ad_logger Find(this ITable<ad_logger> table, int id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static ad_store Find(this ITable<ad_store> table, int id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static ad_wakeup Find(this ITable<ad_wakeup> table, int id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static alarm_content Find(this ITable<alarm_content> table, int id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static alarm_defence Find(this ITable<alarm_defence> table, int id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static alarm_logger Find(this ITable<alarm_logger> table, int id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static alarm_sensor Find(this ITable<alarm_sensor> table, int id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static alarm_zone Find(this ITable<alarm_zone> table, int id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static configsavp Find(this ITable<configsavp> table, string attr)
		{
			return table.FirstOrDefault(t =>
				t.attr == attr);
		}

		public static device Find(this ITable<device> table, int id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static filtersavp Find(this ITable<filtersavp> table, string attr)
		{
			return table.FirstOrDefault(t =>
				t.attr == attr);
		}

		public static group Find(this ITable<group> table, int id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static push_id Find(this ITable<push_id> table, int id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static routesavp Find(this ITable<routesavp> table, string attr)
		{
			return table.FirstOrDefault(t =>
				t.attr == attr);
		}

		public static siloavp Find(this ITable<siloavp> table, string attr)
		{
			return table.FirstOrDefault(t =>
				t.attr == attr);
		}

		public static sms_text Find(this ITable<sms_text> table, int id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static sms_user Find(this ITable<sms_user> table, int id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static staticregsavp Find(this ITable<staticregsavp> table, string attr)
		{
			return table.FirstOrDefault(t =>
				t.attr == attr);
		}

		public static talk_logger Find(this ITable<talk_logger> table, int id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static text_logger Find(this ITable<text_logger> table, int id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static type Find(this ITable<type> table, int id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static user Find(this ITable<user> table, int id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}
	}
}
