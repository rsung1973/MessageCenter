// Decompiled with JetBrains decompiler
// Type: WebHome.DataModels.alarm_logger
// Assembly: MessageCenter, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F7B9E225-6A49-454A-B092-9C198D8EF217
// Assembly location: G:\temp\MessageCenter.dll

using LinqToDB.Mapping;
using System;

namespace WebHome.DataModels
{
  [Table("alarm_logger")]
  public class alarm_logger
  {
    [PrimaryKey]
    [Identity]
    public int id { get; set; }

    [Column]
    [NotNull]
    public string user { get; set; }

    [Column]
    [NotNull]
    public int zone { get; set; }

    [Column]
    [NotNull]
    public int data { get; set; }

    [Column]
    [Nullable]
    public DateTime? ts { get; set; }

    [Column]
    [Nullable]
    public int? confirm { get; set; }

    [Column]
    [Nullable]
    public DateTime? confirmTs { get; set; }

    [Column]
    [Nullable]
    public string confirmUser { get; set; }

    [Column]
    [Nullable]
    public string confirmText { get; set; }
  }
}
