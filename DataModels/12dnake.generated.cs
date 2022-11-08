// Decompiled with JetBrains decompiler
// Type: WebHome.DataModels.alarm_sensor
// Assembly: MessageCenter, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F7B9E225-6A49-454A-B092-9C198D8EF217
// Assembly location: G:\temp\MessageCenter.dll

using LinqToDB.Mapping;

namespace WebHome.DataModels
{
  [Table("alarm_sensor")]
  public class alarm_sensor
  {
    [PrimaryKey]
    [Identity]
    public int id { get; set; }

    [Column]
    [Nullable]
    public int? sensor { get; set; }

    [Column]
    [NotNull]
    public string name { get; set; }
  }
}
