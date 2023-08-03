// Decompiled with JetBrains decompiler
// Type: WebHome.DataModels.access_card
// Assembly: MessageCenter, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F7B9E225-6A49-454A-B092-9C198D8EF217
// Assembly location: G:\temp\MessageCenter.dll

using LinqToDB.Mapping;
using System;

namespace WebHome.DataModels
{
  [Table("access_card")]
  public class access_card
  {
    [PrimaryKey]
    [Identity]
    public int id { get; set; }

    [Column]
    [NotNull]
    public string card { get; set; }

    [Column]
    [Nullable]
    public int? gid { get; set; }

    [Column]
    [Nullable]
    public string name { get; set; }

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
    public string tel { get; set; }

    [Column]
    [Nullable]
    public DateTime? timeout { get; set; }
  }
}
