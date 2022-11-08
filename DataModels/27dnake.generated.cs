// Decompiled with JetBrains decompiler
// Type: WebHome.DataModels.user
// Assembly: MessageCenter, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F7B9E225-6A49-454A-B092-9C198D8EF217
// Assembly location: G:\temp\MessageCenter.dll

using LinqToDB.Mapping;
using System;

namespace WebHome.DataModels
{
  [Table("users")]
  public class user
  {
    [Column]
    [PrimaryKey]
    [Identity]
    public int id { get; set; }

    [Column("user")]
    [NotNull]
    public string user_Column { get; set; }

    [Column]
    [Nullable]
    public string passwd { get; set; }

    [Column]
    [Nullable]
    public string name { get; set; }

    [Column]
    [Nullable]
    public string forwardAddress { get; set; }

    [Column]
    [Nullable]
    public DateTime? heartbeat { get; set; }

    [Column]
    [Nullable]
    public int? online { get; set; }

    [Column]
    [Nullable]
    public int? gid { get; set; }

    [Column]
    [Nullable]
    public int? tid { get; set; }

    [Column]
    [Nullable]
    public int? build { get; set; }

    [Column]
    [Nullable]
    public int? unit { get; set; }

    [Column]
    [Nullable]
    public int? floor { get; set; }

    [Column]
    [Nullable]
    public int? room { get; set; }

    [Column]
    [Nullable]
    public int? idx { get; set; }
  }
}
