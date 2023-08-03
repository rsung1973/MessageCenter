// Decompiled with JetBrains decompiler
// Type: WebHome.DataModels.ad_store
// Assembly: MessageCenter, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F7B9E225-6A49-454A-B092-9C198D8EF217
// Assembly location: G:\temp\MessageCenter.dll

using LinqToDB.Mapping;
using System;

namespace WebHome.DataModels
{
  [Table("ad_store")]
  public class ad_store
  {
    [PrimaryKey]
    [Identity]
    public int id { get; set; }

    [Column]
    [Nullable]
    public int? store { get; set; }

    [Column]
    [NotNull]
    public string name { get; set; }

    [Column]
    [NotNull]
    public string url { get; set; }

    [Column]
    [Nullable]
    public int? expire { get; set; }

    [Column]
    [Nullable]
    public DateTime? start { get; set; }

    [Column]
    [Nullable]
    public DateTime? stop { get; set; }
  }
}
