// Decompiled with JetBrains decompiler
// Type: WebHome.DataModels.alarm_defence
// Assembly: MessageCenter, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F7B9E225-6A49-454A-B092-9C198D8EF217
// Assembly location: G:\temp\MessageCenter.dll

using LinqToDB.Mapping;
using System;

namespace WebHome.DataModels
{
  [Table("alarm_defence")]
  public class alarm_defence
  {
    [PrimaryKey]
    [Identity]
    public int id { get; set; }

    [Column]
    [NotNull]
    public string user { get; set; }

    [Column]
    [NotNull]
    public int defence { get; set; }

    [Column]
    [Nullable]
    public string io { get; set; }

    [Column]
    [Nullable]
    public DateTime? ts { get; set; }
  }
}
