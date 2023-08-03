// Decompiled with JetBrains decompiler
// Type: WebHome.DataModels.ad_wakeup
// Assembly: MessageCenter, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F7B9E225-6A49-454A-B092-9C198D8EF217
// Assembly location: G:\temp\MessageCenter.dll

using LinqToDB.Mapping;
using System;

namespace WebHome.DataModels
{
  [Table("ad_wakeup")]
  public class ad_wakeup
  {
    [PrimaryKey]
    [Identity]
    public int id { get; set; }

    [Column]
    [NotNull]
    public string name { get; set; }

    [Column]
    [Nullable]
    public int? timeout { get; set; }

    [Column]
    [Nullable]
    public int? store { get; set; }

    [Column]
    [NotNull]
    public string url { get; set; }

    [Column]
    [Nullable]
    public DateTime? ts { get; set; }
  }
}
